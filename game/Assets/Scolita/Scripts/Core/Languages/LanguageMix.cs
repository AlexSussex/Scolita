namespace Scolita.Core.Languages
{
    /// <summary>
    /// Which languages a profile learns in: both (Romanian as the anchor, English beside it), or
    /// only one of them. A learning setting of each profile.
    /// </summary>
    public enum LanguageMix
    {
        Both,
        RomanianOnly,
        EnglishOnly
    }
}
