namespace LibraryApp.Data.Entities;

public class Reservation
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string Type { get; set; }
    public string QuickPickUp { get; set; }
    public int ForHowManyDays { get; set; }
}