using RepairIT.Shared.Models;

namespace RepairIT.CatalogService.Services;

/// <summary>
/// Opérations CRUD sur les ordinateurs du catalogue.
/// L'interface permet l'injection de dépendances et le remplacement par un mock dans les tests.
/// </summary>
public interface IOrdinateurService
{
    Task<List<Ordinateur>> ListerAsync();

    /// <returns>L'ordinateur, ou null s'il n'existe pas.</returns>
    Task<Ordinateur?> ObtenirParIdAsync(int id);

    /// <returns>L'ordinateur créé, avec son identifiant.</returns>
    Task<Ordinateur> AjouterAsync(Ordinateur ordinateur);

    /// <returns>false si aucun ordinateur ne porte cet identifiant.</returns>
    Task<bool> MettreAJourAsync(int id, Ordinateur ordinateur);

    /// <returns>false si aucun ordinateur ne porte cet identifiant.</returns>
    Task<bool> SupprimerAsync(int id);
}
