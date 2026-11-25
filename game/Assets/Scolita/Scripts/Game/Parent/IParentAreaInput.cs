using Scolita.Core.Languages;

namespace Scolita.Game.Parent
{
    /// <summary>Where the parent area's taps go; implemented by <see cref="ParentRunner"/>.</summary>
    public interface IParentAreaInput
    {
        /// <summary>Back: a reset question says no, else the map opens again. Called by the back button (and Android Back).</summary>
        void Back();

        /// <summary>A language mix pill was tapped. Called by the parent area screen.</summary>
        void ChooseMix(LanguageMix mix);

        /// <summary>A daily limit pill was tapped: <paramref name="minutes"/>, or 0 for none. Called by the parent area screen.</summary>
        void ChooseLimit(int minutes);

        /// <summary>The voice slider was let go at <paramref name="volume"/> (0 to 1). Called by the parent area screen.</summary>
        void SetVoiceVolume(float volume);

        /// <summary>The "Meniurile în engleză" switch was turned to <paramref name="english"/>. Called by the parent area screen.</summary>
        void SetMenusInEnglish(bool english);

        /// <summary>"Șterge progresul…" was tapped: the first question is asked. Called by the parent area screen.</summary>
        void AskReset();

        /// <summary>A reset question was answered: <paramref name="yes"/> goes on, no closes it. Called by the parent area screen.</summary>
        void AnswerReset(bool yes);

        /// <summary>The chosen profile's button in the header was tapped: the profiles page opens. Called by the parent area screen.</summary>
        void OpenProfiles();
    }
}
