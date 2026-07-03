using ADOFAI.Common.Platform;
using AsyncInputOptimize.Logic;

namespace AsyncInputOptimize
{
    public static class PatchMidLayer
    {
        public static void Reset()
        {
            AsyncInputHook.ResetTime();
        }
        public static void StartOrPlay()
        {
            AsyncInputHook.ResetTime();
        }
        public static void ConductorUpdate(scrConductor @this)
        {
#if RELEASE_2_5_0_R110

#elif ALPHA_2_9_8_R136
            if (scrConductor.isAudioOutputDeviceChanged)
            {
                scrController.CheckForAudioOutputChange();
                scrConductor.isAudioOutputDeviceChanged = false;
            }
            PlatformHelper.Instance.Update();
#else
            if (scrConductor.isAudioOutputDeviceChanged)
            {
                scrController.CheckForAudioOutputChange();
                scrConductor.isAudioOutputDeviceChanged = false;
            }
            PlatformHelper.instance.Update();
#endif
            AsyncInputHook.ConductorUpdate(@this);
        }
    }
}
