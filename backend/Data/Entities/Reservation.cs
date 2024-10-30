namespace backend.Data.Entities;

public class Reservation
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public required int Days { get; set; }
    public required bool QuickPickup { get; set; }
    public required decimal TotalCost { get; set; }
    public DateTime ReservationDate { get; set; } = DateTime.UtcNow;
    public required DateTime ReturnDate { get; set; }   
    public Book Book { get; set; } = null!;
    public ReservationDto ToDto() => new ReservationDto(Id, BookId, Days, QuickPickup, TotalCost, ReservationDate, ReturnDate);
}