using Scolita.Core.Timing;
using Scolita.Game.Profiles;
using TMPro;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UI;

namespace Scolita.UI.Profiles
{
    /// <summary>
    /// The tester's strip (<c>Prefabs/Profiles/TesterStrip.prefab</c>, its own canvas over the screen): a band at the
    /// bottom with a "TEST" badge, the test information and, in a lesson, "Sari peste pas" (64 px). Hidden for a child.
    /// For the performance pass it also shows, once a second, the frames per second, the slowest frame and the most GC
    /// allocated in a frame (Core's <see cref="FrameStats"/>; "GC -" where the player can't count it), written without
    /// allocating, so they can be read on the tablet with no tools.
    /// </summary>
    public sealed class TesterStrip : TesterStripView
    {
        [SerializeField] private GameObject strip;
        [SerializeField] private TMP_Text info;
        [SerializeField] private Button skipButton;
        [SerializeField] private TMP_Text frames;
        [SerializeField] private float framesWindowSeconds = 1f;

        private const string FramesFormat = "{0:0} fps · max {1:0} ms · GC {2:0} B";
        private const string FramesWithoutGcFormat = "{0:0} fps · max {1:0} ms · GC -";
        private const string AllocatedInFrame = "GC Allocated In Frame";

        private ITesterInput input;
        private FrameStats stats;
        private ProfilerRecorder allocated;

        /// <summary>Whether the strip is shown.</summary>
        public bool IsShown => strip.activeSelf;

        /// <summary>The test information shown.</summary>
        public string Info => info.text;

        /// <summary>"Sari peste pas".</summary>
        public Button SkipButton => skipButton;

        /// <summary>The frames' numbers shown (empty until a second has passed with the strip shown).</summary>
        public string Frames => frames.text;

        /// <inheritdoc/>
        public override void Bind(ITesterInput testerInput)
        {
            input = testerInput;
        }

        /// <inheritdoc/>
        public override void Show(bool shown, bool canSkip)
        {
            strip.SetActive(shown);
            skipButton.gameObject.SetActive(shown && canSkip);
        }

        /// <inheritdoc/>
        public override void ShowInfo(string text)
        {
            info.SetText(text);
        }

        private void Awake()
        {
            stats = new FrameStats(framesWindowSeconds);
            frames.SetText(string.Empty);
            strip.SetActive(false);
            skipButton.onClick.AddListener(OnSkip);
        }

        private void OnDestroy()
        {
            skipButton.onClick.RemoveListener(OnSkip);
        }

        private void OnEnable()
        {
            allocated = ProfilerRecorder.StartNew(ProfilerCategory.Memory, AllocatedInFrame);
        }

        private void OnDisable()
        {
            allocated.Dispose();
        }

        private void Update()
        {
            if (!strip.activeSelf)
            {
                return;
            }

            long bytes = allocated.Valid ? allocated.LastValue : 0L;
            if (!stats.Add(Time.unscaledDeltaTime, bytes))
            {
                return;
            }

            if (allocated.Valid)
            {
                frames.SetText(FramesFormat, stats.FramesPerSecond, stats.SlowestFrameMilliseconds, stats.MostAllocatedInAFrame);
            }
            else
            {
                frames.SetText(FramesWithoutGcFormat, stats.FramesPerSecond, stats.SlowestFrameMilliseconds);
            }
        }

        private void OnSkip()
        {
            input?.SkipStep();
        }
    }
}
