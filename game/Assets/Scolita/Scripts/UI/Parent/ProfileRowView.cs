using System;
using System.Globalization;
using Scolita.Core.Profiles;
using Scolita.Game.Parent;
using Scolita.Game.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Parent
{
    /// <summary>
    /// A profile's row on the parent area's profiles page (design screen 10): its avatar, its name (or "Fără nume"), its
    /// kind and summary, the "TEST" badge for the tester, outlined in violet when it is the chosen one, and "Schimbă".
    /// The whole row is the button.
    /// </summary>
    public sealed class ProfileRowView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image avatar;
        [SerializeField] private TMP_Text nameLabel;
        [SerializeField] private TMP_Text detail;
        [SerializeField] private GameObject testBadge;
        [SerializeField] private GameObject chosenOutline;

        /// <summary>The profile of the row.</summary>
        public ProfileId Id { get; private set; }

        /// <summary>The row's button.</summary>
        public Button Button => button;

        /// <summary>The name shown.</summary>
        public string NameShown => nameLabel.text;

        /// <summary>The kind and summary shown.</summary>
        public string DetailShown => detail.text;

        /// <summary>Whether the row is outlined as the chosen one.</summary>
        public bool IsChosen => chosenOutline.activeSelf;

        /// <summary>Draws <paramref name="row"/> in the app's language; called by <see cref="ParentProfilesScreen"/>.</summary>
        public void Show(ParentProfileRow row)
        {
            Profile profile = row.Profile;
            Id = profile.Id;
            avatar.sprite = row.Avatar;
            bool tester = profile.Kind == ProfileKind.Tester;
            nameLabel.SetText(tester ? AppLocale.Text("profile-tester-name")
                : profile.Name.IsNone ? AppLocale.Text("profile-no-name") : profile.Name.Text);
            detail.SetText((tester ? AppLocale.Text("profile-kind-tester") : AppLocale.Text("profile-kind-child")) + " · "
                + AppLocale.Text("profile-learned", row.Summary.LearnedRomanian, row.Summary.LearnedEnglish) + " · "
                + LastPlayed(row));
            testBadge.SetActive(tester);
            chosenOutline.SetActive(row.IsChosen);
        }

        private static string LastPlayed(ParentProfileRow row)
        {
            switch (row.DaysSinceLastPlayed)
            {
                case -1:
                    return AppLocale.Text("profile-never-played");
                case 0:
                    return AppLocale.Text("profile-played-today");
                case 1:
                    return AppLocale.Text("profile-played-yesterday");
                default:
                    return AppLocale.Text("profile-played-on", new DateTime(row.Summary.LastPlayed.DayNumber * TimeSpan.TicksPerDay)
                        .ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
            }
        }
    }
}
