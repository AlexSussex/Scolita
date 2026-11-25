using System.Collections.Generic;
using UnityEngine;

namespace Scolita.Game.Parent
{
    /// <summary>
    /// The parent area's profiles page as <see cref="ParentRunner"/> sees it (design screen 10): the list of profiles with
    /// the tester's row and "Adaugă un copil", the editor of the chosen profile or a new child, and a deletion's two
    /// questions. Drawn by <c>Scolita.UI.Parent.ParentProfilesScreen</c> on <c>Prefabs/Parent/ParentProfiles.prefab</c>,
    /// over the area.
    /// </summary>
    public abstract class ParentProfilesView : MonoBehaviour
    {
        /// <summary>Whether the page is open.</summary>
        public abstract bool IsOpen { get; }

        /// <summary>Sends the page's taps to <paramref name="input"/>; called once by <see cref="ParentRunner"/>.</summary>
        public abstract void Bind(IParentProfilesInput input);

        /// <summary>Opens the page (<paramref name="open"/>) or closes it; called by <see cref="ParentRunner"/>.</summary>
        public abstract void SetOpen(bool open);

        /// <summary>
        /// Draws the list: <paramref name="rows"/> in order (the tester's last, if any), the tester's buttons when there
        /// is one (else "Fă un profil de test"), and "Adaugă un copil" when <paramref name="canAddChild"/>. Called by
        /// <see cref="ParentRunner"/> after every change.
        /// </summary>
        public abstract void ShowList(IReadOnlyList<ParentProfileRow> rows, bool canAddChild);

        /// <summary>
        /// Draws the editor as <paramref name="editor"/> says; the name field is written only when
        /// <paramref name="writeName"/> (a profile newly shown), so typing is never disturbed. Called by
        /// <see cref="ParentRunner"/>.
        /// </summary>
        public abstract void ShowEditor(ProfileEditorShown editor, bool writeName);

        /// <summary>Shows <paramref name="question"/> about deleting the edited profile, or hides it; called by <see cref="ParentRunner"/>.</summary>
        public abstract void AskDelete(ResetQuestion question);

        /// <summary>Says <paramref name="message"/> under the buttons (none hides it); called by <see cref="ParentRunner"/>.</summary>
        public abstract void ShowMessage(ProfilesMessage message);
    }
}
