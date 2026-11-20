using Microsoft.EntityFrameworkCore;
using RepairIT.CatalogService.Data;
using RepairIT.UserService.Data;

namespace RepairIT.Tests;

/// <summary>
/// Fabrique de contextes EF Core en mémoire : chaque test reçoit une base vide et isolée
/// (nom de base unique), ce qui évite toute dépendance entre les tests.
/// </summary>
internal static class TestDbContextFactory
{
    public static CatalogueContext CreerCatalogue() =>
        new(new DbContextOptionsBuilder<CatalogueContext>()
            .UseInMemoryDatabase($"catalogue-{Guid.NewGuid()}")
            .Options);

    public static UtilisateurContext CreerUtilisateurs() =>
        new(new DbContextOptionsBuilder<UtilisateurContext>()
            .UseInMemoryDatabase($"utilisateurs-{Guid.NewGuid()}")
            .Options);
}
