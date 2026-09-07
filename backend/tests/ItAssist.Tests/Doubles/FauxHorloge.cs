namespace ItAssist.Tests.Doubles;

using ItAssist.Domain.Commun;

public sealed class FauxHorloge : IHorloge
{
    public FauxHorloge(DateTime maintenant) => Maintenant = maintenant;

    public DateTime Maintenant { get; set; }
}
