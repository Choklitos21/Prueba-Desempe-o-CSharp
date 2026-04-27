using Microsoft.EntityFrameworkCore;
using PruebaDsesempeño.Models;

namespace PruebaDsesempeño.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {
        
    }
    
    public DbSet<User> User { get; set; }
    public DbSet<Space> Space { get; set; }
    public DbSet<Reservation> Reservation { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Space>()
            .HasMany(s => s.Reservation)
            .WithOne(r => r.Space)
            .HasForeignKey(r => r.SpaceId);
        
        modelBuilder.Entity<Reservation>()
            .HasMany(r => r.User)
            .WithMany(u => u.Reservation);
    }
}