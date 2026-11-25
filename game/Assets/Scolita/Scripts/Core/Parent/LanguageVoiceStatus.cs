using Scolita.Core.Languages;

namespace Scolita.Core.Parent
{
    /// <summary>How many of one language's voice lines are recorded, out of those to record.</summary>
    public readonly struct LanguageVoiceStatus
    {
        /// <summary><paramref name="recorded"/> of <paramref name="toRecord"/> lines in <paramref name="language"/>; called by <see cref="VoiceStatus"/>.</summary>
        public LanguageVoiceStatus(Language language, int recorded, int toRecord)
        {
            Language = language;
            Recorded = recorded;
            ToRecord = toRecord;
        }

        /// <summary>The language.</summary>
        public Language Language { get; }

        /// <summary>The lines with every clip they name.</summary>
        public int Recorded { get; }

        /// <summary>The lines that need a recording, recorded or not (a line said by its sounds needs none).</summary>
        public int ToRecord { get; }
    }
}
