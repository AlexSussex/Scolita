using System.Collections.Generic;
using Scolita.Core.Profiles;
using Scolita.Game.Parent;
using Scolita.Game.Voice;
using UnityEngine;

namespace Scolita.Game.Profiles
{
    /// <summary>
    /// "Who's playing?" as <see cref="ProfilesRunner"/> sees it (design screen 9): the mascot and its bubble, a card per
    /// child, the parent button and its gate. Drawn by <c>Scolita.UI.Profiles.ProfilesScreen</c> on
    /// <c>Prefabs/Profiles/ProfilesScreen.prefab</c>.
    /// </summary>
    public abstract class ProfilesScreenView : MonoBehaviour
    {
        /// <summary>The mascot's bubble, which shows every line said, in its words.</summary>
        public abstract SpeechBubbleView Bubble { get; }

        /// <summary>The parent button and the parent gate's panel.</summary>
        public abstract ParentGateView ParentGate { get; }

        /// <summary>Sends the screen's taps to <paramref name="input"/>; called once by <see cref="ProfilesRunner"/>.</summary>
        public abstract void Bind(IProfilesInput input);

        /// <summary>Draws a card per child of <paramref name="cards"/>, in their order; called once by <see cref="ProfilesRunner"/>.</summary>
        public abstract void Show(IReadOnlyList<ProfileCard> cards);

        /// <summary>Lifts the card of <paramref name="child"/>, the one tapped, and dims the others; called by <see cref="ProfilesRunner"/>.</summary>
        public abstract void ShowChosen(ProfileId child);

        /// <summary>Moves the mascot's mouth while it speaks; called every frame by <see cref="ProfilesRunner"/>.</summary>
        public abstract void SetTalking(bool talking);
    }
}
