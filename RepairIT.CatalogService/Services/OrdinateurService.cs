using Microsoft.EntityFrameworkCore;
using RepairIT.CatalogService.Data;
using RepairIT.Shared.Models;

namespace RepairIT.CatalogService.Services;

/// <summary>
/// Implémentation EF Core du catalogue. Enregistrée en Scoped car elle dépend du DbContext,
/// lui-même créé pour chaque requête HTTP.
/// </summary>
public class OrdinateurService : IOrdinateurService
{
    private readonly CatalogueContext _context;
    private readonly ILogger<OrdinateurService> _logger;

    public OrdinateurService(CatalogueContext context, ILogger<OrdinateurService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Ordinateur>> ListerAsync()
    {
        // AsNoTracking : lecture seule, inutile de suivre les entités
        return await _context.Ordinateurs
            .AsNoTracking()
            .OrderBy(o => o.Marque)
            .ThenBy(o => o.Modele)
            .ToListAsync();
    }

    public async Task<Ordinateur?> ObtenirParIdAsync(int id)
    {
        return await _context.Ordinateurs.FindAsync(id);
    }

    public async Task<Ordinateur> AjouterAsync(Ordinateur ordinateur)
    {
        // L'identifiant est toujours attribué par la base, jamais par l'appelant
        ordinateur.Id = 0;
        _context.Ordinateurs.Add(ordinateur);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Ordinateur {Id} ajouté : {Marque} {Modele}", ordinateur.Id, ordinateur.Marque, ordinateur.Modele);
        return ordinateur;
    }

    public async Task<bool> MettreAJourAsync(int id, Ordinateur ordinateur)
    {
        var existant = await _context.Ordinateurs.FindAsync(id);
        if (existant is null)
        {
            _logger.LogWarning("Mise à jour impossible : ordinateur {Id} introuvable", id);
            return false;
        }

        // Recopie de toutes les valeurs modifiables (l'identifiant reste celui de l'URL)
        existant.Marque = ordinateur.Marque;
        existant.Modele = ordinateur.Modele;
        existant.Processeur = ordinateur.Processeur;
        existant.RamGo = ordinateur.RamGo;
        existant.StockageGo = ordinateur.StockageGo;
        existant.Type = ordinateur.Type;
        existant.Prix = ordinateur.Prix;
        existant.QuantiteStock = ordinateur.QuantiteStock;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Ordinateur {Id} mis à jour", id);
        return true;
    }

    public async Task<bool> SupprimerAsync(int id)
    {
        var existant = await _context.Ordinateurs.FindAsync(id);
        if (existant is null)
        {
            _logger.LogWarning("Suppression impossible : ordinateur {Id} introuvable", id);
            return false;
        }

        _context.Ordinateurs.Remove(existant);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Ordinateur {Id} supprimé", id);
        return true;
    }
}
