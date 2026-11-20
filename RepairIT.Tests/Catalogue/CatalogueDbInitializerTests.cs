using Microsoft.Extensions.Logging.Abstractions;
using RepairIT.CatalogService.Data;
using RepairIT.Shared.Models;

namespace RepairIT.Tests.Catalogue;

/// <summary>
/// Tests de l'initialisation de la base du catalogue.
/// </summary>
public class CatalogueDbInitializerTests
{
    [Fact]
    public void Initialiser_InsereLesOrdinateursDeDepartSiLaBaseEstVide()
    {
        using var context = TestDbContextFactory.CreerCatalogue();

        CatalogueDbInitializer.Initialiser(context, NullLogger.Instance);

        Assert.Equal(5, context.Ordinateurs.Count());
    }

    [Fact]
    public void Initialiser_NeDupliquePasLesDonneesAuSecondLancement()
    {
        using var context = TestDbContextFactory.CreerCatalogue();

        CatalogueDbInitializer.Initialiser(context, NullLogger.Instance);
        CatalogueDbInitializer.Initialiser(context, NullLogger.Instance);

        Assert.Equal(5, context.Ordinateurs.Count());
    }

    [Fact]
    public void Initialiser_ConserveLesDonneesExistantes()
    {
        using var context = TestDbContextFactory.CreerCatalogue();
        context.Ordinateurs.Add(new Ordinateur { Marque = "Acer", Modele = "Swift", Processeur = "i7" });
        context.SaveChanges();

        CatalogueDbInitializer.Initialiser(context, NullLogger.Instance);

        Assert.Equal("Acer", Assert.Single(context.Ordinateurs).Marque);
    }
}
