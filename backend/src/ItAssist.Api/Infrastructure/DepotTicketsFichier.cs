namespace ItAssist.Api.Infrastructure;

using System.Text.Json;
using System.Text.Json.Serialization;
using ItAssist.Domain.Commun;
using ItAssist.Domain.Tickets;

/// <summary>
/// Dépôt de tickets adossé au fichier JSONL de démonstration. Implémentation de
/// référence des conventions du projet : Resultat en sortie, jeton d'annulation
/// en dernier paramètre, journalisation par IJournal.
/// </summary>
public sealed class DepotTicketsFichier : IDepotTickets
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly string _chemin;
    private readonly IJournal _journal;
    private IReadOnlyList<Ticket>? _tickets;

    public DepotTicketsFichier(string chemin, IJournal journal)
    {
        _chemin = chemin;
        _journal = journal;
    }

    public async Task<Resultat<IReadOnlyList<Ticket>>> ListerAsync(CancellationToken jeton)
    {
        if (_tickets is not null)
        {
            return Resultat<IReadOnlyList<Ticket>>.Succes(_tickets);
        }

        if (!File.Exists(_chemin))
        {
            _journal.Erreur("Fichier de tickets absent", null, ("chemin", _chemin));
            return Resultat<IReadOnlyList<Ticket>>.Echec(CodesErreur.DepotIndisponible);
        }

        var lus = new List<Ticket>();
        await foreach (var ligne in File.ReadLinesAsync(_chemin, jeton))
        {
            if (string.IsNullOrWhiteSpace(ligne))
            {
                continue;
            }

            var brut = JsonSerializer.Deserialize<TicketBrut>(ligne, Options);
            if (brut is null)
            {
                continue;
            }

            lus.Add(brut.VersDomaine());
        }

        _tickets = lus;
        _journal.Info("Depot charge", ("chemin", _chemin), ("tickets", lus.Count.ToString()));
        return Resultat<IReadOnlyList<Ticket>>.Succes(_tickets);
    }

    public async Task<Resultat<Ticket>> ParIdAsync(string id, CancellationToken jeton)
    {
        var tous = await ListerAsync(jeton);
        if (!tous.EstSucces)
        {
            return Resultat<Ticket>.Echec(tous.CodeErreur);
        }

        var trouve = tous.Valeur.FirstOrDefault(t => t.Id == id);
        return trouve is null
            ? Resultat<Ticket>.Echec(CodesErreur.TicketInconnu)
            : Resultat<Ticket>.Succes(trouve);
    }

    public async Task<Resultat<IReadOnlyList<Ticket>>> ParServiceAsync(string service, CancellationToken jeton)
    {
        var tous = await ListerAsync(jeton);
        if (!tous.EstSucces)
        {
            return tous;
        }

        var filtres = tous.Valeur
            .Where(t => string.Equals(t.Service, service, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Resultat<IReadOnlyList<Ticket>>.Succes(filtres);
    }

    private sealed record TicketBrut(
        string Id,
        string Titre,
        string Categorie,
        string Priorite,
        string Statut,
        string Demandeur,
        string Service,
        string Site,
        string OuvertLe,
        string Description,
        string? Resolution)
    {
        public Ticket VersDomaine() => new(
            Id,
            Titre,
            Categorie,
            LirePriorite(Priorite),
            LireStatut(Statut),
            Demandeur,
            Service,
            Site,
            DateOnly.Parse(OuvertLe),
            Description,
            string.IsNullOrWhiteSpace(Resolution) ? null : Resolution);

        private static Priorite LirePriorite(string valeur) => valeur switch
        {
            "Critique" => Domain.Tickets.Priorite.Critique,
            "Haute" => Domain.Tickets.Priorite.Haute,
            "Basse" => Domain.Tickets.Priorite.Basse,
            _ => Domain.Tickets.Priorite.Normale
        };

        private static StatutTicket LireStatut(string valeur) => valeur switch
        {
            "En cours" => StatutTicket.EnCours,
            "En attente demandeur" => StatutTicket.EnAttenteDemandeur,
            "Résolu" => StatutTicket.Resolu,
            "Fermé" => StatutTicket.Clos,
            _ => StatutTicket.Nouveau
        };
    }
}
