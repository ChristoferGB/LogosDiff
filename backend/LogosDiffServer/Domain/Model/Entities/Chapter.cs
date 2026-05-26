namespace Domain.Model.Entities
{
    public class Chapter
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public int NumberOfVerses { get; set; }

        public Guid BookId { get; set; }
        public Book? Book { get; set; }
    }
}
