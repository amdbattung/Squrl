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
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    public DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Shadow Properties
        modelBuilder.Entity<Item>()
            .Property<Instant?>("DateCreated");
        
        modelBuilder.Entity<Supplier>()
            .Property<Instant?>("DateCreated");
        
        modelBuilder.Entity<UnitOfMeasure>()
            .Property<Instant?>("DateCreated");
        
        modelBuilder.Entity<PurchaseOrder>()
            .Property<Instant?>("DateCreated");
        
        modelBuilder.Entity<PurchaseOrderDetail>()
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
            .HasIndex(e => new { e.Code });
        
        modelBuilder.Entity<UnitOfMeasure>()
            .HasIndex("DateCreated");
        
        modelBuilder.Entity<PurchaseOrder>()
            .HasIndex("DateCreated");
        
        modelBuilder.Entity<PurchaseOrderDetail>()
            .HasIndex("PurchaseOrderId", nameof(PurchaseOrderDetail.LineSequence))
            .IsUnique();
        
        modelBuilder.Entity<PurchaseOrderDetail>()
            .HasIndex("DateCreated");

        // Relationships
        modelBuilder.Entity<Item>()
            .HasOne(e => e.Uom)
            .WithMany()
            .IsRequired();
        
        modelBuilder.Entity<PurchaseOrder>()
            .HasOne(e => e.Supplier)
            .WithMany();

        modelBuilder.Entity<PurchaseOrderDetail>()
            .HasOne(e => e.PurchaseOrder)
            .WithMany()
            .IsRequired();

        modelBuilder.Entity<PurchaseOrderDetail>()
            .HasOne(e => e.Item)
            .WithMany()
            .IsRequired();
        
        // Constraints
        modelBuilder.Entity<PurchaseOrderDetail>()
            .ToTable("purchase_order_details", t =>
            {
                t.HasCheckConstraint(
                    "CK_purchase_order_details_line_sequence",
                    "line_sequence >= 1");
            });
    }
}