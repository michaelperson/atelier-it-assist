namespace ItAssist.Tests;

using ItAssist.Domain.Commun;
using ItAssist.Domain.Tickets;
using ItAssist.Tests.Doubles;
using Xunit;

/// <summary>
/// Convention de nommage : Methode_Contexte_ResultatAttendu.
/// Un test valide une seule règle et n'utilise qu'une seule assertion logique.
/// </summary>
public sealed class RoutageServiceTests
{
    private static RoutageService Service(DateTime maintenant)
        => new(new FauxHorloge(maintenant), new JournalMuet());

    [Fact]
    public void EquipeCible_CategorieSecurite_RenvoieSecuriteOperationnelle()
    {
        var service = Service(new DateTime(2026, 3, 10));
        var ticket = FabriqueTicket.Nominal(categorie: "Sécurité");

        var resultat = service.EquipeCible(ticket);

        Assert.True(resultat.EstSucces);
        Assert.Equal("Sécurité opérationnelle", resultat.Valeur);
    }

    [Fact]
    public void EquipeCible_CategorieInconnue_EchoueAvecServiceInconnu()
    {
        var service = Service(new DateTime(2026, 3, 10));
        var ticket = FabriqueTicket.Nominal(categorie: "Cantine");

        var resultat = service.EquipeCible(ticket);

        Assert.False(resultat.EstSucces);
        Assert.Equal(CodesErreur.ServiceInconnu, resultat.CodeErreur);
    }

    [Fact]
    public void JoursOuvresEntre_SemaineComplete_IgnoreLeWeekEnd()
    {
        // Lundi 2 mars 2026 -> lundi 9 mars 2026 : 5 jours ouvrés.
        var compte = RoutageService.JoursOuvresEntre(
            new DateOnly(2026, 3, 2),
            new DateOnly(2026, 3, 9));

        Assert.Equal(5, compte);
    }

    [Fact]
    public void DoitEscalader_TicketClos_RenvoieFaux()
    {
        var service = Service(new DateTime(2026, 6, 1));
        var ticket = FabriqueTicket.Nominal(statut: StatutTicket.Clos, ouvertLe: new DateOnly(2026, 1, 5));

        var resultat = service.DoitEscalader(ticket);

        Assert.True(resultat.EstSucces);
        Assert.False(resultat.Valeur);
    }
}
