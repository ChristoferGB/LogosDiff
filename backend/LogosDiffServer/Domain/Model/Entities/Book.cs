using Domain.Model.Enums;

namespace Domain.Model.Entities
{
    public class Book
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int NumberOfChapters { get; set; }
        public TestamentEnum Testament { get; set; }
        
        public List<Chapter> Chapters { get; } = [];
    }
}