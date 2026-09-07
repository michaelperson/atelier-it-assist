namespace ItAssist.Api.Infrastructure;

using ItAssist.Domain.Commun;

/// <summary>
/// Implémentation de IJournal adossée à ILogger. Le format de sortie est
/// « message | cle=valeur | cle=valeur » pour rester lisible en recherche texte.
/// </summary>
public sealed class JournalConsole : IJournal
{
    private readonly ILogger<JournalConsole> _logger;

    public JournalConsole(ILogger<JournalConsole> logger) => _logger = logger;

    public void Info(string message, params (string Cle, string Valeur)[] contexte)
        => _logger.LogInformation("{Message}", Formater(message, contexte));

    public void Avertissement(string message, params (string Cle, string Valeur)[] contexte)
        => _logger.LogWarning("{Message}", Formater(message, contexte));

    public void Erreur(string message, Exception? exception, params (string Cle, string Valeur)[] contexte)
        => _logger.LogError(exception, "{Message}", Formater(message, contexte));

    private static string Formater(string message, (string Cle, string Valeur)[] contexte)
        => contexte.Length == 0
            ? message
            : message + " | " + string.Join(" | ", contexte.Select(c => $"{c.Cle}={c.Valeur}"));
}
