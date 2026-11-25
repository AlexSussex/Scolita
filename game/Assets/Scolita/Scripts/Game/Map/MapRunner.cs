using Scolita.Content;
using Scolita.Core.Content;
using Scolita.Core.Languages;
using Scolita.Core.Lessons;
using Scolita.Core.Map;
using Scolita.Core.Parent;
using Scolita.Core.Profiles;
using Scolita.Core.Progress;
using Scolita.Core.Randomness;
using Scolita.Core.Voice;
using Scolita.Game.Lessons;
using Scolita.Game.Parent;
using Scolita.Game.Profiles;
using Scolita.Game.Progress;
using Scolita.Game.Rewards;
using Scolita.Game.Saving;
using Scolita.Game.Scenes;
using Scolita.Game.Settings;
using Scolita.Game.Timing;
using Scolita.Game.Voice;
using UnityEngine;

namespace Scolita.Game.Map
{
    /// <summary>
    /// The Map scene's composition root (AGENTS.md → "How the app is put together"), the only place the
    /// galaxy map is wired together. It builds the curriculum, the voice and Core's <see cref="MapRoute"/>
    /// from the settings and the <c>progress</c> (<see cref="ProgressFile"/>) of the profile to open
    /// (<see cref="ProfileList.ProfileToOpen"/>; the tester's map opens every stop), has the screen draw it and bring the rocket (the trip
    /// after a finished planet), says the mascot's lines (<see cref="MapLines"/>), and opens a lesson: Go's,
    /// or a stop she taps, written into <c>lesson-run.json</c> for the Lesson scene unless that already names
    /// it (so a lesson left half-way resumes at its step). It also counts her play time for the daily limit
    /// (<see cref="PlayClock"/>): at the limit the rocket rests and no lesson opens; and it runs the parent gate
    /// (<see cref="ParentGateFlow"/>), which opens the Parent scene. Android Back closes the gate or an open book.
    /// </summary>
    public sealed class MapRunner : MonoBehaviour, IMapInput
    {
        [SerializeField] private DomainCatalog domains;
        [SerializeField] private LetterCatalog letters;
        [SerializeField] private WordCatalog words;
        [SerializeField] private PictureCatalog pictures;
        [SerializeField] private VoiceLineCatalog voiceLines;
        [SerializeField] private LetterShapeCatalog shapes;
        [SerializeField] private ActivityRegistry registry;
        [SerializeField] private VoiceHost voice;
        [SerializeField] private MapScreenView screen;
        [SerializeField] private StickerCatalog stickers;
        [SerializeField] private ConstellationCatalog constellations;
        [SerializeField] private TrailCatalog trails;
        [SerializeField] private RewardRulesAsset rewardRules;
        [SerializeField] private LearningRulesAsset learningRules;
        [SerializeField] private AvatarCatalog avatars;

        private SaveStore profileFiles;
        private LearnerProgress progress;
        private MapRoute route;
        private MapLines lines;
        private LessonVoice mapVoice;
        private RewardsSession rewards;
        private PlayClock playClock;
        private SessionLimit limit;
        private ParentGateFlow gate;
        private bool resting;
        private bool opening;

        /// <summary>The map as drawn; null before it is built, or when the content has no domain.</summary>
        public MapRoute Route => route;

        /// <summary>The mascot's lines on this map; null before it is built.</summary>
        public MapLines Lines => lines;

        /// <summary>The map's voice; null before it is built.</summary>
        public LessonVoice MapVoice => mapVoice;

        /// <summary>Her rewards on the map (the bottom bar and its books); null before it is built.</summary>
        public RewardsSession Rewards => rewards;

        /// <summary>The parent gate on this map; null before it is built.</summary>
        public ParentGateFlow Gate => gate;

        /// <summary>Whether the rocket rests (the daily limit is reached): no lesson opens.</summary>
        public bool IsResting => resting;

        /// <inheritdoc/>
        public void Go()
        {
            if (route?.GoStop == null || screen.IsTripPlaying)
            {
                return;
            }

            if (resting)
            {
                mapVoice.Say(lines.Resting, null);
                return;
            }

            Open(route.GoStop);
        }

        /// <inheritdoc/>
        public void TapStop(MapStop stop)
        {
            if (route == null || screen.IsTripPlaying)
            {
                return;
            }

            if (stop.CanOpen && resting)
            {
                mapVoice.Say(lines.Resting, null);
                return;
            }

            if (stop.CanOpen)
            {
                Open(stop);
                return;
            }

            screen.Wobble(stop);
            mapVoice.Say(route.Planets[stop.PlanetIndex].IsAsleep ? lines.PlanetAsleep : lines.StopAhead, null);
        }

        /// <inheritdoc/>
        public void TapPlanet(MapPlanet planet)
        {
            if (route == null || screen.IsTripPlaying)
            {
                return;
            }

            if (!planet.IsAsleep)
            {
                mapVoice.Say(lines.Greeting, null);
                return;
            }

            screen.Wobble(planet);
            mapVoice.Say(lines.PlanetAsleep, null);
        }

        /// <inheritdoc/>
        public void RepeatLastLine()
        {
            mapVoice?.SayAgain();
        }

        /// <inheritdoc/>
        public void TripEnded()
        {
            if (route == null)
            {
                return;
            }

            RocketArrived();
            mapVoice.Say(resting ? lines.Resting : lines.Landed, null);
        }

