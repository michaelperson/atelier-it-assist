namespace ItAssist.Domain.Tickets;

/// <summary>
/// Ticket du service d'assistance. Type immuable : toute évolution d'état passe
/// par une copie (with) produite par le domaine.
/// </summary>
public sealed record Ticket(
    string Id,
    string Titre,
    string Categorie,
    Priorite Priorite,
    StatutTicket Statut,
    string Demandeur,
    string Service,
    string Site,
    DateOnly OuvertLe,
    string Description,
    string? Resolution);
