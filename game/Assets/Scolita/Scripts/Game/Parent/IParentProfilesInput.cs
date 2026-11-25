using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Profiles;

namespace Scolita.Game.Parent
{
    /// <summary>Where the taps on the parent area's profiles page go (design screen 10); implemented by <see cref="ParentRunner"/>.</summary>
    public interface IParentProfilesInput
    {
        /// <summary>Back on the page: a question says no, else the page closes. Called by its back button (and Android Back).</summary>
        void CloseProfiles();

        /// <summary>A profile's row was tapped: it is the one chosen for the area, and the editor shows it. Called by the page.</summary>
        void ChooseProfile(ProfileId profile);

        /// <summary>"Adaugă un copil": the editor shows a new child. Called by the page.</summary>
        void StartNewChild();

        /// <summary>The name field now holds <paramref name="typed"/>. Called by the page.</summary>
        void TypeName(string typed);

        /// <summary>An avatar was tapped in the editor. Called by the page.</summary>
        void ChooseAvatar(ContentId avatar);

        /// <summary>A language mix pill was tapped in the editor. Called by the page.</summary>
        void ChooseEditedMix(LanguageMix mix);

        /// <summary>"Salvează" / "Adaugă": the editor's profile is saved. Called by the page.</summary>
        void SaveProfile();

        /// <summary>"Șterge profilul…": the first question is asked. Called by the page.</summary>
        void AskDelete();

        /// <summary>A deletion question was answered: <paramref name="yes"/> goes on, no closes it. Called by the page.</summary>
        void AnswerDelete(bool yes);

        /// <summary>"Fă un profil de test": the tester's profile is made. Called by the page.</summary>
        void MakeTester();

        /// <summary>"Joacă ca tester": the map opens as the tester. Called by the page.</summary>
        void PlayAsTester();

        /// <summary>"Șterge progresul de test": the tester's progress is deleted at once. Called by the page.</summary>
        void ResetTester();
    }
}
