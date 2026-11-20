using System.ComponentModel.DataAnnotations;
using RepairIT.Shared.Models;

namespace RepairIT.Shared.Dtos;

/// <summary>
/// Données attendues pour créer un utilisateur.
/// L'identifiant, le lien Keycloak et l'état du compte ne sont pas fournis par l'appelant :
/// ils sont fixés par le service lors de la création.
/// </summary>
public class UtilisateurCreationDto
{
    [Required(ErrorMessage = "Le nom est obligatoire.")]
    [StringLength(100)]
    public string Nom { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le prénom est obligatoire.")]
    [StringLength(100)]
    public string Prenom { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'e-mail est obligatoire.")]
    [EmailAddress(ErrorMessage = "L'e-mail n'est pas valide.")]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    public RoleUtilisateur Role { get; set; } = RoleUtilisateur.Gestionnaire;
}
