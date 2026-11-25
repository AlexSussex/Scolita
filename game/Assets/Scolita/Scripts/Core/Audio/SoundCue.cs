namespace Scolita.Core.Audio
{
    /// <summary>
    /// Every sound effect the app plays (AGENTS.md → "Audio"), named by what happens, never by its clip: the
    /// Unity side's sound bank gives each its clips. Calm and kind: a miss is a soft <see cref="Boing"/>, never a buzzer.
    /// </summary>
    public enum SoundCue
    {
        /// <summary>A soft tick: a tap that isn't an answer, a stop or a planet that opens.</summary>
        Tap,

        /// <summary>A right answer, and the step goes on.</summary>
        Sparkle,

        /// <summary>A right answer that finishes the step.</summary>
        StepDone,

        /// <summary>A miss: a soft, low "boing".</summary>
        Boing,

        /// <summary>The answer shown after the second miss: a gentle bell.</summary>
        Glow,

        /// <summary>A beat on <c>Syllables</c>' drum.</summary>
        Drum,

        /// <summary>A planet asleep or a stop ahead, tapped: a soft "not yet".</summary>
        NotYet,

        /// <summary>The rocket leaves the map for a lesson.</summary>
        TakeOff,

        /// <summary>The rocket's trip to the next planet begins.</summary>
        Whoosh,

        /// <summary>The rocket lands after its trip.</summary>
        Landing,

        /// <summary>A star of a finished lesson lands in its constellation.</summary>
        StarLands,

        /// <summary>The gift box opens on a sticker.</summary>
        GiftOpens,

        /// <summary>A sticker is placed in the sticker book.</summary>
        StickerPlaced,

        /// <summary>A child's card is tapped on "Who's playing?".</summary>
        CardTapped,

        /// <summary>A book (the sky, the stickers, the trails) opens.</summary>
        BookOpens
    }
}
