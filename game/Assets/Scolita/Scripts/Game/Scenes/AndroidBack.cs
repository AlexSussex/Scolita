using UnityEngine.InputSystem;

namespace Scolita.Game.Scenes
{
    /// <summary>
    /// Android's Back (the Input System reports it as the keyboard's Escape, which is Escape in the Editor too): every
    /// screen does with it what its own Back does (the brief's scope 8). Read once a frame by the scenes' composition
    /// roots.
    /// </summary>
    public static class AndroidBack
    {
        /// <summary>Whether Back was pressed this frame. Called by the scenes' composition roots.</summary>
        public static bool WasPressed()
        {
            Keyboard keyboard = Keyboard.current;
            return keyboard != null && keyboard.escapeKey.wasPressedThisFrame;
        }
    }
}
