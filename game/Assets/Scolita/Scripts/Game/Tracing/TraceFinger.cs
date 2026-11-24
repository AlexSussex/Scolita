using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Scolita.Game.Tracing
{
    /// <summary>
    /// Reads the finger drawing on the writing board through the Input System's Enhanced Touch API (every
    /// sample since the last frame, so a fast finger leaves no gaps), and the mouse when there is no touch
    /// (the Editor): the first finger down on the board draws, a second finger or a resting palm is ignored
    /// (<see cref="FingerPicker"/>). It sends the stroke to its <see cref="ITraceInput"/> in the board's
    /// local pixels, allocating nothing.
    /// </summary>
    public sealed class TraceFinger : MonoBehaviour
    {
        private const int MouseId = -2;

        [SerializeField] private RectTransform board;
        [SerializeField, Min(1f), Tooltip("A touch whose contact radius is larger than this, in dp, is a palm.")]
        private float palmRadius = 60f;

        private readonly FingerPicker picker = new FingerPicker();
        private ITraceInput input;

        /// <summary>
        /// Sends the strokes drawn on the board to <paramref name="traceInput"/> from now on (null to stop);
        /// called by the tracing view when its step is shown and hidden.
        /// </summary>
        public void Listen(ITraceInput traceInput)
        {
            input = traceInput;
            picker.Reset();
        }

        /// <summary>
        /// A touch <paramref name="touchId"/> came down at <paramref name="screen"/> with a contact radius of
        /// <paramref name="radius"/> screen pixels; it draws if it is the first finger on the board and not a
        /// palm. Called by the frame's touches and the PlayMode tests.
        /// </summary>
        public void Press(int touchId, Vector2 screen, float radius)
        {
            Down(touchId, screen, radius);
        }

        /// <summary>The touch <paramref name="touchId"/> moved to <paramref name="screen"/>; called by the frame's touches and the PlayMode tests.</summary>
        public void Move(int touchId, Vector2 screen)
        {
            Moved(touchId, screen);
        }

        /// <summary>
        /// The touch <paramref name="touchId"/> lifted (or was <paramref name="cancelled"/> by the system);
        /// called by the frame's touches and the PlayMode tests.
        /// </summary>
        public void Release(int touchId, bool cancelled)
        {
            Up(touchId, cancelled);
        }

        private void OnEnable()
        {
            EnhancedTouchSupport.Enable();
        }

        private void OnDisable()
        {
            EnhancedTouchSupport.Disable();
            picker.Reset();
        }

        private void Update()
        {
            if (input == null)
            {
                return;
            }

            if (Touch.activeTouches.Count > 0)
            {
                ReadTouches();
                return;
            }

            ReadMouse();
        }

        private void ReadTouches()
        {
            for (int index = 0; index < Touch.activeTouches.Count; index++)
            {
                Touch touch = Touch.activeTouches[index];
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        Down(touch.touchId, touch.startScreenPosition, touch.radius.magnitude);
                        MovedThroughHistory(touch);
                        break;
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        MovedThroughHistory(touch);
                        break;
                    case TouchPhase.Ended:
                        MovedThroughHistory(touch);
                        Up(touch.touchId, false);
                        break;
                    case TouchPhase.Canceled:
                        Up(touch.touchId, true);
                        break;
                }
            }
        }

        private void MovedThroughHistory(Touch touch)
        {
            if (!picker.Draws(touch.touchId))
            {
                return;
            }

            for (int sample = touch.history.Count - 1; sample >= 0; sample--)
            {
                Moved(touch.touchId, touch.history[sample].screenPosition);
            }

            Moved(touch.touchId, touch.screenPosition);
        }

        private void ReadMouse()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null)
            {
                return;
            }

            Vector2 position = mouse.position.ReadValue();
            if (mouse.leftButton.wasPressedThisFrame)
            {
                Down(MouseId, position, 0f);
            }
            else if (mouse.leftButton.isPressed)
            {
                Moved(MouseId, position);
            }
            else if (mouse.leftButton.wasReleasedThisFrame)
            {
                Moved(MouseId, position);
                Up(MouseId, false);
            }
        }

        private void Down(int touchId, Vector2 screen, float radius)
        {
            bool onBoard = RectTransformUtility.RectangleContainsScreenPoint(board, screen, null);
            if (input != null && picker.Began(touchId, onBoard, radius > PalmRadiusInPixels()) && ToBoard(screen, out Vector2 local))
            {
                input.StrokeBegan(local);
            }
        }

        private void Moved(int touchId, Vector2 screen)
        {
            if (input != null && picker.Draws(touchId) && ToBoard(screen, out Vector2 local))
            {
                input.StrokeMoved(local);
            }
        }

        private void Up(int touchId, bool cancelled)
        {
            if (input == null || !picker.Lifted(touchId))
            {
                return;
            }

            if (cancelled)
            {
                input.StrokeCancelled();
            }
            else
            {
                input.StrokeEnded();
            }
        }

        private bool ToBoard(Vector2 screen, out Vector2 local)
        {
            return RectTransformUtility.ScreenPointToLocalPointInRectangle(board, screen, null, out local);
        }

        private float PalmRadiusInPixels()
        {
            float dpi = Screen.dpi > 0f ? Screen.dpi : 160f;
            return palmRadius * dpi / 160f;
        }
    }
}
