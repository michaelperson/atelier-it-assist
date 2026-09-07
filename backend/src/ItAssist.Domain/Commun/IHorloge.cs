namespace ItAssist.Domain.Commun;

/// <summary>
/// Source de temps injectée. Le domaine n'appelle jamais DateTime.Now ni
/// DateTime.UtcNow directement : les règles de délai doivent être testables.
/// </summary>
public interface IHorloge
{
    DateTime Maintenant { get; }
}

public sealed class HorlogeSysteme : IHorloge
{
    public DateTime Maintenant => DateTime.UtcNow;
}
