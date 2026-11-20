using RepairIT.Shared.Dtos;
using RepairIT.Shared.Models;

namespace RepairIT.UserService.Services;

/// <summary>
/// Gestion des utilisateurs pour la V1 : création et consultation.
/// (Attribution de rôle et activation/désactivation viendront avec Keycloak.)
/// </summary>
public interface IUtilisateurService
{
    Task<List<Utilisateur>> ListerAsync();

    /// <returns>L'utilisateur, ou null s'il n'existe pas.</returns>
    Task<Utilisateur?> ObtenirParIdAsync(int id);

    /// <returns>L'utilisateur créé, ou null si l'e-mail est déjà utilisé.</returns>
    Task<Utilisateur?> CreerAsync(UtilisateurCreationDto demande);
}
