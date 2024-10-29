namespace LibraryApp.Data.Entities;

public class Reservation
{
    public int Id { get; set; }
    public required int BookId { get; set; }
    public required Book Book { get; set; }
    public required int Days { get; set; }
    public required bool QuickPickUp { get; set; }
    public required decimal TotalCost { get; set; }
    public required DateTime ReservationDate { get; set; }

    public required string UserId { get; set; }

    public ReservationDto ToDto() => new ReservationDto(Id, BookId, Days, QuickPickUp, TotalCost, ReservationDate);
}