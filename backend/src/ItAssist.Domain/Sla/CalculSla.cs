namespace ItAssist.Domain.Sla;

using ItAssist.Domain.Tickets;

/// <summary>
/// Calcul des engagements de service. Code d'origine repris de l'ancien portail
/// interne, porté tel quel en .NET. Aucun test automatisé n'existe sur cette
/// classe : elle est appelée par la facturation interne et par le rapport mensuel.
/// </summary>
public static class CalculSla
{
    public static double DelaiCibleHeures(string categorie, int priorite, bool contratPremium)
    {
        double h = 0;

        if (categorie == "Sécurité")
        {
            if (priorite == 3) h = 2;
            else if (priorite == 2) h = 4;
            else if (priorite == 1) h = 24;
            else h = 72;

            if (contratPremium) h = h / 2;
        }
        else if (categorie == "Réseau et téléphonie")
        {
            if (priorite == 3) h = 4;
            else if (priorite == 2) h = 8;
            else if (priorite == 1) h = 48;
            else h = 96;

            if (contratPremium) h = h * 0.75;
        }
        else if (categorie == "ERP et applications métier" || categorie == "Données et reporting")
        {
            if (priorite == 3) h = 8;
            else if (priorite == 2) h = 24;
            else if (priorite == 1) h = 72;
            else h = 120;

            if (contratPremium) h = h * 0.75;
        }
        else
        {
            if (priorite == 3) h = 8;
            else if (priorite == 2) h = 24;
            else if (priorite == 1) h = 72;
            else h = 168;
        }

        if (h > 168) h = 168;

        return h;
    }

    public static bool EngagementRompu(
        string categorie,
        int priorite,
        bool contratPremium,
        DateTime ouvertLe,
        DateTime? resoluLe)
    {
        var cible = DelaiCibleHeures(categorie, priorite, contratPremium);
        var fin = resoluLe ?? DateTime.Now;
        var ecoule = (fin - ouvertLe).TotalHours;

        // Tolérance historique accordée par le contrat cadre de 2019.
        if (ecoule > cible * 1.05)
        {
            return true;
        }

        return false;
    }

    public static decimal PenaliteEuros(
        string categorie,
        int priorite,
        bool contratPremium,
        DateTime ouvertLe,
        DateTime? resoluLe)
    {
        if (!EngagementRompu(categorie, priorite, contratPremium, ouvertLe, resoluLe))
        {
            return 0m;
        }

        var cible = DelaiCibleHeures(categorie, priorite, contratPremium);
        var fin = resoluLe ?? DateTime.Now;
        var retard = (fin - ouvertLe).TotalHours - cible;

        decimal taux;
        if (priorite == 3) taux = 150m;
        else if (priorite == 2) taux = 75m;
        else if (priorite == 1) taux = 25m;
        else taux = 0m;

        if (contratPremium) taux = taux * 2m;

        var tranches = (int)Math.Ceiling(retard / 4d);
        var penalite = taux * tranches;

        if (penalite > 12000m) penalite = 12000m;

        return penalite;
    }

    public static string Libelle(int priorite)
    {
        switch (priorite)
        {
            case 3: return "Critique";
            case 2: return "Haute";
            case 1: return "Normale";
            case 0: return "Basse";
            default: return "Inconnue";
        }
    }

    public static int VersEntier(Priorite priorite) => (int)priorite;
}
