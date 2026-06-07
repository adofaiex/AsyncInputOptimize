using ADOFAI.Common.Platform;
using AsyncInputOptimize.Logic;

namespace AsyncInputOptimize
{
    public static class PatchMidLayer
    {
        public static bool calibration;

        public static void Calibration(bool state)
        {
            calibration = state;
        }
        public static void Reset()
        {
            AsyncInputHook.ResetTime();
            SongsHook.ResetTime();
        }
        public static void StartOrPlay()
        {
            AsyncInputHook.ResetTime();
            SongsHook.ResetTime();
        }
        public static void CountdownUpdate(scrCountdown @this)
        {
            SongsHook.CountdownUpdate();
        }
        public static void ConductorUpdate(scrConductor @this)
        {
            if (scrConductor.isAudioOutputDeviceChanged)
            {
                scrController.CheckForAudioOutputChange();
                scrConductor.isAudioOutputDeviceChanged = false;
            }
#if RELEASE || BETA || ALPHA
            PlatformHelper.instance.Update();
#elif Alpha_2_9_8_R136
            PlatformHelper.Instance.Update();
#endif
            AsyncInputHook.ConductorUpdate(@this);
            if (calibration)
                return;
            SongsHook.ConductorUpdate(@this);
        }
    }
}
