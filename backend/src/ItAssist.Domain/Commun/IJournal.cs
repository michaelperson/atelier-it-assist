namespace ItAssist.Domain.Commun;

/// <summary>
/// Journalisation applicative. Le contexte est passé en paires clé/valeur pour
/// rester exploitable en recherche ; aucune donnée personnelle ne doit y figurer.
/// </summary>
public interface IJournal
{
    void Info(string message, params (string Cle, string Valeur)[] contexte);

    void Avertissement(string message, params (string Cle, string Valeur)[] contexte);

    void Erreur(string message, Exception? exception, params (string Cle, string Valeur)[] contexte);
}
