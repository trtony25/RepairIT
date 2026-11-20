using Microsoft.EntityFrameworkCore;
using RepairIT.Shared.Models;

namespace RepairIT.CatalogService.Data;

/// <summary>
/// Initialisation de la base du catalogue au démarrage du service :
/// application des migrations puis insertion d'un jeu de données de départ si la table est vide.
/// </summary>
public static class CatalogueDbInitializer
{
    public static void Initialiser(CatalogueContext context, ILogger logger)
    {
        // Les migrations ne s'appliquent qu'à une vraie base (SQLite) ;
        // le fournisseur InMemory des tests se contente de créer le modèle.
        if (context.Database.IsRelational())
        {
            context.Database.Migrate();
        }
        else
        {
            context.Database.EnsureCreated();
        }

        if (context.Ordinateurs.Any())
        {
            logger.LogInformation("Base catalogue déjà initialisée ({Nombre} ordinateurs)", context.Ordinateurs.Count());
            return;
        }

        context.Ordinateurs.AddRange(OrdinateursInitiaux());
        context.SaveChanges();
        logger.LogInformation("Base catalogue initialisée avec les ordinateurs de départ");
    }

    /// <summary>Catalogue de démonstration inséré lors du premier lancement.</summary>
    private static IEnumerable<Ordinateur> OrdinateursInitiaux() =>
    [
        new() { Marque = "Dell", Modele = "Latitude 5440", Processeur = "Intel Core i5-1335U", RamGo = 16, StockageGo = 512, Type = TypeOrdinateur.Portable, Prix = 1099.00m, QuantiteStock = 8 },
        new() { Marque = "HP", Modele = "EliteBook 840 G10", Processeur = "Intel Core i7-1355U", RamGo = 32, StockageGo = 1024, Type = TypeOrdinateur.Portable, Prix = 1549.00m, QuantiteStock = 5 },
        new() { Marque = "Lenovo", Modele = "ThinkCentre M70q", Processeur = "Intel Core i5-13400T", RamGo = 16, StockageGo = 512, Type = TypeOrdinateur.Fixe, Prix = 789.00m, QuantiteStock = 12 },
        new() { Marque = "Apple", Modele = "MacBook Air 13 M3", Processeur = "Apple M3", RamGo = 16, StockageGo = 512, Type = TypeOrdinateur.Portable, Prix = 1499.00m, QuantiteStock = 3 },
        new() { Marque = "Asus", Modele = "Zen AiO 24", Processeur = "AMD Ryzen 7 7730U", RamGo = 16, StockageGo = 1024, Type = TypeOrdinateur.ToutEnUn, Prix = 999.00m, QuantiteStock = 0 }
    ];
}
