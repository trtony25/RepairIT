using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using RepairIT.CatalogService.Controllers;
using RepairIT.CatalogService.Services;
using RepairIT.Shared.Models;

namespace RepairIT.Tests.Catalogue;

/// <summary>
/// Tests du contrôleur du catalogue : le service est simulé avec Moq
/// pour ne vérifier que la traduction des résultats en codes HTTP.
/// </summary>
public class OrdinateursControllerTests
{
    private readonly Mock<IOrdinateurService> _service = new();
    private readonly OrdinateursController _controller;

    public OrdinateursControllerTests()
    {
        _controller = new OrdinateursController(_service.Object, NullLogger<OrdinateursController>.Instance);
    }

    private static Ordinateur Ordinateur(int id = 1) => new() { Id = id, Marque = "Dell", Modele = "Latitude", Processeur = "i5" };

    [Fact]
    public async Task Lister_Retourne200AvecLaListe()
    {
        _service.Setup(s => s.ListerAsync()).ReturnsAsync([Ordinateur(1), Ordinateur(2)]);

        var resultat = await _controller.Lister();

        var ok = Assert.IsType<OkObjectResult>(resultat.Result);
        Assert.Equal(2, Assert.IsType<List<Ordinateur>>(ok.Value).Count);
    }

    [Fact]
    public async Task ObtenirParId_Retourne200SiTrouve()
    {
        _service.Setup(s => s.ObtenirParIdAsync(1)).ReturnsAsync(Ordinateur());

        var resultat = await _controller.ObtenirParId(1);

        var ok = Assert.IsType<OkObjectResult>(resultat.Result);
        Assert.Equal(1, Assert.IsType<Ordinateur>(ok.Value).Id);
    }

    [Fact]
    public async Task ObtenirParId_Retourne404SiInexistant()
    {
        _service.Setup(s => s.ObtenirParIdAsync(It.IsAny<int>())).ReturnsAsync((Ordinateur?)null);

        var resultat = await _controller.ObtenirParId(99);

        Assert.IsType<NotFoundResult>(resultat.Result);
    }

    [Fact]
    public async Task Ajouter_Retourne201AvecLaRouteDeLaRessource()
    {
        var aCreer = Ordinateur(0);
        _service.Setup(s => s.AjouterAsync(aCreer)).ReturnsAsync(Ordinateur(7));

        var resultat = await _controller.Ajouter(aCreer);

        var cree = Assert.IsType<CreatedAtActionResult>(resultat.Result);
        Assert.Equal(nameof(OrdinateursController.ObtenirParId), cree.ActionName);
        Assert.Equal(7, cree.RouteValues!["id"]);
    }

    [Fact]
    public async Task MettreAJour_Retourne204SiModifie()
    {
        _service.Setup(s => s.MettreAJourAsync(1, It.IsAny<Ordinateur>())).ReturnsAsync(true);

        Assert.IsType<NoContentResult>(await _controller.MettreAJour(1, Ordinateur(1)));
    }

    [Fact]
    public async Task MettreAJour_Retourne404SiInexistant()
    {
        _service.Setup(s => s.MettreAJourAsync(It.IsAny<int>(), It.IsAny<Ordinateur>())).ReturnsAsync(false);

        Assert.IsType<NotFoundResult>(await _controller.MettreAJour(5, Ordinateur(0)));
    }

    [Fact]
    public async Task MettreAJour_Retourne400SiLesIdentifiantsDifferent()
    {
        var resultat = await _controller.MettreAJour(1, Ordinateur(2));

        Assert.IsType<BadRequestObjectResult>(resultat);
        _service.Verify(s => s.MettreAJourAsync(It.IsAny<int>(), It.IsAny<Ordinateur>()), Times.Never);
    }

    [Fact]
    public async Task Supprimer_Retourne204SiSupprime()
    {
        _service.Setup(s => s.SupprimerAsync(1)).ReturnsAsync(true);

        Assert.IsType<NoContentResult>(await _controller.Supprimer(1));
    }

    [Fact]
    public async Task Supprimer_Retourne404SiInexistant()
    {
        _service.Setup(s => s.SupprimerAsync(It.IsAny<int>())).ReturnsAsync(false);

        Assert.IsType<NotFoundResult>(await _controller.Supprimer(99));
    }
}
