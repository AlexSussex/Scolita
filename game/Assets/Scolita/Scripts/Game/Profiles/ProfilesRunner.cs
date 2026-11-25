using System.Collections.Generic;
using System.Linq;
using Scolita.Content;
using Scolita.Core.Audio;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Parent;
using Scolita.Core.Profiles;
using Scolita.Core.Randomness;
using Scolita.Core.Voice;
using Scolita.Game.Audio;
using Scolita.Game.Lessons;
using Scolita.Game.Parent;
using Scolita.Game.Rewards;
using Scolita.Game.Saving;
using Scolita.Game.Scenes;
using Scolita.Game.Settings;
using Scolita.Game.Voice;
using UnityEngine;

namespace Scolita.Game.Profiles
{
    /// <summary>
    /// The Profiles scene's composition root, "Who's playing?" (design screen 9; AGENTS.md → "How the app is put
    /// together"): a card per child (never the tester), the mascot saying „Cine se joacă? Atinge poza ta.”; a tapped
    /// card opens that child's saves (<see cref="ProfileList.Play"/>), greets her by her name in her map's language
    /// (<see cref="ProfileLines.Hello"/>) and, once the greeting is said, opens her map. The parent button and gate open
    /// the Parent scene, as on the map. Android Back does nothing: this is where the app starts. The space theme plays
    /// under it, and a tapped card chimes (<see cref="SoundPlayer"/>).
    /// </summary>
    public sealed class ProfilesRunner : MonoBehaviour, IProfilesInput
    {
        [SerializeField] private DomainCatalog domains;
        [SerializeField] private LetterCatalog letters;
        [SerializeField] private WordCatalog words;
        [SerializeField] private PictureCatalog pictures;
        [SerializeField] private VoiceLineCatalog voiceLines;
        [SerializeField] private LetterShapeCatalog shapes;
        [SerializeField] private AvatarCatalog avatars;
        [SerializeField] private LearningRulesAsset learningRules;
        [SerializeField] private VoiceHost voice;
        [SerializeField] private SoundPlayer sounds;
        [SerializeField] private ProfilesScreenView screen;

        private readonly Dictionary<ProfileId, VoiceSequence> greetings = new Dictionary<ProfileId, VoiceSequence>();
        private ProfileStore profiles;
        private ProfileList list;
        private LessonVoice pickerVoice;
        private ParentGateFlow gate;
        private bool chosen;
        private bool leaving;

        /// <summary>The cards shown, in order; empty before the screen is built.</summary>
        public IReadOnlyList<ProfileCard> Cards { get; private set; } = new ProfileCard[0];

        /// <summary>The screen's voice; null before it is built.</summary>
        public LessonVoice PickerVoice => pickerVoice;

        /// <summary>The parent gate on this screen; null before it is built.</summary>
        public ParentGateFlow Gate => gate;

        /// <inheritdoc/>
        public void Choose(ProfileId child)
        {
            if (chosen || leaving || gate.IsAsking || !list.TryGet(child, out Profile profile) || !profile.IsChild)
            {
                return;
            }

            list.Play(child);
            if (!profiles.Save(list))
            {
                Debug.LogError("Who's playing: the child chosen could not be saved, so her map doesn't open (it would open "
                    + "the profile the saved list names).");
                list.StopPlaying();
                pickerVoice.SayAgain();
                return;
            }

            chosen = true;
            sounds.Play(SoundCue.CardTapped);
            screen.ShowChosen(child);
            pickerVoice.Say(greetings[child], null);
        }

        /// <inheritdoc/>
        public void RepeatLastLine()
        {
            if (!chosen)
            {
                pickerVoice?.SayAgain();
            }
        }

        private void Start()
        {
            Curriculum curriculum = CurriculumAssets.Build(domains, letters, words, pictures, voiceLines, shapes);
            voice.Begin(curriculum);
            TabletSettingsData tablet = SaveStore.ForTablet(SaveRoot.Path)
                .Load(TabletSettingsData.FileName, TabletSettingsData.CreateDefaults()).Data;
            Language appLanguage = tablet.AppLanguage;
            voice.SetVolume(tablet.VoiceVolume);
            sounds.Begin(learningRules.ToSoundRules(), tablet, MusicPiece.Space);
            sounds.FollowVoice(voice.Voice);
            AppLocale.Select(appLanguage);
            profiles = ProfileStore.ForApp(avatars, learningRules);
            list = profiles.Load();
            List<ProfileCard> cards = new List<ProfileCard>();
            foreach (Profile child in list.Children)
            {
                SaveStore files = profiles.FilesOf(child);
                LanguageMix mix = files.Load(LearningSettingsData.FileName, LearningSettingsData.CreateDefaults()).Data.LanguageMix;
                greetings.Add(child.Id, ProfileLines.Hello(child, ProfileLines.LanguageFor(mix, appLanguage)));
                int stars = files.Load(RewardsData.FileName, RewardsData.CreateDefaults()).Data.ToBook().StarsEarned;
                cards.Add(new ProfileCard(child.Id, avatars.SpriteOf(child.Avatar), child.Name.Text, stars,
                    child.Id == list.LastChild));
            }

            Cards = cards;
            VoiceSequence whoPlays = ProfileLines.WhoPlays(appLanguage);
            voice.LoadLines(ProfileLines.LineIds(Language.Romanian).Concat(ProfileLines.LineIds(Language.English)));
            pickerVoice = new LessonVoice(voice.Voice, screen.Bubble);
            pickerVoice.Prepare(curriculum, greetings.Values.Append(whoPlays));
            voice.PartStarted += OnPartStarted;
            screen.Bind(this);
            screen.Show(cards);
            ParentRules parentRules = learningRules.ToParentRules();
            gate = new ParentGateFlow(new ParentGate(parentRules, new SeededRandom((ulong)System.DateTime.UtcNow.Ticks)),
                voice.Voice, screen.ParentGate, appLanguage, pickerVoice.Silence, OpenParentArea);
            screen.ParentGate.Bind(gate);
            pickerVoice.Say(whoPlays, null);
        }

        private void Update()
        {
            if (pickerVoice == null)
            {
                return;
            }

            gate.Tick(Time.unscaledDeltaTime);
            if (AndroidBack.WasPressed() && gate.IsAsking)
            {
                gate.Close();
            }

            pickerVoice.Tick();
            screen.SetTalking(pickerVoice.IsSpeaking);
            if (chosen && !leaving && !pickerVoice.IsBusy)
            {
                leaving = true;
                SceneNavigator.Load(AppScene.Map);
            }
        }

        private void OnApplicationPause(bool paused)
        {
            if (pickerVoice == null)
            {
                return;
            }

            if (paused)
            {
                gate.Release();
                pickerVoice.Pause();
            }
            else
            {
                pickerVoice.Resume();
            }
        }

        private void OnDestroy()
        {
            if (pickerVoice != null)
            {
                voice.PartStarted -= OnPartStarted;
            }
        }

        private void OnPartStarted(VoiceUtterance part)
        {
            gate.PartStarted(part);
            pickerVoice.PartStarted();
        }

        private void OpenParentArea()
        {
            if (leaving || chosen)
            {
                return;
            }

            leaving = true;
            SceneNavigator.Load(AppScene.Parent);
        }
    }
}
