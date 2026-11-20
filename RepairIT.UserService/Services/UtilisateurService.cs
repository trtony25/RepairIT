using Microsoft.EntityFrameworkCore;
using RepairIT.Shared.Dtos;
using RepairIT.Shared.Models;
using RepairIT.UserService.Data;

namespace RepairIT.UserService.Services;

/// <summary>
/// Implémentation EF Core de la gestion des utilisateurs (Scoped, comme le DbContext).
/// </summary>
public class UtilisateurService : IUtilisateurService
{
    private readonly UtilisateurContext _context;
    private readonly ILogger<UtilisateurService> _logger;

    public UtilisateurService(UtilisateurContext context, ILogger<UtilisateurService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Utilisateur>> ListerAsync()
    {
        return await _context.Utilisateurs
            .AsNoTracking()
            .OrderBy(u => u.Nom)
            .ThenBy(u => u.Prenom)
            .ToListAsync();
    }

    public async Task<Utilisateur?> ObtenirParIdAsync(int id)
    {
        return await _context.Utilisateurs.FindAsync(id);
    }

    public async Task<Utilisateur?> CreerAsync(UtilisateurCreationDto demande)
    {
        // E-mail normalisé pour que "A@x.fr" et "a@x.fr" soient bien considérés comme identiques
        var email = demande.Email.Trim().ToLowerInvariant();

        if (await _context.Utilisateurs.AnyAsync(u => u.Email == email))
        {
            _logger.LogWarning("Création refusée : l'e-mail {Email} est déjà utilisé", email);
            return null;
        }

        var utilisateur = new Utilisateur
        {
            Nom = demande.Nom.Trim(),
            Prenom = demande.Prenom.Trim(),
            Email = email,
            Role = demande.Role,
            // Pas encore de Keycloak en V1 : valeur par défaut, comme demandé par le sujet
            UserKeycloak = Utilisateur.UserKeycloakParDefaut,
            Actif = true
        };

        _context.Utilisateurs.Add(utilisateur);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Utilisateur {Id} créé avec le rôle {Role}", utilisateur.Id, utilisateur.Role);
        return utilisateur;
    }
}
