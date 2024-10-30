using backend.Data;
using backend.Data.Entities;
using backend.Services;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace backend;

public static class Endpoints
{
    public static void AddBookApi(this WebApplication app)
    {
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
    }

    public static void AddReservationApi(this WebApplication app)
    {
        var reservationsGroup = app.MapGroup("/api").AddFluentValidationAutoValidation();

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

        reservationsGroup.MapPost("/reservations", async (CreateReservationDto request, LibraryDbContext dbContext, ReservationService reservationService) =>
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

            return Results.Created($"/reservations/{reservation.Id}", reservation);
        }); 
    }
    
}