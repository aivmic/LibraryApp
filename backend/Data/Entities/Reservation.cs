namespace backend.Data.Entities;

public class Reservation
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public int Days { get; set; }
    public bool QuickPickup { get; set; }
    public decimal TotalCost { get; set; }
    public DateTime ReservationDate { get; set; } = DateTime.Now;

    // Navigation property to the Book
    public Book Book { get; set; } = null!;
    public ReservationDto ToDto() => new ReservationDto(Id, BookId, Days, QuickPickup, TotalCost, ReservationDate);
}