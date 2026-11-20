using Microsoft.Extensions.Logging.Abstractions;
using RepairIT.Shared.Dtos;
using RepairIT.Shared.Models;
using RepairIT.UserService.Data;
using RepairIT.UserService.Services;

namespace RepairIT.Tests.Utilisateurs;

/// <summary>
/// Tests du service de gestion des utilisateurs, sur une base EF Core en mémoire.
/// </summary>
public class UtilisateurServiceTests
{
    private readonly UtilisateurContext _context = TestDbContextFactory.CreerUtilisateurs();
    private readonly UtilisateurService _service;

    public UtilisateurServiceTests()
    {
        _service = new UtilisateurService(_context, NullLogger<UtilisateurService>.Instance);
    }

    private static UtilisateurCreationDto Demande(string email = "jean.dupont@repairit.fr", RoleUtilisateur role = RoleUtilisateur.Gestionnaire) => new()
    {
        Nom = "Dupont",
        Prenom = "Jean",
        Email = email,
        Role = role
    };

    [Fact]
    public async Task CreerAsync_CreeUnCompteActifAvecLeLienKeycloakParDefaut()
    {
        var cree = await _service.CreerAsync(Demande());

        Assert.NotNull(cree);
        Assert.True(cree.Id > 0);
        Assert.True(cree.Actif);
        Assert.Equal(Utilisateur.UserKeycloakParDefaut, cree.UserKeycloak);
    }

    [Fact]
    public async Task CreerAsync_ConserveLeRoleDemande()
    {
        var cree = await _service.CreerAsync(Demande(role: RoleUtilisateur.Administrateur));

        Assert.Equal(RoleUtilisateur.Administrateur, cree!.Role);
    }

    [Fact]
    public async Task CreerAsync_NormaliseLesSaisies()
    {
        var demande = new UtilisateurCreationDto { Nom = "  Martin ", Prenom = " Léa ", Email = "  Lea.Martin@RepairIT.fr " };

        var cree = await _service.CreerAsync(demande);

        Assert.Equal("Martin", cree!.Nom);
        Assert.Equal("Léa", cree.Prenom);
        Assert.Equal("lea.martin@repairit.fr", cree.Email);
    }

    [Fact]
    public async Task CreerAsync_RefuseUnEmailDejaUtilise()
    {
        await _service.CreerAsync(Demande("jean.dupont@repairit.fr"));

        // Même adresse, casse différente : doit être détectée comme doublon
        var doublon = await _service.CreerAsync(Demande("JEAN.DUPONT@repairit.fr"));

        Assert.Null(doublon);
        Assert.Equal(1, _context.Utilisateurs.Count());
    }

    [Fact]
    public async Task ListerAsync_RetourneLesUtilisateursTriesParNomPuisPrenom()
    {
        await _service.CreerAsync(new UtilisateurCreationDto { Nom = "Martin", Prenom = "Léa", Email = "lea@repairit.fr" });
        await _service.CreerAsync(new UtilisateurCreationDto { Nom = "Dupont", Prenom = "Paul", Email = "paul@repairit.fr" });
        await _service.CreerAsync(new UtilisateurCreationDto { Nom = "Dupont", Prenom = "Anne", Email = "anne@repairit.fr" });

        var liste = await _service.ListerAsync();

        Assert.Equal(["Anne", "Paul", "Léa"], liste.Select(u => u.Prenom));
    }

    [Fact]
    public async Task ObtenirParIdAsync_RetourneLUtilisateurExistant()
    {
        var cree = await _service.CreerAsync(Demande());

        var trouve = await _service.ObtenirParIdAsync(cree!.Id);

        Assert.NotNull(trouve);
        Assert.Equal("Dupont", trouve.Nom);
    }

    [Fact]
    public async Task ObtenirParIdAsync_RetourneNullSiInexistant()
    {
        Assert.Null(await _service.ObtenirParIdAsync(42));
    }
}
