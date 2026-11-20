using System.ComponentModel.DataAnnotations;

namespace RepairIT.Shared.Models;

/// <summary>
/// Ordinateur référencé dans le catalogue de RepairIT (vendu ou utilisé pour les réparations).
/// Les annotations servent à la fois à la validation automatique des requêtes de l'API
/// (attribut [ApiController]) et à la définition des colonnes par EF Core.
/// </summary>
public class Ordinateur
{
    /// <summary>Identifiant technique généré par la base de données.</summary>
    public int Id { get; set; }

    [Required(ErrorMessage = "La marque est obligatoire.")]
    [StringLength(50)]
    public string Marque { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le modèle est obligatoire.")]
    [StringLength(100)]
    public string Modele { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le processeur est obligatoire.")]
    [StringLength(100)]
    public string Processeur { get; set; } = string.Empty;

    /// <summary>Mémoire vive en gigaoctets.</summary>
    [Range(1, 1024, ErrorMessage = "La RAM doit être comprise entre 1 et 1024 Go.")]
    public int RamGo { get; set; }

    /// <summary>Capacité de stockage en gigaoctets.</summary>
    [Range(1, 100_000, ErrorMessage = "Le stockage doit être compris entre 1 et 100 000 Go.")]
    public int StockageGo { get; set; }

    public TypeOrdinateur Type { get; set; }

    /// <summary>Prix de vente unitaire en euros.</summary>
    [Range(0, 1_000_000, ErrorMessage = "Le prix doit être positif.")]
    public decimal Prix { get; set; }

    /// <summary>Nombre d'exemplaires disponibles en stock.</summary>
    [Range(0, 100_000, ErrorMessage = "La quantité en stock ne peut pas être négative.")]
    public int QuantiteStock { get; set; }
}
