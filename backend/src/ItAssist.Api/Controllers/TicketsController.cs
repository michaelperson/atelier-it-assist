namespace ItAssist.Api.Controllers;

using ItAssist.Domain.Commun;
using ItAssist.Domain.Tickets;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/tickets")]
[Tags("Tickets")]
public sealed class TicketsController : ControllerBase
{
    private readonly IDepotTickets _depot;
    private readonly RoutageService _routage;
    private readonly IJournal _journal;

    public TicketsController(IDepotTickets depot, RoutageService routage, IJournal journal)
    {
        _depot = depot;
        _routage = routage;
        _journal = journal;
    }

    /// <summary>
    /// Liste tous les tickets ou les tickets d'un service spécifique.
    /// </summary>
    /// <param name="service">Nom du service (optionnel)</param>
    /// <param name="jeton">Token d'annulation</param>
    /// <returns>Liste des tickets</returns>
    [HttpGet]
    public async Task<IActionResult> Lister([FromQuery] string? service, CancellationToken jeton)
    {
        var resultat = string.IsNullOrWhiteSpace(service)
            ? await _depot.ListerAsync(jeton)
            : await _depot.ParServiceAsync(service, jeton);

        return resultat.EstSucces
            ? Ok(resultat.Valeur)
            : VersReponse(resultat.CodeErreur);
    }

    /// <summary>
    /// Récupère un ticket par son ID.
    /// </summary>
    /// <param name="id">Identifiant du ticket</param>
    /// <param name="jeton">Token d'annulation</param>
    /// <returns>Le ticket demandé</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> ParId(string id, CancellationToken jeton)
    {
        var resultat = await _depot.ParIdAsync(id, jeton);
        return resultat.EstSucces
            ? Ok(resultat.Valeur)
            : VersReponse(resultat.CodeErreur);
    }

    /// <summary>
    /// Consulte les informations d'escalade d'un ticket.
    /// </summary>
    /// <param name="id">Identifiant du ticket</param>
    /// <param name="jeton">Token d'annulation</param>
    /// <returns>Informations d'escalade du ticket</returns>
    [HttpGet("{id}/escalade")]
    public async Task<IActionResult> Escalade(string id, CancellationToken jeton)
    {
        var ticket = await _depot.ParIdAsync(id, jeton);
        if (!ticket.EstSucces)
        {
            return VersReponse(ticket.CodeErreur);
        }

        var equipe = _routage.EquipeCible(ticket.Valeur);
        var doit = _routage.DoitEscalader(ticket.Valeur);
        var cible = _routage.PrioriteApresEscalade(ticket.Valeur);

        if (!doit.EstSucces || !cible.EstSucces)
        {
            return VersReponse(doit.EstSucces ? cible.CodeErreur : doit.CodeErreur);
        }

        _journal.Info("Escalade consultee", ("ticket", id), ("escalade", doit.Valeur.ToString()));

        return Ok(new
        {
            ticket = ticket.Valeur.Id,
            equipe = equipe.EstSucces ? equipe.Valeur : null,
            escaladeRequise = doit.Valeur,
            prioriteCible = cible.Valeur.ToString()
        });
    }

    /// <summary>
    /// Transposition unique des codes du domaine vers HTTP. Aucun contrôleur ne
    /// construit de réponse d'erreur autrement.
    /// </summary>
    private IActionResult VersReponse(string codeErreur) => codeErreur switch
    {
        CodesErreur.TicketInconnu => NotFound(new { code = codeErreur }),
        CodesErreur.ServiceInconnu => NotFound(new { code = codeErreur }),
        CodesErreur.PrioriteInvalide => BadRequest(new { code = codeErreur }),
        CodesErreur.DonneeManquante => BadRequest(new { code = codeErreur }),
        _ => StatusCode(StatusCodes.Status503ServiceUnavailable, new { code = codeErreur })
    };
}
