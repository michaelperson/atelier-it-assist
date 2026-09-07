namespace ItAssist.Domain.Commun;

/// <summary>
/// Codes d'erreur du domaine. Aucun littéral de code d'erreur n'est écrit
/// ailleurs dans la solution : l'API les transpose en codes HTTP.
/// </summary>
public static class CodesErreur
{
    public const string TicketInconnu = "TICKET_INCONNU";
    public const string TransitionInterdite = "TRANSITION_INTERDITE";
    public const string ServiceInconnu = "SERVICE_INCONNU";
    public const string PrioriteInvalide = "PRIORITE_INVALIDE";
    public const string DonneeManquante = "DONNEE_MANQUANTE";
    public const string DepotIndisponible = "DEPOT_INDISPONIBLE";
}
