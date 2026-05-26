using DataRW.Dtos;
using Domain.Model.Entities;
using Domain.Model.Enums;
using Domain.Repositories;
using Infrastucture.Persistence.Context;
using Infrastucture.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Text.Json;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddDbContext<ApplicationDataContext>(options =>
                    options.UseSqlite(context.Configuration.GetConnectionString("DefaultConnection")));

                services.AddScoped<IBookRepository, BookRepository>();
                services.AddScoped<IChapterRepository, ChapterRepository>();
                services.AddScoped<IVerseRepository, VerseRepository>();
                services.AddScoped<ITranslationRepository, TranslationRepository>();

                services.AddScoped<DataProcessor>();
            })
            .Build();

        using (var scope = host.Services.CreateScope())
        {
            var dataProcessor = scope.ServiceProvider.GetRequiredService<DataProcessor>();
            await dataProcessor.ProcessAsync(args);
        }
    }
}

internal class DataProcessor
{
    private readonly IBookRepository _bookRepository;
    private readonly IChapterRepository _chapterRepository;
    private readonly IVerseRepository _verseRepository;
    private readonly ITranslationRepository _translationRepository;
    private readonly ApplicationDataContext _context;

    public DataProcessor(
        IBookRepository bookRepository, 
        IChapterRepository chapterRepository, 
        IVerseRepository verseRepository, 
        ITranslationRepository translationRepository,
        ApplicationDataContext context)
    {
        _bookRepository = bookRepository;
        _chapterRepository = chapterRepository;
        _verseRepository = verseRepository;
        _translationRepository = translationRepository;
        _context = context;
    }

    public async Task ProcessAsync(string[] args)
    {
        var jsonsPath = GetJsonsPathInFolder();

        var isFirstExecution = args.Length != 0 && args[0] == "1";

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        foreach (var jsonPath in jsonsPath)
        {
            var json = await File.ReadAllTextAsync(jsonPath);

            var dto = JsonSerializer.Deserialize<JsonDto>(json, options) ??
                throw new InvalidOperationException("Failed to deserialize JSON.");

            await MapsAndInsertsDataIntoDatabase(dto, isFirstExecution);
        }
    }

    private static string[] GetJsonsPathInFolder()
    {
        var path = Path.GetFullPath(@"C:\Git\LogosDiff\backend\LogosDiffServer\DataRW\Json\");

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var jsonsPath = Directory.GetFiles(path, "*.json");

        return jsonsPath ?? [];
    }

    private async Task MapsAndInsertsDataIntoDatabase(JsonDto dto, bool isFirstExecution)
    {
        var sp = Stopwatch.StartNew();

        await ExecuteInTransactionAsync(async () =>
        {
            var verses = new List<Verse>(31200);

            var (books, chapters) = InsertOrRetrieveBooksAndChapter(dto, isFirstExecution);

            var translation = new Translation
            {
                Id = Guid.NewGuid(),
                Name = dto.TranslationFullName,
                Abbreviature = dto.TranslationAbbreviature,
                Language = Enum.Parse<LanguageEnum>(dto.Language.ToUpper()),
                Year = dto.Year
            };

            var chaptersLookup = chapters.ToLookup(c => c.BookId);
            var dtoBooksByAbbrev = dto.Books.ToDictionary(b => b.Abbreviature.ToUpper(), b => b);

            foreach (var book in books)
            {
                foreach (var chapter in chaptersLookup[book.Id])
                {
                    var versesDto = dtoBooksByAbbrev[book.Abbreviature]
                        .Chapters
                        .ElementAt(chapter.Number - 1) ?? [];

                    var verseNumber = 1;

                    foreach (var verseDto in versesDto)
                    {
                        verses.Add(new Verse
                        {
                            Id = Guid.NewGuid(),
                            Number = verseNumber++,
                            Content = verseDto,
                            ChapterId = chapter.Id,
                            TranslationId = translation.Id
                        });
                    }
                }
            }

            _translationRepository.AddItem(translation);
            _verseRepository.AddRange(verses);

            await _context.SaveChangesAsync();
        });

        sp.Stop();
        Console.WriteLine($"Execution time: {sp.Elapsed}");
    }

    private (List<Book> books, List<Chapter> chapters) InsertOrRetrieveBooksAndChapter(JsonDto dto, bool isFirstExecution)
    {
        var books = new List<Book>();
        var chapters = new List<Chapter>();

        if (isFirstExecution)
        {
            var bookIndex = 1;

            foreach (var bookDto in dto.Books)
            {
                var book = new Book
                {
                    Id = Guid.NewGuid(),
                    Name = bookDto.Name,
                    Abbreviature = bookDto.Abbreviature.ToUpper(),
                    Index = bookIndex,
                    NumberOfChapters = bookDto.Chapters.Count(),
                    Testament = (TestamentEnum)bookDto.Testament
                };

                bookIndex++;
                books.Add(book);

                var chapterNumber = 1;

                foreach (var chapterVerses in bookDto.Chapters)
                {
                    var chapter = new Chapter
                    {
                        Id = Guid.NewGuid(),
                        Number = chapterNumber,
                        NumberOfVerses = chapterVerses.Length,
                        BookId = book.Id
                    };

                    chapterNumber++;
                    chapters.Add(chapter);
                }
            }

            _bookRepository.AddRange(books);
            _chapterRepository.AddRange(chapters);
        }
        else
        {
            books = _bookRepository.GetAllItems();
            chapters = _chapterRepository.GetAllItems();
        }

        return (books, chapters);
    }

    private async Task ExecuteInTransactionAsync(Func<Task> action)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            _context.ChangeTracker.AutoDetectChangesEnabled = false;

            await action();

            _context.ChangeTracker.DetectChanges();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
        finally
        {
            _context.ChangeTracker.AutoDetectChangesEnabled = true;
        }
    }
}