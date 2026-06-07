using ADOFAI.Common.Platform;
using AsyncInput.Logic;

namespace AsyncInput
{
    public static class PatchMidLayer
    {
        public static void Reset()
        {
            AsyncInputHook.ResetTime();
        }
        public static void UpdateInput(scrController @this)
        {
            AsyncInputHook.UpdateInput(@this);
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
#elif ALPHA_2_9_8_R136
            PlatformHelper.Instance.Update();
#endif
            AsyncInputHook.ConductorUpdate(@this);
            SongsHook.ConductorUpdate(@this);
        }
    }
}
