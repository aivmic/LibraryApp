using backend;
using backend.Data;
using backend.Data.Entities;
using backend.Services;
using FluentValidation;
using FluentValidation.Results;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Results;
using SharpGrip.FluentValidation.AutoValidation.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseInMemoryDatabase("LibraryDb"));


builder.Services.AddScoped<ReservationService>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation(configuration =>
{
    configuration.OverrideDefaultResultFactoryWith<ProblemDetailsResultFactory>();
});
builder.Services.AddControllers();

var app = builder.Build();

// Enable serving static files
app.UseStaticFiles();

app.UseCors("AllowAllOrigins");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    SeedDatabase(dbContext);
}

app.AddBookApi();
app.AddReservationApi();

app.Run();

void SeedDatabase(LibraryDbContext dbContext)
{
    if (!dbContext.Books.Any())
    {
        dbContext.Books.AddRange(
            new Book { Id = 1, Name = "The Great Gatsby", Year = 1925, Picture = "/images/book1.jpg", Type = BookType.Book },
            new Book { Id = 2, Name = "Moby-Dick", Year = 1851, Picture = "/images/book2.jpg", Type = BookType.Book },
            new Book { Id = 3, Name = "Frankenstein", Year = 1823, Picture = "/images/audiobook1.jpg", Type = BookType.Audiobook },
            new Book { Id = 4, Name = "Nineteen Eighty-Four", Year = 1949, Picture = "/images/audiobook2.jpg", Type = BookType.Audiobook },
            new Book { Id = 5, Name = "The Road", Year = 2006, Picture = "/images/book3.jpg", Type = BookType.Book },
            new Book { Id = 6, Name = "The Corrections", Year = 2001, Picture = "/images/book4.jpg", Type = BookType.Book },
            new Book { Id = 7, Name = "The Year of Magical Thinking", Year = 2005, Picture = "/images/audiobook3.jpg", Type = BookType.Audiobook },
            new Book { Id = 8, Name = "The Known World", Year = 2003, Picture = "/images/audiobook4.jpg", Type = BookType.Audiobook }
        );
        dbContext.SaveChanges();
    }
}

public class ProblemDetailsResultFactory : IFluentValidationAutoValidationResultFactory
{
    public IResult CreateResult(EndpointFilterInvocationContext context, ValidationResult validationResult)
    {
        var problemDetails = new HttpValidationProblemDetails(validationResult.ToValidationProblemErrors())
        { 
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Unprocessable Entity",
            Status = 422,
        };
        return TypedResults.Problem(problemDetails);
    }
}

public record BookDto(int Id, string Name, int Year, string Picture, BookType Type);
public record ReservationDto(int Id, int BookId, int Days, bool QuickPickUp, decimal TotalCost, DateTime ReservationDate, DateTime ReturnDate);

public record CreateReservationDto(int BookId, int Days, bool QuickPickup, DateTime ReservationDate)
{
    public class CreateReservationDtoValidator : AbstractValidator<CreateReservationDto>
    {
        public CreateReservationDtoValidator()
        {
            RuleFor(x => x.BookId)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("BookId must be a positive integer.");

            RuleFor(x => x.Days)
                .NotEmpty()
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(360)
                .WithMessage("Days must be between 1 and 360.");

            RuleFor(x => x.ReservationDate)
                .NotEmpty()
                .Must(date => date >= DateTime.UtcNow.Date)
                .WithMessage("Reservation date must be today or in the future.");
        }
    }
}




