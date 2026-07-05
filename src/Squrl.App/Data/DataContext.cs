using Microsoft.EntityFrameworkCore;
using NodaTime;
using Squrl.App.Models;

namespace Squrl.App.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        
    }
    
    public DbSet<Item> Items { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<UnitOfMeasure> UnitOfMeasures { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Enums
        
        // Shadow Properties
        modelBuilder.Entity<Item>()
            .Property<Instant?>("DateCreated");
        
        modelBuilder.Entity<Supplier>()
            .Property<Instant?>("DateCreated");
        
        modelBuilder.Entity<UnitOfMeasure>()
            .Property<Instant?>("DateCreated");

        // Defaults

        // Indexes
        modelBuilder.Entity<Item>()
            .HasIndex(e => new { e.Name  });
        
        modelBuilder.Entity<Item>()
            .HasIndex("DateCreated");
        
        modelBuilder.Entity<Supplier>()
            .HasIndex(e => new { e.Name });
        
        modelBuilder.Entity<Supplier>()
            .HasIndex("DateCreated");

        modelBuilder.Entity<UnitOfMeasure>()
            .HasIndex(e => new { e.Name });
        
        modelBuilder.Entity<UnitOfMeasure>()
            .HasIndex("DateCreated");

        // Relationships
        modelBuilder.Entity<UnitOfMeasure>()
            .HasMany<Item>()
            .WithOne(e => e.Uom)
            .IsRequired();
    }
}