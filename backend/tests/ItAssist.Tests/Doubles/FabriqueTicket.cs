namespace ItAssist.Tests.Doubles;

using ItAssist.Domain.Tickets;

/// <summary>
/// Fabrique de tickets de test. Les tests ne construisent jamais un Ticket
/// directement : ils partent du cas nominal et surchargent le seul champ utile.
/// </summary>
public static class FabriqueTicket
{
    public static Ticket Nominal(
        string? id = null,
        string? categorie = null,
        Priorite? priorite = null,
        StatutTicket? statut = null,
        DateOnly? ouvertLe = null)
        => new(
            id ?? "INC-2026-0001",
            "Titre de test",
            categorie ?? "Poste de travail",
            priorite ?? Priorite.Normale,
            statut ?? StatutTicket.EnCours,
            "Demandeur de test",
            "Comptabilité",
            "Namur",
            ouvertLe ?? new DateOnly(2026, 3, 2),
            "Description de test",
            null);
}
