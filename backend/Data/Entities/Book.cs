namespace backend.Data.Entities;

public class Book
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required int Year { get; set; }
    public required string Picture { get; set; }
    public required BookType Type { get; set; }
    //public List<Reservation> Reservations { get; set; } = new();
    
    public BookDto ToDto() => new BookDto(Id, Name, Year, Picture, Type);
}
public enum BookType
{
    Book,
    Audiobook
}





