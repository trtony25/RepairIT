namespace RepairIT.Shared.Configuration;

/// <summary>
/// Section "Application" des fichiers appsettings.json, liée via le pattern Options.
/// Elle décrit le micro-service qui s'exécute (nom affiché, version livrée).
/// </summary>
public class ApplicationOptions
{
    /// <summary>Nom de la section dans appsettings.json.</summary>
    public const string Section = "Application";

    public string Nom { get; set; } = string.Empty;

    public string Version { get; set; } = string.Empty;
}
