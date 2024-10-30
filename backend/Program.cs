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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy.WithOrigins("http://localhost:3000") // React app URL
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseInMemoryDatabase("LibraryDb"));

builder.Services.AddScoped<ReservationService>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation(configuration =>
{
    configuration.OverrideDefaultResultFactoryWith<ProblemDetailsResultFactory>();
});

var app = builder.Build();

app.UseCors("AllowReactApp");

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
public record CreateReservationDto(int BookId, int Days, bool QuickPickup)
{
    public class CreateReservationDtoValidator : AbstractValidator<CreateReservationDto>
    {
        public CreateReservationDtoValidator()
        {
            RuleFor(x => x.BookId)
                .NotEmpty()
                .GreaterThan(0);
                
            RuleFor(x => x.Days)
                .NotEmpty()
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(360)
                .WithMessage("Days must be between 1 and 360.");
        }
    }
}