        private void Start()
        {
            Curriculum curriculum = CurriculumAssets.Build(domains, letters, words, pictures, voiceLines, shapes);
            voice.Begin(curriculum);
            string saveRoot = Application.persistentDataPath;
            TabletSettingsData tablet = SaveStore.ForTablet(saveRoot)
                .Load(TabletSettingsData.FileName, TabletSettingsData.CreateDefaults()).Data;
            Language appLanguage = tablet.AppLanguage;
            voice.SetVolume(tablet.VoiceVolume);
            AppLocale.Select(appLanguage);
            ProfileStore profiles = ProfileStore.ForApp(avatars, learningRules);
            Profile profile = profiles.Load().ProfileToOpen;
            profileFiles = profiles.FilesOf(profile);
            LearningSettingsData learning = profileFiles.Load(LearningSettingsData.FileName,
                LearningSettingsData.CreateDefaults()).Data;
            LanguageMix mix = learning.LanguageMix;
            ParentRules parentRules = learningRules.ToParentRules();
            limit = learning.SessionLimitBy(parentRules);
            playClock = new PlayClock(profileFiles, new SystemClock());
            resting = playClock.HasReached(limit);
            if (curriculum.Domains.Count == 0)
            {
                Debug.LogError("Map: the content has no domain, so there is no planet to show.");
                return;
            }

            Domain domain = curriculum.Domains[0];
            LessonOrder order = new LessonOrder(domain, mix, new PlayableSteps(registry.PlayableKinds(), curriculum));
            progress = ProgressFile.Load(profileFiles, curriculum, order);
            route = new MapRoute(curriculum, domain, order, progress, profile.OpensEveryLesson);
            if (route.GoStop == null)
            {
                Debug.LogError("Map: no lesson has a step that can be played yet.");
            }

            lines = new MapLines(route, MapLines.LanguageFor(route, mix, appLanguage));
            voice.LoadLines(lines.SpokenLines);
            mapVoice = new LessonVoice(voice.Voice, screen.Bubble);
            mapVoice.Prepare(curriculum, lines.All);
            voice.PartStarted += OnPartStarted;
            screen.Bind(this);
            screen.Show(route);
            gate = new ParentGateFlow(new ParentGate(parentRules, new SeededRandom((ulong)System.DateTime.UtcNow.Ticks)),
                voice.Voice, screen.ParentGate, appLanguage, mapVoice.Silence, OpenParentArea);
            screen.ParentGate.Bind(gate);
            if (resting)
            {
                screen.ShowResting();
            }

            ShowRewards(curriculum, order);
            Arrive(RocketMove.Between(route, progress.Lessons.RocketStop));
        }

        private void Update()
        {
            if (mapVoice == null)
            {
                return;
            }

            gate.Tick(Time.unscaledDeltaTime);
            if (AndroidBack.WasPressed())
            {
                Back();
            }

            mapVoice.Tick();
            screen.SetTalking(mapVoice.IsSpeaking);
            if (resting)
            {
                return;
            }

            playClock.Tick(Time.unscaledDeltaTime);
            if (playClock.HasReached(limit))
            {
                Rest();
            }
        }

        private void OnApplicationPause(bool paused)
        {
            if (mapVoice == null)
            {
                return;
            }

            if (paused)
            {
                gate.Release();
                playClock.Save();
                mapVoice.Pause();
            }
            else
            {
                mapVoice.Resume();
            }
        }

        private void OnDestroy()
        {
            if (mapVoice != null)
            {
                voice.PartStarted -= OnPartStarted;
                playClock.Save();
            }
        }

        private void ShowRewards(Curriculum curriculum, LessonOrder order)
        {
            RewardSources sources = RewardSources.Build(stickers, constellations, trails, rewardRules);
            rewards = new RewardsSession(sources, profileFiles, order, progress, curriculum, mapVoice, lines.Language,
                screen.Rewards, new SeededRandom((ulong)System.DateTime.UtcNow.Ticks));
            voice.LoadLines(rewards.SpokenLines);
            rewards.ShowOnMap();
        }

        private void Arrive(RocketMove move)
        {
            screen.MoveRocket(move);
            if (move.Kind == RocketMoveKind.Trip)
            {
                return;
            }

            RocketArrived();
            mapVoice.Say(resting ? lines.Resting : lines.Greeting, null);
        }

        private void RocketArrived()
        {
            if (route.GoStop == null)
            {
                return;
            }

            progress.Lessons.ShowRocketAt(route.GoStop.Lesson.Id);
            ProgressFile.Save(profileFiles, progress);
        }

        private void Open(MapStop stop)
        {
            if (opening)
            {
                return;
            }

            Lesson lesson = stop.Lesson;
            LessonRunData saved = profileFiles.Load(LessonRunData.FileName, LessonRunData.CreateDefaults()).Data;
            bool alreadySaved = saved.TryGetLessonId(out ContentId savedId) && savedId == lesson.Id;
            if (!alreadySaved
                && !profileFiles.Save(LessonRunData.FileName, new LessonRunData(lesson.Id, 1, LessonSignature.Of(lesson))))
            {
                Debug.LogError("Map: " + lesson.Id + " could not be saved as the lesson to open, so it isn't opened.");
                return;
            }

            opening = true;
            SceneNavigator.Load(AppScene.Lesson);
        }

        private void OnPartStarted(VoiceUtterance part)
        {
            gate.PartStarted(part);
            mapVoice.PartStarted();
        }

        private void Rest()
        {
            resting = true;
            playClock.Save();
            screen.ShowResting();
            if (!gate.IsAsking)
            {
                mapVoice.Say(lines.Resting, null);
            }
        }

        private void Back()
        {
            if (gate.IsAsking)
            {
                gate.Close();
                return;
            }

            if (rewards.IsBookOpen)
            {
                rewards.CloseBook();
            }
        }

        private void OpenParentArea()
        {
            if (opening)
            {
                return;
            }

            opening = true;
            playClock.Save();
            SceneNavigator.Load(AppScene.Parent);
        }
    }
}
