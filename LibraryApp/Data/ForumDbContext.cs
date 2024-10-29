using LibraryApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Data;

public class ForumDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    public DbSet<Book> Books { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    
    public ForumDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("MyDatabase");
    }
}


