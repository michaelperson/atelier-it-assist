namespace ItAssist.Domain.Tickets;

using ItAssist.Domain.Commun;

/// <summary>
/// Accès aux tickets. Toute méthode d'entrée/sortie prend un CancellationToken
/// en dernier paramètre.
/// </summary>
public interface IDepotTickets
{
    Task<Resultat<Ticket>> ParIdAsync(string id, CancellationToken jeton);

    Task<Resultat<IReadOnlyList<Ticket>>> ListerAsync(CancellationToken jeton);

    Task<Resultat<IReadOnlyList<Ticket>>> ParServiceAsync(string service, CancellationToken jeton);
}
