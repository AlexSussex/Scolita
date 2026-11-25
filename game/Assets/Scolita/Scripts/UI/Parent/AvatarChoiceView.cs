using Scolita.Core.Content;
using Scolita.Game.Parent;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Parent
{
    /// <summary>
    /// An avatar in the profile's editor (design screen 10): its picture, outlined in violet when chosen, dimmed (and not
    /// tappable) when another child has it.
    /// </summary>
    public sealed class AvatarChoiceView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image picture;
        [SerializeField] private GameObject outline;
        [SerializeField] private CanvasGroup group;
        [SerializeField, Range(0f, 1f)] private float takenAlpha = 0.3f;

        /// <summary>The avatar's id.</summary>
        public ContentId Id { get; private set; }

        /// <summary>The avatar's button.</summary>
        public Button Button => button;

        /// <summary>Whether it is outlined as the chosen one.</summary>
        public bool IsChosen => outline.activeSelf;

        /// <summary>Whether it can be tapped (no other child has it).</summary>
        public bool IsFree => button.interactable;

        /// <summary>Draws <paramref name="choice"/>; called by <see cref="ParentProfilesScreen"/>.</summary>
        public void Show(AvatarChoice choice)
        {
            Id = choice.Id;
            picture.sprite = choice.Picture;
            outline.SetActive(choice.IsChosen);
            button.interactable = choice.IsFree;
            group.alpha = choice.IsFree ? 1f : takenAlpha;
        }
    }
}
