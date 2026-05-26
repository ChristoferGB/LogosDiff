using Domain.Model.Enums;

namespace Domain.Model.Entities
{
    public class Translation
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Abbreviature { get; set; } = string.Empty;
        public LanguageEnum Language { get; set; }
        public int Year { get; set; }

        public List<Verse> Verses { get; set; } = [];
    }
}
