using DeveloperStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DeveloperStore.ORM;

/// <summary>
/// Main database context for DeveloperStore application.
/// </summary>
public class DefaultContext : DbContext
{
    /// <summary>
    /// DbSet for Sale entities.
    /// </summary>
    public DbSet<Sale> Sales { get; set; }

    /// <summary>
    /// DbSet for SaleItem entities.
    /// </summary>
    public DbSet<SaleItem> SaleItems { get; set; }

    public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
