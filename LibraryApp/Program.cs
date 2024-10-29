using LibraryApp.Data;
using LibraryApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseInMemoryDatabase("LibraryDb"));

var app = builder.Build();

// Seed data on application startup
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

booksGroup.MapGet("/books/{id}", (int id) => {});
booksGroup.MapPost("/books", (CreateBookDto dto) => "POST");
booksGroup.MapPut("/books/{id}", (int id, UpdateBookDto dto) => "PUT");
booksGroup.MapDelete("/books/{id}", (int id) => "DELETE");

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


//     • View list of books in library. Book must include picture, name, year.
//     • Search a list by name, year, type.
//     • On Book click. User can select options and reserve it. Type of book they want: Book or Audiobook;
//       Quick pick up; For how many days.
//     • View my reservations as a list. It should be on a separate page.