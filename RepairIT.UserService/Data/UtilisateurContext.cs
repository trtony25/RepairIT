using Microsoft.EntityFrameworkCore;
using RepairIT.Shared.Models;

namespace RepairIT.UserService.Data;

/// <summary>
/// Contexte EF Core du micro-service Utilisateurs (base de données dédiée).
/// </summary>
public class UtilisateurContext : DbContext
{
    public UtilisateurContext(DbContextOptions<UtilisateurContext> options) : base(options)
    {
    }

    public DbSet<Utilisateur> Utilisateurs => Set<Utilisateur>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Utilisateur>(utilisateur =>
        {
            utilisateur.ToTable("Utilisateurs");

            // Deux comptes ne peuvent pas partager la même adresse e-mail
            utilisateur.HasIndex(u => u.Email).IsUnique();

            // Le rôle est stocké en texte ("Gestionnaire", "Administrateur"), comme dans Keycloak
            utilisateur.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);
        });
    }
}
