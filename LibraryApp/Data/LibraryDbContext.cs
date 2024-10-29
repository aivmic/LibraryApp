using LibraryApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Data;

public class LibraryDbContext : DbContext
{
    public DbSet<Book> Books { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }
}



