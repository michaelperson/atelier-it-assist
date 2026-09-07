namespace ItAssist.Domain.Tickets;

using ItAssist.Domain.Commun;

/// <summary>
/// Règles d'escalade. Un ticket dont le délai de traitement dépasse l'engagement
/// associé à sa priorité doit être escaladé au niveau supérieur.
/// </summary>
public sealed class RoutageService
{
    private readonly IHorloge _horloge;
    private readonly IJournal _journal;

    // Engagement de prise en charge, exprimé en jours ouvrés.
    private static readonly Dictionary<Priorite, int> EngagementJoursOuvres = new()
    {
        [Priorite.Critique] = 2,
        [Priorite.Haute] = 1,
        [Priorite.Normale] = 5,
        [Priorite.Basse] = 10
    };

    private static readonly Dictionary<string, string> ServiceParCategorie = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Accès et authentification"] = "Support N1",
        ["ERP et applications métier"] = "Applications métier",
        ["Poste de travail"] = "Support N1",
        ["Réseau et téléphonie"] = "Infrastructure",
        ["Données et reporting"] = "Applications métier",
        ["Sécurité"] = "Sécurité opérationnelle"
    };

    public RoutageService(IHorloge horloge, IJournal journal)
    {
        _horloge = horloge;
        _journal = journal;
    }

    public Resultat<string> EquipeCible(Ticket ticket)
    {
        if (!ServiceParCategorie.TryGetValue(ticket.Categorie, out var equipe))
        {
            _journal.Avertissement(
                "Categorie sans equipe de rattachement",
                ("ticket", ticket.Id),
                ("categorie", ticket.Categorie));

            return Resultat<string>.Echec(CodesErreur.ServiceInconnu);
        }

        return Resultat<string>.Succes(equipe);
    }

    public Resultat<bool> DoitEscalader(Ticket ticket)
    {
        if (ticket.Statut is StatutTicket.Resolu or StatutTicket.Clos)
        {
            return Resultat<bool>.Succes(false);
        }

        if (!EngagementJoursOuvres.TryGetValue(ticket.Priorite, out var engagement))
        {
            return Resultat<bool>.Echec(CodesErreur.PrioriteInvalide);
        }

        var aujourdhui = DateOnly.FromDateTime(_horloge.Maintenant);
        var ecoules = JoursOuvresEntre(ticket.OuvertLe, aujourdhui);
        var escalade = ecoules > engagement;

        _journal.Info(
            "Evaluation escalade",
            ("ticket", ticket.Id),
            ("priorite", ticket.Priorite.ToString()),
            ("engagement", engagement.ToString()),
            ("joursOuvresEcoules", ecoules.ToString()),
            ("escalade", escalade.ToString()));

        return Resultat<bool>.Succes(escalade);
    }

    public Resultat<Priorite> PrioriteApresEscalade(Ticket ticket)
    {
        var doit = DoitEscalader(ticket);
        if (!doit.EstSucces)
        {
            return Resultat<Priorite>.Echec(doit.CodeErreur);
        }

        if (!doit.Valeur || ticket.Priorite == Priorite.Critique)
        {
            return Resultat<Priorite>.Succes(ticket.Priorite);
        }

        return Resultat<Priorite>.Succes(ticket.Priorite + 1);
    }

    // TODO(dette connue) : les jours fériés belges ne sont pas pris en compte.
    // Une table de jours fériés est prévue mais non implémentée.
    public static int JoursOuvresEntre(DateOnly debut, DateOnly fin)
    {
        if (fin <= debut)
        {
            return 0;
        }

        var compte = 0;
        for (var jour = debut; jour < fin; jour = jour.AddDays(1))
        {
            if (jour.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday)
            {
                compte++;
            }
        }

        return compte;
    }
}
