namespace ItAssist.Api.Controllers;

using ItAssist.Domain.Tickets;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Endpoints de reporting. Encore adossés au TicketService historique.
/// </summary>
[ApiController]
[Route("api/rapports")]
public class RapportsController : ControllerBase
{
    private readonly TicketService _service;

    public RapportsController(TicketService service) => _service = service;

    [HttpGet("mensuel")]
    public IActionResult Mensuel(int mois, int annee)
    {
        try
        {
            var texte = _service.RapportMensuelTexte(mois, annee);
            return Content(texte, "text/plain; charset=utf-8");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("par-categorie")]
    public IActionResult ParCategorie() => Ok(_service.StatistiquesParCategorie());

    [HttpGet("par-site")]
    public IActionResult ParSite() => Ok(_service.StatistiquesParSite());

    [HttpGet("recherche")]
    public IActionResult Recherche(string? texte, string? service, string? categorie, string? priorite, bool ouvertsSeulement = false)
        => Ok(_service.Rechercher(texte, service, categorie, priorite, ouvertsSeulement));
}
