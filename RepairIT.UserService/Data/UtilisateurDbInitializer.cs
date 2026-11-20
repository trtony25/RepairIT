using Microsoft.EntityFrameworkCore;
using RepairIT.Shared.Models;

namespace RepairIT.UserService.Data;

/// <summary>
/// Initialisation de la base des utilisateurs : migrations puis comptes de départ si la table est vide.
/// Les comptes de départ reprennent la répartition demandée pour Keycloak (1 administrateur, 4 gestionnaires) ;
/// leur lien Keycloak garde la valeur par défaut tant que Keycloak n'est pas en place.
/// </summary>
public static class UtilisateurDbInitializer
{
    public static void Initialiser(UtilisateurContext context, ILogger logger)
    {
        // Les migrations ne s'appliquent qu'à une vraie base (SQLite) ; InMemory (tests) crée simplement le modèle.
        if (context.Database.IsRelational())
        {
            context.Database.Migrate();
        }
        else
        {
            context.Database.EnsureCreated();
        }

        if (context.Utilisateurs.Any())
        {
            logger.LogInformation("Base utilisateurs déjà initialisée ({Nombre} comptes)", context.Utilisateurs.Count());
            return;
        }

        context.Utilisateurs.AddRange(UtilisateursInitiaux());
        context.SaveChanges();
        logger.LogInformation("Base utilisateurs initialisée avec les comptes de départ");
    }

    private static IEnumerable<Utilisateur> UtilisateursInitiaux() =>
    [
        new() { Nom = "Admin", Prenom = "RepairIT", Email = "admin@repairit.fr", Role = RoleUtilisateur.Administrateur },
        new() { Nom = "Gestionnaire", Prenom = "Un", Email = "gestionnaire1@repairit.fr" },
        new() { Nom = "Gestionnaire", Prenom = "Deux", Email = "gestionnaire2@repairit.fr" },
        new() { Nom = "Gestionnaire", Prenom = "Trois", Email = "gestionnaire3@repairit.fr" },
        new() { Nom = "Gestionnaire", Prenom = "Quatre", Email = "gestionnaire4@repairit.fr" }
    ];
}
