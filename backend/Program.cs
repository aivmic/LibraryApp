using backend.Data;
using backend.Data.Entities;
using backend.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseInMemoryDatabase("LibraryDb"));

builder.Services.AddScoped<ReservationService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    SeedDatabase(dbContext);
}

var booksGroup = app.MapGroup("/api");

booksGroup.MapGet("/books", async (LibraryDbContext dbContext) =>
{
    var books = await dbContext.Books.Select(b => b.ToDto()).ToListAsync();
    return Results.Ok(books);
});

booksGroup.MapGet("/books/{id}", async(int id, LibraryDbContext dbContext) =>
{
    var book = await dbContext.Books.FindAsync(id);
    return book == null ? Results.NotFound() : TypedResults.Ok(book.ToDto());
});

var reservationsGroup = app.MapGroup("/api");

reservationsGroup.MapGet("/reservations", async (LibraryDbContext dbContext) =>
{
    var reservations = await dbContext.Reservations.Select(r => r.ToDto()).ToListAsync();
    return Results.Ok(reservations);
});

reservationsGroup.MapGet("/reservations/{id}", async(int id, LibraryDbContext dbContext) =>
{
    var reservation = await dbContext.Reservations.FindAsync(id);
    return reservation == null ? Results.NotFound() : TypedResults.Ok(reservation.ToDto());
});

app.MapPost("/api/reservations", async (ReservationRequestDto request, LibraryDbContext dbContext, ReservationService reservationService) =>
{
    // Find the book by ID
    var book = await dbContext.Books.FindAsync(request.BookId);
    if (book == null)
    {
        return Results.NotFound("Book not found.");
    }

    // Calculate the cost of the reservation
    decimal totalCost = reservationService.CalculateCost(book.Type, request.Days, request.QuickPickup);

    // Create and save the reservation
    var reservation = new Reservation
    {
        BookId = request.BookId,
        Days = request.Days,
        QuickPickup = request.QuickPickup,
        TotalCost = totalCost,
        Book = book
    };

    dbContext.Reservations.Add(reservation);
    await dbContext.SaveChangesAsync();

    return Results.Created($"/api/reservations/{reservation.Id}", reservation);
});

app.Run();

void SeedDatabase(LibraryDbContext dbContext)
{
    if (!dbContext.Books.Any())
    {
        dbContext.Books.AddRange(
            new Book { Id = 1, Name = "Book1", Year = 2021, Picture = "book1.jpg", Type = BookType.Book },
            new Book { Id = 2, Name = "Book2", Year = 2020, Picture = "book2.jpg", Type = BookType.Book },
            new Book { Id = 3, Name = "Audiobook1", Year = 2019, Picture = "audiobook1.jpg", Type = BookType.Audiobook },
            new Book { Id = 4, Name = "Audiobook2", Year = 2018, Picture = "audiobook2.jpg", Type = BookType.Audiobook }
        );
        dbContext.SaveChanges();
    }
}

public record BookDto(int Id, string Name, int Year, string Picture, BookType Type);

public record CreateBookDto(string Name, int Year, string Picture);

public record UpdateBookDto(string Name, int Year, string Picture);

public record ReservationDto(int Id, int BookId, int Days, bool QuickPickUp, decimal TotalCost, DateTime ReservationDate);
public record CreateReservationDto(int BookId, int Days, bool QuickPickUp);

public record ReservationRequestDto(int BookId, int Days, bool QuickPickup);


//     • View list of books in library. Book must include picture, name, year.
//     • Search a list by name, year, type.
//     • On Book click. User can select options and reserve it. Type of book they want: Book or Audiobook;
//       Quick pick up; For how many days.
//     • View my reservations as a list. It should be on a separate page.