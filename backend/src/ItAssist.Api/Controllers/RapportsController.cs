namespace ItAssist.Api.Controllers;

using ItAssist.Domain.Tickets;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Endpoints de reporting. Encore adossés au TicketService historique.
/// </summary>
[ApiController]
[Route("api/rapports")]
[Tags("Rapports")]
public class RapportsController : ControllerBase
{
    private readonly TicketService _service;

    public RapportsController(TicketService service) => _service = service;

    /// <summary>
    /// Génère un rapport mensuel au format texte.
    /// </summary>
    /// <param name="mois">Mois (1-12)</param>
    /// <param name="annee">Année</param>
    /// <returns>Rapport mensuel</returns>
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

    /// <summary>
    /// Retourne les statistiques des tickets par catégorie.
    /// </summary>
    /// <returns>Statistiques par catégorie</returns>
    [HttpGet("par-categorie")]
    public IActionResult ParCategorie() => Ok(_service.StatistiquesParCategorie());

    /// <summary>
    /// Retourne les statistiques des tickets par site.
    /// </summary>
    /// <returns>Statistiques par site</returns>
    [HttpGet("par-site")]
    public IActionResult ParSite() => Ok(_service.StatistiquesParSite());

    /// <summary>
    /// Recherche des tickets selon plusieurs critères.
    /// </summary>
    /// <param name="texte">Texte de recherche (optionnel)</param>
    /// <param name="service">Service (optionnel)</param>
    /// <param name="categorie">Catégorie (optionnel)</param>
    /// <param name="priorite">Priorité (optionnel)</param>
    /// <param name="ouvertsSeulement">Afficher seulement les tickets ouverts</param>
    /// <returns>Résultats de recherche</returns>
    [HttpGet("recherche")]
    public IActionResult Recherche(string? texte, string? service, string? categorie, string? priorite, bool ouvertsSeulement = false)
        => Ok(_service.Rechercher(texte, service, categorie, priorite, ouvertsSeulement));
}
