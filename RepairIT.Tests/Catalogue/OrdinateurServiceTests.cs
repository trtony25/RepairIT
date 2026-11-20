using Microsoft.Extensions.Logging.Abstractions;
using RepairIT.CatalogService.Data;
using RepairIT.CatalogService.Services;
using RepairIT.Shared.Models;

namespace RepairIT.Tests.Catalogue;

/// <summary>
/// Tests du service métier du catalogue, exécutés sur une base EF Core en mémoire.
/// </summary>
public class OrdinateurServiceTests
{
    private readonly CatalogueContext _context = TestDbContextFactory.CreerCatalogue();
    private readonly OrdinateurService _service;

    public OrdinateurServiceTests()
    {
        _service = new OrdinateurService(_context, NullLogger<OrdinateurService>.Instance);
    }

    private static Ordinateur NouvelOrdinateur(string marque = "Dell", string modele = "Latitude") => new()
    {
        Marque = marque,
        Modele = modele,
        Processeur = "Intel Core i5",
        RamGo = 16,
        StockageGo = 512,
        Type = TypeOrdinateur.Portable,
        Prix = 999.99m,
        QuantiteStock = 4
    };

    [Fact]
    public async Task AjouterAsync_EnregistreLOrdinateurEtAttribueUnId()
    {
        var cree = await _service.AjouterAsync(NouvelOrdinateur());

        Assert.True(cree.Id > 0);
        Assert.Equal(1, _context.Ordinateurs.Count());
    }

    [Fact]
    public async Task AjouterAsync_IgnoreLIdFourniParLAppelant()
    {
        var existant = await _service.AjouterAsync(NouvelOrdinateur());
        var doublon = NouvelOrdinateur("HP", "EliteBook");
        doublon.Id = existant.Id;

        var cree = await _service.AjouterAsync(doublon);

        Assert.NotEqual(existant.Id, cree.Id);
        Assert.Equal(2, _context.Ordinateurs.Count());
    }

    [Fact]
    public async Task ListerAsync_RetourneLesOrdinateursTriesParMarquePuisModele()
    {
        await _service.AjouterAsync(NouvelOrdinateur("Lenovo", "ThinkPad"));
        await _service.AjouterAsync(NouvelOrdinateur("Dell", "XPS"));
        await _service.AjouterAsync(NouvelOrdinateur("Dell", "Latitude"));

        var liste = await _service.ListerAsync();

        Assert.Equal(["Latitude", "XPS", "ThinkPad"], liste.Select(o => o.Modele));
    }

    [Fact]
    public async Task ObtenirParIdAsync_RetourneLOrdinateurExistant()
    {
        var cree = await _service.AjouterAsync(NouvelOrdinateur());

        var trouve = await _service.ObtenirParIdAsync(cree.Id);

        Assert.NotNull(trouve);
        Assert.Equal("Dell", trouve.Marque);
    }

    [Fact]
    public async Task ObtenirParIdAsync_RetourneNullSiInexistant()
    {
        Assert.Null(await _service.ObtenirParIdAsync(42));
    }

    [Fact]
    public async Task MettreAJourAsync_ModifieToutesLesInformations()
    {
        var cree = await _service.AjouterAsync(NouvelOrdinateur());
        var modifications = new Ordinateur
        {
            Marque = "HP",
            Modele = "ProBook",
            Processeur = "AMD Ryzen 5",
            RamGo = 32,
            StockageGo = 1024,
            Type = TypeOrdinateur.Fixe,
            Prix = 1200m,
            QuantiteStock = 0
        };

        var resultat = await _service.MettreAJourAsync(cree.Id, modifications);

        Assert.True(resultat);
        var enBase = await _service.ObtenirParIdAsync(cree.Id);
        Assert.NotNull(enBase);
        Assert.Equal("HP", enBase.Marque);
        Assert.Equal("ProBook", enBase.Modele);
        Assert.Equal("AMD Ryzen 5", enBase.Processeur);
        Assert.Equal(32, enBase.RamGo);
        Assert.Equal(1024, enBase.StockageGo);
        Assert.Equal(TypeOrdinateur.Fixe, enBase.Type);
        Assert.Equal(1200m, enBase.Prix);
        Assert.Equal(0, enBase.QuantiteStock);
    }

    [Fact]
    public async Task MettreAJourAsync_RetourneFalseSiInexistant()
    {
        Assert.False(await _service.MettreAJourAsync(42, NouvelOrdinateur()));
    }

    [Fact]
    public async Task SupprimerAsync_RetireLOrdinateur()
    {
        var cree = await _service.AjouterAsync(NouvelOrdinateur());

        var resultat = await _service.SupprimerAsync(cree.Id);

        Assert.True(resultat);
        Assert.Empty(_context.Ordinateurs);
    }

    [Fact]
    public async Task SupprimerAsync_RetourneFalseSiInexistant()
    {
        Assert.False(await _service.SupprimerAsync(42));
    }
}
