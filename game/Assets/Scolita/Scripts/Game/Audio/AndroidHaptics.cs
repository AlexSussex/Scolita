using System;
using UnityEngine;

namespace Scolita.Game.Audio
{
    /// <summary>
    /// The tablet's vibration (Android's <c>Vibrator</c>, which needs the VIBRATE permission in the launcher manifest): a light
    /// tick, <c>VibrationEffect.EFFECT_TICK</c> on Android 10 and later, else a 15 ms one-shot. The effect, the method and its
    /// argument are made once, so a tick allocates nothing. Off the tablet (the Editor), or on a tablet without a vibrator,
    /// it does nothing.
    /// </summary>
    public sealed class AndroidHaptics : IDisposable
    {
        private const int TickEffect = 2;
        private const int FirstSdkWithTick = 29;
        private const long OneShotMilliseconds = 15L;
        private const int DefaultAmplitude = -1;

        private readonly AndroidJavaObject vibrator;
        private readonly AndroidJavaObject effect;
        private readonly IntPtr vibrate;
        private readonly jvalue[] arguments;
        private bool failed;

        /// <summary>Finds the tablet's vibrator, on Android only; called by <see cref="SoundPlayer.Begin"/>.</summary>
        public AndroidHaptics()
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                return;
            }

            AndroidJavaObject found = null;
            AndroidJavaObject made = null;
            try
            {
                using (AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
                using (AndroidJavaClass version = new AndroidJavaClass("android.os.Build$VERSION"))
                using (AndroidJavaClass effects = new AndroidJavaClass("android.os.VibrationEffect"))
                {
                    found = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                    if (found == null || !found.Call<bool>("hasVibrator"))
                    {
                        found?.Dispose();
                        Debug.Log("Haptics: this tablet has no vibrator.");
                        return;
                    }

                    made = version.GetStatic<int>("SDK_INT") >= FirstSdkWithTick
                        ? effects.CallStatic<AndroidJavaObject>("createPredefined", TickEffect)
                        : effects.CallStatic<AndroidJavaObject>("createOneShot", OneShotMilliseconds, DefaultAmplitude);
                    IntPtr method = AndroidJNIHelper.GetMethodID(found.GetRawClass(), "vibrate", "(Landroid/os/VibrationEffect;)V");
                    if (made == null || method == IntPtr.Zero)
                    {
                        made?.Dispose();
                        found.Dispose();
                        Debug.LogWarning("Haptics: Vibrator.vibrate(VibrationEffect) or the tick effect was not found; nothing vibrates.");
                        return;
                    }

                    jvalue[] argument = new jvalue[1];
                    argument[0].l = made.GetRawObject();
                    arguments = argument;
                    vibrate = method;
                    effect = made;
                    vibrator = found;
                }
            }
            catch (AndroidJavaException exception)
            {
                made?.Dispose();
                found?.Dispose();
                Debug.LogWarning("Haptics: the vibrator could not be reached, so nothing vibrates: " + exception.Message);
            }
        }

        /// <summary>Whether the tablet can vibrate.</summary>
        public bool IsAvailable => vibrator != null && !failed;

        /// <summary>A light tick; called by <see cref="SoundPlayer.Vibrate"/> when the vibration switch is on.</summary>
        public void Tick()
        {
            if (!IsAvailable)
            {
                return;
            }

            AndroidJNI.CallVoidMethod(vibrator.GetRawObject(), vibrate, arguments);
            IntPtr thrown = AndroidJNI.ExceptionOccurred();
            if (thrown != IntPtr.Zero)
            {
                AndroidJNI.ExceptionDescribe();
                AndroidJNI.ExceptionClear();
                AndroidJNI.DeleteLocalRef(thrown);
                failed = true;
                Debug.LogWarning("Haptics: the vibrator refused a tick (is VIBRATE in the manifest?); nothing vibrates now.");
            }
        }

        /// <summary>Lets go of the Java objects; called by <see cref="SoundPlayer"/> when its scene closes.</summary>
        public void Dispose()
        {
            effect?.Dispose();
            vibrator?.Dispose();
        }
    }
}
