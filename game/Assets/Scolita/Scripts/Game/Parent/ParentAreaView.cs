using Scolita.Core.Parent;
using UnityEngine;

namespace Scolita.Game.Parent
{
    /// <summary>
    /// The parent area as <see cref="ParentRunner"/> sees it (design screen 7): "Ce a învățat" and "Setări", and the
    /// reset's two questions. Drawn by <c>Scolita.UI.Parent.ParentAreaScreen</c> on
    /// <c>Prefabs/Parent/ParentArea.prefab</c>.
    /// </summary>
    public abstract class ParentAreaView : MonoBehaviour
    {
        /// <summary>Sends the screen's taps to <paramref name="input"/>; called once by <see cref="ParentRunner"/>.</summary>
        public abstract void Bind(IParentAreaInput input);

        /// <summary>
        /// Draws <paramref name="report"/>, <paramref name="settings"/> and the app's <paramref name="version"/>, in the
        /// selected locale's words. Called by <see cref="ParentRunner"/> when the area opens and after every change.
        /// </summary>
        public abstract void Show(ParentReport report, ParentSettingsShown settings, string version);

        /// <summary>
        /// Shows, in the area's header, the chosen profile: <paramref name="avatar"/> and <paramref name="name"/> (empty for
        /// none; "TEST" for the tester, <paramref name="isTester"/>). Called by <see cref="ParentRunner"/> with every redraw.
        /// </summary>
        public abstract void ShowChosen(Sprite avatar, bool isTester, string name);

        /// <summary>Shows <paramref name="question"/>, or hides the questions for <see cref="ResetQuestion.None"/>; called by <see cref="ParentRunner"/>.</summary>
        public abstract void AskReset(ResetQuestion question);

        /// <summary>
        /// Says, under the reset button, that the progress was deleted (<paramref name="deleted"/>), or that it could not
        /// all be deleted and to try again; called by <see cref="ParentRunner"/>.
        /// </summary>
        public abstract void ShowResetDone(bool deleted);
    }
}
