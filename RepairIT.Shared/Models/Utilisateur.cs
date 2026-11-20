using System.ComponentModel.DataAnnotations;

namespace RepairIT.Shared.Models;

/// <summary>
/// Utilisateur de l'application (gestionnaire ou administrateur).
/// Aucun mot de passe n'est stocké : l'authentification sera entièrement déléguée à Keycloak.
/// </summary>
public class Utilisateur
{
    /// <summary>
    /// Valeur provisoire du lien Keycloak : la gestion Keycloak n'existe pas encore en V1,
    /// le sujet demande donc de renseigner une valeur par défaut pour ce champ.
    /// </summary>
    public const string UserKeycloakParDefaut = "00000000-0000-0000-0000-000000000000";

    /// <summary>Identifiant technique unique généré par la base de données.</summary>
    public int Id { get; set; }

    /// <summary>Identifiant (sub) de l'utilisateur dans Keycloak.</summary>
    [Required]
    [StringLength(36)]
    public string UserKeycloak { get; set; } = UserKeycloakParDefaut;

    [Required]
    [StringLength(100)]
    public string Nom { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Prenom { get; set; } = string.Empty;

    /// <summary>Adresse e-mail, unique pour chaque utilisateur.</summary>
    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    public RoleUtilisateur Role { get; set; } = RoleUtilisateur.Gestionnaire;

    /// <summary>Un compte désactivé ne pourra plus se connecter (règle appliquée avec Keycloak).</summary>
    public bool Actif { get; set; } = true;
}
