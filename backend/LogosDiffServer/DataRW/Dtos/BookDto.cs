namespace DataRW.Dtos
{
    public record BookDto(
        string Abbreviature, 
        int Testament, 
        string Name, 
        IEnumerable<string[]> Chapters);
}
