using System.Collections.Generic;
using UnityEngine;

namespace Scolita.Game.Parent
{
    /// <summary>
    /// The parent gate as <see cref="ParentGateFlow"/> sees it: the parent button's ring and the panel over the map
    /// with the numbers written as words. Drawn by <c>Scolita.UI.Parent.ParentGatePanel</c> (in
    /// <c>Prefabs/UI/MapScreen.prefab</c>), with the button from <c>Prefabs/Parent/ParentButton.prefab</c>.
    /// </summary>
    public abstract class ParentGateView : MonoBehaviour
    {
        /// <summary>Whether the panel is shown.</summary>
        public abstract bool IsAsking { get; }

        /// <summary>Sends the button's and the panel's touches to <paramref name="input"/>; called once by the map's composition root.</summary>
        public abstract void Bind(IParentGateInput input);

        /// <summary>Fills the button's ring to <paramref name="progress"/> (0 to 1); called every frame by <see cref="ParentGateFlow"/>.</summary>
        public abstract void SetHold(float progress);

        /// <summary>
        /// Shows the panel with <paramref name="words"/>, in their places; called by <see cref="ParentGateFlow"/> when the
        /// hold is full.
        /// </summary>
        public abstract void Ask(IReadOnlyList<string> words);

        /// <summary>
        /// Shows the number in digits under the question, when no voice can say it (so a parent is never locked out);
        /// called by <see cref="ParentGateFlow"/>.
        /// </summary>
        public abstract void ShowNumber(int number);

        /// <summary>Hides the panel, quietly; called by <see cref="ParentGateFlow"/>.</summary>
        public abstract void Hide();
    }
}
