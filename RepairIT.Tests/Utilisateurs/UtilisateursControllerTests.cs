using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using RepairIT.Shared.Dtos;
using RepairIT.Shared.Models;
using RepairIT.UserService.Controllers;
using RepairIT.UserService.Services;

namespace RepairIT.Tests.Utilisateurs;

/// <summary>
/// Tests du contrôleur des utilisateurs avec un service simulé (Moq).
/// </summary>
public class UtilisateursControllerTests
{
    private readonly Mock<IUtilisateurService> _service = new();
    private readonly UtilisateursController _controller;

    public UtilisateursControllerTests()
    {
        _controller = new UtilisateursController(_service.Object, NullLogger<UtilisateursController>.Instance);
    }

    private static Utilisateur Utilisateur(int id = 1) => new() { Id = id, Nom = "Dupont", Prenom = "Jean", Email = "jean@repairit.fr" };

    [Fact]
    public async Task Lister_Retourne200AvecLaListe()
    {
        _service.Setup(s => s.ListerAsync()).ReturnsAsync([Utilisateur(1), Utilisateur(2)]);

        var resultat = await _controller.Lister();

        var ok = Assert.IsType<OkObjectResult>(resultat.Result);
        Assert.Equal(2, Assert.IsType<List<Utilisateur>>(ok.Value).Count);
    }

    [Fact]
    public async Task ObtenirParId_Retourne200SiTrouve()
    {
        _service.Setup(s => s.ObtenirParIdAsync(1)).ReturnsAsync(Utilisateur());

        var resultat = await _controller.ObtenirParId(1);

        Assert.IsType<OkObjectResult>(resultat.Result);
    }

    [Fact]
    public async Task ObtenirParId_Retourne404SiInexistant()
    {
        _service.Setup(s => s.ObtenirParIdAsync(It.IsAny<int>())).ReturnsAsync((Utilisateur?)null);

        var resultat = await _controller.ObtenirParId(99);

        Assert.IsType<NotFoundResult>(resultat.Result);
    }

    [Fact]
    public async Task Creer_Retourne201AvecLaRouteDeLaRessource()
    {
        var demande = new UtilisateurCreationDto { Nom = "Dupont", Prenom = "Jean", Email = "jean@repairit.fr" };
        _service.Setup(s => s.CreerAsync(demande)).ReturnsAsync(Utilisateur(3));

        var resultat = await _controller.Creer(demande);

        var cree = Assert.IsType<CreatedAtActionResult>(resultat.Result);
        Assert.Equal(nameof(UtilisateursController.ObtenirParId), cree.ActionName);
        Assert.Equal(3, cree.RouteValues!["id"]);
    }

    [Fact]
    public async Task Creer_Retourne409SiLEmailExisteDeja()
    {
        _service.Setup(s => s.CreerAsync(It.IsAny<UtilisateurCreationDto>())).ReturnsAsync((Utilisateur?)null);

        var resultat = await _controller.Creer(new UtilisateurCreationDto());

        Assert.IsType<ConflictObjectResult>(resultat.Result);
    }
}
