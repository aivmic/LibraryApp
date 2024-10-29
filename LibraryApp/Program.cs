var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var booksGroup = app.MapGroup("/api");

booksGroup.MapGet("/books", () => "GET ALL");
booksGroup.MapGet("/books/{id}", (int id) => {});
booksGroup.MapPost("/books", (CreateBookDto dto) => "POST");
booksGroup.MapPut("/books/{id}", (int id, UpdateBookDto dto) => "PUT");
booksGroup.MapDelete("/books/{id}", (int id) => "DELETE");

app.Run();

public record BookDto(int Id, string Name, int Year, string Picture);

public record CreateBookDto(string Name, int Year, string Picture);

public record UpdateBookDto(string Name, int Year, string Picture);

//     • View list of books in library. Book must include picture, name, year.
//     • Search a list by name, year, type.
//     • On Book click. User can select options and reserve it. Type of book they want: Book or Audiobook;
//       Quick pick up; For how many days.
//     • View my reservations as a list. It should be on a separate page.