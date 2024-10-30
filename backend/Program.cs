using backend.Data;
using backend.Data.Entities;
using backend.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy.WithOrigins("http://localhost:3000") // Replace with your React app's URL if needed
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseInMemoryDatabase("LibraryDb"));

builder.Services.AddScoped<ReservationService>();

var app = builder.Build();

app.UseCors("AllowReactApp");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    SeedDatabase(dbContext);
}

var booksGroup = app.MapGroup("/api");

booksGroup.MapGet("/books", async (LibraryDbContext dbContext, string? search = null, string? type = null) =>
{
    var query = dbContext.Books.AsQueryable();
    
    if (!string.IsNullOrWhiteSpace(search))
    {
            query = query.Where(b => b.Year.ToString().Contains(search) || b.Name.Contains(search) || b.Type.ToString().Contains(search)) ;
    }
    
    if (!string.IsNullOrEmpty(type) && Enum.TryParse(type, true, out BookType explicitType))
    {
        query = query.Where(b => b.Type == explicitType);
    }

    var books = await query.Select(b => b.ToDto()).ToListAsync();
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
    var book = await dbContext.Books.FindAsync(request.BookId);
    if (book == null)
    {
        return Results.NotFound("Book not found.");
    }
    decimal totalCost = reservationService.CalculateCost(book.Type, request.Days, request.QuickPickup);
    
    var reservation = new Reservation
    {
        BookId = request.BookId,
        Days = request.Days,
        QuickPickup = request.QuickPickup,
        TotalCost = totalCost,
        ReturnDate = DateTime.UtcNow.AddDays(request.Days),
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
            new Book { Id = 1, Name = "The Great Gatsby", Year = 1925, Picture = "book1.jpg", Type = BookType.Book },
            new Book { Id = 2, Name = "Moby-Dick", Year = 1851, Picture = "book2.jpg", Type = BookType.Book },
            new Book { Id = 3, Name = "Frankenstein", Year = 1823, Picture = "audiobook1.jpg", Type = BookType.Audiobook },
            new Book { Id = 4, Name = "Nineteen Eighty-Four", Year = 1949, Picture = "audiobook2.jpg", Type = BookType.Audiobook },
            new Book { Id = 5, Name = "The Road", Year = 2006, Picture = "book3.jpg", Type = BookType.Book },
            new Book { Id = 6, Name = "The Corrections", Year = 2001, Picture = "book4.jpg", Type = BookType.Book },
            new Book { Id = 7, Name = "The Year of Magical Thinking", Year = 2005, Picture = "audiobook3.jpg", Type = BookType.Audiobook },
            new Book { Id = 8, Name = "The Known World", Year = 2003, Picture = "audiobook4.jpg", Type = BookType.Audiobook },
            new Book { Id = 9, Name = "The Great Gatsby", Year = 1925, Picture = "book1.jpg", Type = BookType.Audiobook },
            new Book { Id = 10, Name = "Moby-Dick", Year = 1851, Picture = "book2.jpg", Type = BookType.Audiobook },
            new Book { Id = 11, Name = "Frankenstein", Year = 1823, Picture = "audiobook1.jpg", Type = BookType.Book },
            new Book { Id = 12, Name = "Nineteen Eighty-Four", Year = 1949, Picture = "audiobook2.jpg", Type = BookType.Book }
        );
        dbContext.SaveChanges();
    }

    if (!dbContext.Reservations.Any())
    {
        Reservation newReservation = new Reservation
        {
            Id = 1,
            BookId = 5,
            Days = 5,
            QuickPickup = true,
            TotalCost = 10, 
            ReturnDate = DateTime.UtcNow.AddDays(5)
        };
    }
}

public record BookDto(int Id, string Name, int Year, string Picture, BookType Type);

public record CreateBookDto(string Name, int Year, string Picture);

public record UpdateBookDto(string Name, int Year, string Picture);

public record ReservationDto(int Id, int BookId, int Days, bool QuickPickUp, decimal TotalCost, DateTime ReservationDate, DateTime ReturnDate);
public record CreateReservationDto(int BookId, int Days, bool QuickPickUp);

public record ReservationRequestDto(int BookId, int Days, bool QuickPickup);
