namespace Supervertaler.Trados.Licensing
{
    /// <summary>
    /// Represents the active license tier.
    /// Ordered so that higher tiers have higher numeric values.
    /// </summary>
    public enum LicenseTier
    {
        /// <summary>No valid license — trial expired, subscription lapsed.</summary>
        None = 0,

        /// <summary>14-day free trial — grants full access (TermLens + Assistant).</summary>
        Trial = 1,

        /// <summary>TermLens only — terminology panel features.</summary>
        Tier1 = 2,

        /// <summary>TermLens + Supervertaler Assistant — all features.</summary>
        Tier2 = 3,

        /// <summary>Supervertaler Assistant only — AI features without the TermLens panel.</summary>
        AssistantOnly = 4,
    }
}
