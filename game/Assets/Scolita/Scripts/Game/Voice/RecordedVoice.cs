using System.Collections.Generic;
using Scolita.Content;
using Scolita.Core.Content;
using Scolita.Core.Voice;
using UnityEngine;

namespace Scolita.Game.Voice
{
    /// <summary>
    /// The recorded clips (<see cref="IClipVoice"/>), played through one <see cref="AudioSource"/>.
    /// The clips come from the voice line assets; they are imported with their audio data left
    /// unloaded, so <see cref="LoadLines"/> loads a lesson's clips when it starts and
    /// <see cref="UnloadAll"/> frees them after. A clip played outside a loaded lesson is loaded then.
    /// </summary>
    public sealed class RecordedVoice : IClipVoice
    {
        private readonly AudioSource source;
        private readonly Dictionary<string, AudioClip> clipsByName = new Dictionary<string, AudioClip>();
        private readonly Dictionary<string, IReadOnlyList<AudioClip>> clipsByLine = new Dictionary<string, IReadOnlyList<AudioClip>>();
        private readonly HashSet<AudioClip> loaded = new HashSet<AudioClip>();

        /// <summary>Creates the voice playing through <paramref name="source"/> the clips of <paramref name="voiceLines"/>; called by <see cref="VoiceHost"/>.</summary>
        public RecordedVoice(AudioSource source, VoiceLineCatalog voiceLines)
        {
            this.source = source;
            foreach (VoiceLineAsset line in voiceLines.Items)
            {
                IReadOnlyList<string> names = line.ToVoiceLine().Clips;
                if (line.AudioClips.Count != names.Count)
                {
                    continue;
                }

                for (int index = 0; index < names.Count; index++)
                {
                    clipsByName[names[index]] = line.AudioClips[index];
                }

                clipsByLine[line.Id] = line.AudioClips;
            }
        }

        /// <inheritdoc/>
        public bool IsPlaying => source.isPlaying;

        /// <inheritdoc/>
        public bool Has(string clip)
        {
            return clipsByName.TryGetValue(clip, out AudioClip audio) && audio != null;
        }

        /// <inheritdoc/>
        public bool Play(string clip)
        {
            if (!clipsByName.TryGetValue(clip, out AudioClip audio) || audio == null || !EnsureLoaded(audio))
            {
                return false;
            }

            source.Stop();
            source.clip = audio;
            source.Play();
            return true;
        }

        /// <inheritdoc/>
        public void Stop()
        {
            source.Stop();
        }

        /// <summary>
        /// Loads the clips of <paramref name="lines"/> (a lesson's, <see cref="LessonVoiceLines"/>), so
        /// they play at once. Called by the Lesson scene's composition root when a lesson starts.
        /// </summary>
        public void LoadLines(IEnumerable<VoiceLineId> lines)
        {
            foreach (VoiceLineId line in lines)
            {
                if (!clipsByLine.TryGetValue(line.ToString(), out IReadOnlyList<AudioClip> clips))
                {
                    continue;
                }

                foreach (AudioClip clip in clips)
                {
                    EnsureLoaded(clip);
                }
            }
        }

        /// <summary>Frees every clip's audio data; called by <see cref="VoiceHost"/> when its scene closes, and when a lesson ends.</summary>
        public void UnloadAll()
        {
            source.Stop();
            source.clip = null;
            foreach (AudioClip clip in loaded)
            {
                clip.UnloadAudioData();
            }

            loaded.Clear();
        }

        private bool EnsureLoaded(AudioClip clip)
        {
            if (clip.loadState != AudioDataLoadState.Loaded && !clip.LoadAudioData())
            {
                Debug.LogWarning("Voice: the clip " + clip.name + " could not be loaded; the system voice says its line.");
                return false;
            }

            loaded.Add(clip);
            return clip.loadState != AudioDataLoadState.Failed;
        }
    }
}
