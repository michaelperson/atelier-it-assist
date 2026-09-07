namespace ItAssist.Domain.Commun;

/// <summary>
/// Résultat d'une opération métier. Le domaine ne lève pas d'exception pour un
/// cas de gestion prévu : il renvoie un échec porteur d'un code d'erreur.
/// </summary>
public readonly struct Resultat<T>
{
    private readonly T? _valeur;

    public bool EstSucces { get; }

    public string CodeErreur { get; }

    private Resultat(T valeur)
    {
        _valeur = valeur;
        EstSucces = true;
        CodeErreur = string.Empty;
    }

    private Resultat(string codeErreur)
    {
        _valeur = default;
        EstSucces = false;
        CodeErreur = codeErreur;
    }

    public static Resultat<T> Succes(T valeur) => new(valeur);

    public static Resultat<T> Echec(string codeErreur) => new(codeErreur);

    public T Valeur => EstSucces
        ? _valeur!
        : throw new InvalidOperationException(
            $"Lecture de la valeur d'un résultat en échec ({CodeErreur}).");
}
