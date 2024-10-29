using backend.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class LibraryDbContext : DbContext
{
    public DbSet<Book> Books { get; set; } = null!;
    public DbSet<Reservation> Reservations { get; set; } = null!;
    
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }
}