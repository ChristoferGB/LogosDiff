namespace DataRW.Dtos
{
    public record JsonDto(
        string TranslationAbbreviature,
        string TranslationFullName,
        string Language, 
        int Year,
        IEnumerable<BookDto> Books);
}
