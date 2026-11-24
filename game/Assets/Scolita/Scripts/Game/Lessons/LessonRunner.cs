using Scolita.Content;
using Scolita.Core.Content;
using Scolita.Game.Voice;
using UnityEngine;

namespace Scolita.Game.Lessons
{
    /// <summary>
    /// The Lesson scene's composition root: it will build a lesson's run from the chosen lesson and
    /// the profile (<c>lesson-engine</c>). For now it builds the curriculum from the catalogs, starts
    /// the scene's voice and binds the voice sampler's letters and words to it, so both voices can be
    /// heard; and it keeps the screen awake while the Lesson scene is open, and only then, so the
    /// tablet can sleep on the map when she walks away.
    /// </summary>
    public sealed class LessonRunner : MonoBehaviour
    {
        [SerializeField] private DomainCatalog domains;
        [SerializeField] private LetterCatalog letters;
        [SerializeField] private WordCatalog words;
        [SerializeField] private PictureCatalog pictures;
        [SerializeField] private VoiceLineCatalog voiceLines;
        [SerializeField] private VoiceHost voice;
        [SerializeField] private Hearable[] voiceSampler = new Hearable[0];

        private void Start()
        {
            Curriculum curriculum = CurriculumAssets.Build(domains, letters, words, pictures, voiceLines);
            voice.Begin(curriculum);
            foreach (Hearable hearable in voiceSampler)
            {
                hearable.Bind(voice.Voice);
            }
        }

        private void OnEnable()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        private void OnDisable()
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
    }
}
