using DeveloperStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeveloperStore.ORM.Mapping;

/// <summary>
/// Entity Framework configuration for Sale entity.
/// </summary>
public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasColumnType("uuid")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(s => s.SaleNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.SaleDate)
            .IsRequired();

        builder.OwnsOne(s => s.Customer, customer =>
        {
            customer.Property(c => c.Id)
                .HasColumnName("CustomerId")
                .HasColumnType("uuid")
                .IsRequired();

            customer.Property(c => c.Name)
                .HasColumnName("CustomerName")
                .HasMaxLength(200)
                .IsRequired();

            customer.Property(c => c.Email)
                .HasColumnName("CustomerEmail")
                .HasMaxLength(100)
                .IsRequired();

            customer.Property(c => c.Phone)
                .HasColumnName("CustomerPhone")
                .HasMaxLength(20);
        });

        builder.OwnsOne(s => s.Branch, branch =>
        {
            branch.Property(b => b.Id)
                .HasColumnName("BranchId")
                .HasColumnType("uuid")
                .IsRequired();

            branch.Property(b => b.Name)
                .HasColumnName("BranchName")
                .HasMaxLength(200)
                .IsRequired();
        });

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Ignore(s => s.TotalAmount);
        builder.Ignore(s => s.IsCancelled);
        
        // Ignore the public Items property - maping the backing field _items
        builder.Ignore(s => s.Items);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .IsRequired();

        builder.HasMany<SaleItem>("_items")
            .WithOne()
            .HasForeignKey("SaleId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => s.SaleNumber)
            .IsUnique();

        builder.HasIndex(s => s.SaleDate);
    }
}
