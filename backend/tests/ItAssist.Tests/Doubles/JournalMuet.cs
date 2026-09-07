namespace ItAssist.Tests.Doubles;

using ItAssist.Domain.Commun;

public sealed class JournalMuet : IJournal
{
    public List<string> Messages { get; } = new();

    public void Info(string message, params (string Cle, string Valeur)[] contexte) => Messages.Add(message);

    public void Avertissement(string message, params (string Cle, string Valeur)[] contexte) => Messages.Add(message);

    public void Erreur(string message, Exception? exception, params (string Cle, string Valeur)[] contexte) => Messages.Add(message);
}
