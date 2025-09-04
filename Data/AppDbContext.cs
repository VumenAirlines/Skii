using Microsoft.EntityFrameworkCore;
using Skii.Models;

namespace Skii.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options):DbContext(options)
{
    public DbSet<Answer> Answers { get; set; }
    public DbSet<DateSelection> DateSelections { get; set; }
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Answer>()
            .HasMany(a => a.AvailableDates)
            .WithOne(d => d.Answer)
            .HasForeignKey(d => d.AnswerId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<DateSelection>()
            .Property(d => d.Date)
            .HasConversion(
                v => v.ToDateTime(TimeOnly.MinValue),
                v => DateOnly.FromDateTime(v));    
       
        modelBuilder.Entity<Answer>()
            .HasIndex(a => a.UserId);
        modelBuilder.Entity<DateSelection>()
            .HasIndex(d => d.AnswerId);
       
        modelBuilder.Entity<User>()
            .HasIndex(u => u.GoogleId)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}