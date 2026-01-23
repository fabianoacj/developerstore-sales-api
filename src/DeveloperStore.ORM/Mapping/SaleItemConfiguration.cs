using DeveloperStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeveloperStore.ORM.Mapping;

/// <summary>
/// Entity Framework configuration for SaleItem entity.
/// </summary>
public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");

        builder.HasKey(si => si.Id);
        builder.Property(si => si.Id)
            .HasColumnType("uuid")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property<Guid>("SaleId")
            .HasColumnType("uuid")
            .IsRequired();

        builder.OwnsOne(si => si.Product, product =>
        {
            product.Property(p => p.Id)
                .HasColumnName("ProductId")
                .HasColumnType("uuid")
                .IsRequired();

            product.Property(p => p.Title)
                .HasColumnName("ProductTitle")
                .HasMaxLength(200)
                .IsRequired();

            product.Property(p => p.Category)
                .HasColumnName("ProductCategory")
                .HasMaxLength(100)
                .IsRequired();

            product.Property(p => p.Description)
                .HasColumnName("ProductDescription")
                .HasMaxLength(500);
        });

        builder.Property(si => si.Quantity)
            .IsRequired();

        builder.Property(si => si.UnitPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(si => si.Discount)
            .HasColumnType("decimal(5,2)")
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(si => si.IsCancelled)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Ignore(si => si.TotalAmount);

        builder.Property(si => si.CreatedAt)
            .IsRequired();

        builder.Property(si => si.UpdatedAt)
            .IsRequired();

        builder.HasIndex("SaleId");
    }
}
