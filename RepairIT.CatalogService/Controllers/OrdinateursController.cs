using Microsoft.AspNetCore.Mvc;
using RepairIT.CatalogService.Services;
using RepairIT.Shared.Models;

namespace RepairIT.CatalogService.Controllers;

/// <summary>
/// Points d'entrée REST du catalogue : CRUD complet sur les ordinateurs.
/// [ApiController] valide automatiquement le corps des requêtes (400 si invalide).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdinateursController : ControllerBase
{
    private readonly IOrdinateurService _ordinateurService;
    private readonly ILogger<OrdinateursController> _logger;

    public OrdinateursController(IOrdinateurService ordinateurService, ILogger<OrdinateursController> logger)
    {
        _ordinateurService = ordinateurService;
        _logger = logger;
    }

    /// <summary>GET api/ordinateurs : liste complète du catalogue.</summary>
    [HttpGet]
    public async Task<ActionResult<List<Ordinateur>>> Lister()
    {
        _logger.LogInformation("Consultation de la liste des ordinateurs");
        return Ok(await _ordinateurService.ListerAsync());
    }

    /// <summary>GET api/ordinateurs/{id} : détail d'un ordinateur (404 s'il n'existe pas).</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Ordinateur>> ObtenirParId(int id)
    {
        _logger.LogInformation("Consultation de l'ordinateur {Id}", id);
        var ordinateur = await _ordinateurService.ObtenirParIdAsync(id);
        return ordinateur is null ? NotFound() : Ok(ordinateur);
    }

    /// <summary>POST api/ordinateurs : ajout au catalogue (201 avec l'URL de la ressource créée).</summary>
    [HttpPost]
    public async Task<ActionResult<Ordinateur>> Ajouter(Ordinateur ordinateur)
    {
        _logger.LogInformation("Demande d'ajout d'un ordinateur {Marque} {Modele}", ordinateur.Marque, ordinateur.Modele);
        var cree = await _ordinateurService.AjouterAsync(ordinateur);
        return CreatedAtAction(nameof(ObtenirParId), new { id = cree.Id }, cree);
    }

    /// <summary>PUT api/ordinateurs/{id} : remplacement des informations d'un ordinateur.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> MettreAJour(int id, Ordinateur ordinateur)
    {
        _logger.LogInformation("Demande de mise à jour de l'ordinateur {Id}", id);

        // Un identifiant dans le corps qui contredit l'URL est une erreur de l'appelant
        if (ordinateur.Id != 0 && ordinateur.Id != id)
        {
            return BadRequest("L'identifiant du corps ne correspond pas à celui de l'URL.");
        }

        return await _ordinateurService.MettreAJourAsync(id, ordinateur) ? NoContent() : NotFound();
    }

    /// <summary>DELETE api/ordinateurs/{id} : retrait d'un ordinateur du catalogue.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Supprimer(int id)
    {
        _logger.LogInformation("Demande de suppression de l'ordinateur {Id}", id);
        return await _ordinateurService.SupprimerAsync(id) ? NoContent() : NotFound();
    }
}
