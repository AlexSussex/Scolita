using System.Collections.Generic;

namespace Scolita.Core.Content
{
    /// <summary>
    /// What a step of one activity kind needs from the content, checked by the content rules: how
    /// many letters and words, whether its words need pictures or are shown to read, whether it
    /// bridges both languages, whether its letters are choices (and so must all be taught), its
    /// options, and the instruction and hint lines it speaks (<c>&lt;language&gt;-line-&lt;key&gt;</c>).
    /// One row of <see cref="ActivityKinds"/>.
    /// </summary>
    public sealed class ActivityKindRequirements
    {
        /// <summary>Creates a kind's requirements; called by <see cref="ActivityKinds"/>.</summary>
        public ActivityKindRequirements(ActivityKind kind, ItemRange letters, ItemRange words, ItemRange items,
            ActivityItemRules itemRules, ActivityWordUse wordUse, IReadOnlyList<VoiceLineKind> wordLines, bool isBridge,
            bool lettersAreChoices, IReadOnlyList<ActivityOption> options, IReadOnlyList<string> instructionLines,
            IReadOnlyList<string> hintLines)
        {
            ItemRules = itemRules;
            Kind = kind;
            Letters = letters;
            Words = words;
            Items = items;
            WordUse = wordUse;
            WordLines = wordLines;
            IsBridge = isBridge;
            Options = options;
            LettersAreChoices = lettersAreChoices;
            InstructionLines = instructionLines;
            HintLines = hintLines;
        }

        /// <summary>The kind.</summary>
        public ActivityKind Kind { get; }

        /// <summary>How many letters a step takes.</summary>
        public ItemRange Letters { get; }

        /// <summary>How many words a step takes.</summary>
        public ItemRange Words { get; }

        /// <summary>How many letters and words together a step takes.</summary>
        public ItemRange Items { get; }

        /// <summary>What a step's letters and words must be to each other.</summary>
        public ActivityItemRules ItemRules { get; }

        /// <summary>How the kind shows its words.</summary>
        public ActivityWordUse WordUse { get; }

        /// <summary>
        /// The lines each of its words must have besides its word line (<see cref="VoiceLineKind.Syllables"/>
        /// for <see cref="ActivityKind.Syllables"/>).
        /// </summary>
        public IReadOnlyList<VoiceLineKind> WordLines { get; }

        /// <summary>
        /// Whether a step of the kind is in both languages at once (<see cref="ActivityKind.TwoSounds"/>,
        /// <see cref="ActivityKind.WordTwins"/>): its letters need both languages, its words may be in
        /// either, and it is skipped unless both languages are learned.
        /// </summary>
        public bool IsBridge { get; }

        /// <summary>
        /// Whether a step's letters are the choices she taps among (<see cref="ActivityKind.ListenAndPick"/>,
        /// <see cref="ActivityKind.LetterHunt"/>, <see cref="ActivityKind.FirstSound"/>),
        /// so every one of them must be taught by the step's lesson in its language: a wrong choice is
        /// never a letter still to come (the human, 2026-11-24).
        /// </summary>
        public bool LettersAreChoices { get; }

        /// <summary>The options a step of the kind takes.</summary>
        public IReadOnlyList<ActivityOption> Options { get; }

        /// <summary>The keys of the instruction lines it speaks, in each of a step's languages (<c>pick-letter</c>).</summary>
        public IReadOnlyList<string> InstructionLines { get; }

        /// <summary>
        /// The keys of the lines its hints and its shown answers speak, in each of a step's languages
        /// (<c>pick-hint-show</c>); checked by the content rules like its instruction lines.
        /// </summary>
        public IReadOnlyList<string> HintLines { get; }
    }
}
