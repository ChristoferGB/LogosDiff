namespace Domain.Model.Entities
{
    public class Verse
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public string Content { get; set; } = string.Empty;

        public Guid ChapterId { get; set; }
        public Chapter Chapter { get; set; } = new();

        public Guid TranslationId { get; set; }
        public Translation Translation { get; set; } = new();
    }
}
