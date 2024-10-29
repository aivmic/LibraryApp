using LibraryApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Data;

public class LibraryDbContext : DbContext
{
    public DbSet<Book> Books { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>().HasData(
            new Book { Id = 1, Name = "Book1", Year = 2021, Picture = "book1.jpg", Type = BookType.Book },
            new Book { Id = 2, Name = "Book2", Year = 2020, Picture = "book2.jpg", Type = BookType.Book },
            new Book { Id = 3, Name = "Audiobook1", Year = 2019, Picture = "audiobook1.jpg", Type = BookType.Audiobook },
            new Book { Id = 4, Name = "Audiobook2", Year = 2018, Picture = "audiobook2.jpg", Type = BookType.Audiobook }
        );
    }
    
    
    
}



