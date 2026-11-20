using Microsoft.EntityFrameworkCore;
using RepairIT.Shared.Models;

namespace RepairIT.CatalogService.Data;

/// <summary>
/// Contexte EF Core du micro-service Catalogue : il ne connaît que les ordinateurs,
/// chaque micro-service ayant sa propre base de données.
/// </summary>
public class CatalogueContext : DbContext
{
    public CatalogueContext(DbContextOptions<CatalogueContext> options) : base(options)
    {
    }

    public DbSet<Ordinateur> Ordinateurs => Set<Ordinateur>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ordinateur>(ordinateur =>
        {
            ordinateur.ToTable("Ordinateurs");

            // Le type est stocké en texte pour rester lisible directement dans la base
            ordinateur.Property(o => o.Type).HasConversion<string>().HasMaxLength(20);

            // Précision explicite du prix (utile dès le passage à PostgreSQL en V2)
            ordinateur.Property(o => o.Prix).HasPrecision(10, 2);
        });
    }
}
