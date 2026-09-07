namespace ItAssist.Domain.Tickets;

using System.Text;
using System.Text.Json;

/// <summary>
/// Service de tickets historique. Ajouté au fil des demandes depuis la reprise
/// du portail interne. Point d'entrée unique utilisé par l'API, par le rapport
/// mensuel et par un script de nuit.
/// </summary>
public class TicketService
{
    private static List<Ticket>? _cache;
    private static DateTime _cacheLe;

    public string CheminFichier { get; set; } = "data/tickets.jsonl";

    public List<Ticket> ChargerTout()
    {
        if (_cache != null && (DateTime.Now - _cacheLe).TotalMinutes < 15)
        {
            return _cache;
        }

        if (!File.Exists(CheminFichier))
        {
            throw new FileNotFoundException("Fichier de tickets introuvable : " + CheminFichier);
        }

        var liste = new List<Ticket>();
        foreach (var ligne in File.ReadAllLines(CheminFichier))
        {
            if (string.IsNullOrWhiteSpace(ligne))
            {
                continue;
            }

            var doc = JsonDocument.Parse(ligne).RootElement;

            var prio = doc.GetProperty("priorite").GetString();
            var p = Priorite.Normale;
            if (prio == "Critique") p = Priorite.Critique;
            if (prio == "Haute") p = Priorite.Haute;
            if (prio == "Basse") p = Priorite.Basse;

            var st = doc.GetProperty("statut").GetString();
            var s = StatutTicket.Nouveau;
            if (st == "En cours") s = StatutTicket.EnCours;
            if (st == "En attente demandeur") s = StatutTicket.EnAttenteDemandeur;
            if (st == "Résolu") s = StatutTicket.Resolu;
            if (st == "Fermé") s = StatutTicket.Clos;

            string? resolution = null;
            if (doc.TryGetProperty("resolution", out var r))
            {
                resolution = r.GetString();
            }

            liste.Add(new Ticket(
                doc.GetProperty("id").GetString()!,
                doc.GetProperty("titre").GetString()!,
                doc.GetProperty("categorie").GetString()!,
                p,
                s,
                doc.GetProperty("demandeur").GetString()!,
                doc.GetProperty("service").GetString()!,
                doc.GetProperty("site").GetString()!,
                DateOnly.Parse(doc.GetProperty("ouvert_le").GetString()!),
                doc.GetProperty("description").GetString()!,
                resolution));
        }

        _cache = liste;
        _cacheLe = DateTime.Now;
        Console.WriteLine("[TicketService] " + liste.Count + " tickets charges depuis " + CheminFichier);
        return liste;
    }

    public Ticket Get(string id)
    {
        var t = ChargerTout().FirstOrDefault(x => x.Id == id);
        if (t == null)
        {
            throw new Exception("Ticket inconnu : " + id);
        }

        return t;
    }

    public List<Ticket> Rechercher(string? texte, string? service, string? categorie, string? priorite, bool ouvertsSeulement)
    {
        var res = ChargerTout();

        if (texte != null && texte != "")
        {
            res = res.Where(t => t.Titre.ToLower().Contains(texte.ToLower())
                              || t.Description.ToLower().Contains(texte.ToLower())
                              || (t.Resolution != null && t.Resolution.ToLower().Contains(texte.ToLower()))).ToList();
        }

        if (service != null && service != "")
        {
            res = res.Where(t => t.Service == service).ToList();
        }

        if (categorie != null && categorie != "")
        {
            res = res.Where(t => t.Categorie == categorie).ToList();
        }

        if (priorite != null && priorite != "")
        {
            if (priorite == "Critique") res = res.Where(t => t.Priorite == Priorite.Critique).ToList();
            else if (priorite == "Haute") res = res.Where(t => t.Priorite == Priorite.Haute).ToList();
            else if (priorite == "Normale") res = res.Where(t => t.Priorite == Priorite.Normale).ToList();
            else if (priorite == "Basse") res = res.Where(t => t.Priorite == Priorite.Basse).ToList();
        }

        if (ouvertsSeulement)
        {
            res = res.Where(t => t.Statut != StatutTicket.Resolu && t.Statut != StatutTicket.Clos).ToList();
        }

        return res;
    }

    public Dictionary<string, int> StatistiquesParCategorie()
    {
        var d = new Dictionary<string, int>();
        foreach (var t in ChargerTout())
        {
            if (d.ContainsKey(t.Categorie)) d[t.Categorie] = d[t.Categorie] + 1;
            else d.Add(t.Categorie, 1);
        }

        return d;
    }

    public Dictionary<string, int> StatistiquesParSite()
    {
        var d = new Dictionary<string, int>();
        foreach (var t in ChargerTout())
        {
            if (d.ContainsKey(t.Site)) d[t.Site] = d[t.Site] + 1;
            else d.Add(t.Site, 1);
        }

        return d;
    }

    public string RapportMensuelTexte(int mois, int annee)
    {
        var sb = new StringBuilder();
        sb.AppendLine("RAPPORT MENSUEL " + mois + "/" + annee);
        sb.AppendLine("Genere le " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
        sb.AppendLine(new string('-', 60));

        var tickets = ChargerTout().Where(t => t.OuvertLe.Month == mois && t.OuvertLe.Year == annee).ToList();
        sb.AppendLine("Tickets ouverts sur la periode : " + tickets.Count);

        foreach (var groupe in tickets.GroupBy(t => t.Categorie).OrderByDescending(g => g.Count()))
        {
            sb.AppendLine("  " + groupe.Key.PadRight(35) + groupe.Count());
            foreach (var t in groupe.Where(x => x.Priorite == Priorite.Critique))
            {
                sb.AppendLine("      CRITIQUE " + t.Id + " " + t.Titre + " (" + t.Demandeur + ")");
            }
        }

        var ouverts = tickets.Where(t => t.Statut != StatutTicket.Resolu && t.Statut != StatutTicket.Clos).Count();
        sb.AppendLine(new string('-', 60));
        sb.AppendLine("Encore ouverts : " + ouverts + " (" + (tickets.Count == 0 ? 0 : ouverts * 100 / tickets.Count) + " %)");

        return sb.ToString();
    }

    public void ViderCache()
    {
        _cache = null;
    }
}
