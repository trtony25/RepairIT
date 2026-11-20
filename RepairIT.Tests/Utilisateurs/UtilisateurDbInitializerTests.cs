using Microsoft.Extensions.Logging.Abstractions;
using RepairIT.Shared.Models;
using RepairIT.UserService.Data;

namespace RepairIT.Tests.Utilisateurs;

/// <summary>
/// Tests de l'initialisation de la base des utilisateurs.
/// </summary>
public class UtilisateurDbInitializerTests
{
    [Fact]
    public void Initialiser_CreeUnAdministrateurEtQuatreGestionnaires()
    {
        using var context = TestDbContextFactory.CreerUtilisateurs();

        UtilisateurDbInitializer.Initialiser(context, NullLogger.Instance);

        Assert.Equal(1, context.Utilisateurs.Count(u => u.Role == RoleUtilisateur.Administrateur));
        Assert.Equal(4, context.Utilisateurs.Count(u => u.Role == RoleUtilisateur.Gestionnaire));
        Assert.All(context.Utilisateurs, u => Assert.Equal(Utilisateur.UserKeycloakParDefaut, u.UserKeycloak));
    }

    [Fact]
    public void Initialiser_NeDupliquePasLesDonneesAuSecondLancement()
    {
        using var context = TestDbContextFactory.CreerUtilisateurs();

        UtilisateurDbInitializer.Initialiser(context, NullLogger.Instance);
        UtilisateurDbInitializer.Initialiser(context, NullLogger.Instance);

        Assert.Equal(5, context.Utilisateurs.Count());
    }
}
