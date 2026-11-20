using Microsoft.AspNetCore.Mvc;
using RepairIT.Shared.Dtos;
using RepairIT.Shared.Models;
using RepairIT.UserService.Services;

namespace RepairIT.UserService.Controllers;

/// <summary>
/// Points d'entrée REST de la gestion des utilisateurs.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UtilisateursController : ControllerBase
{
    private readonly IUtilisateurService _utilisateurService;
    private readonly ILogger<UtilisateursController> _logger;

    public UtilisateursController(IUtilisateurService utilisateurService, ILogger<UtilisateursController> logger)
    {
        _utilisateurService = utilisateurService;
        _logger = logger;
    }

    /// <summary>GET api/utilisateurs : liste des utilisateurs.</summary>
    [HttpGet]
    public async Task<ActionResult<List<Utilisateur>>> Lister()
    {
        _logger.LogInformation("Consultation de la liste des utilisateurs");
        return Ok(await _utilisateurService.ListerAsync());
    }

    /// <summary>GET api/utilisateurs/{id} : détail d'un utilisateur (404 s'il n'existe pas).</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Utilisateur>> ObtenirParId(int id)
    {
        _logger.LogInformation("Consultation de l'utilisateur {Id}", id);
        var utilisateur = await _utilisateurService.ObtenirParIdAsync(id);
        return utilisateur is null ? NotFound() : Ok(utilisateur);
    }

    /// <summary>POST api/utilisateurs : création d'un utilisateur (409 si l'e-mail existe déjà).</summary>
    [HttpPost]
    public async Task<ActionResult<Utilisateur>> Creer(UtilisateurCreationDto demande)
    {
        _logger.LogInformation("Demande de création d'un utilisateur ({Role})", demande.Role);
        var cree = await _utilisateurService.CreerAsync(demande);

        if (cree is null)
        {
            return Conflict("Un utilisateur utilise déjà cette adresse e-mail.");
        }

        return CreatedAtAction(nameof(ObtenirParId), new { id = cree.Id }, cree);
    }
}
