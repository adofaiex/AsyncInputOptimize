using ADOFAI.Common.Platform;
using AsyncInput.Logic;

namespace AsyncInput
{
    public static class PatchMidLayer
    {
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
        public static void UpdateInput(scrController @this)
        {
            AsyncInputHook.UpdateInput(@this);
        }
        public static void CountdownUpdate(scrCountdown @this)
        {
            SongsHook.CountdownUpdate();
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
            SongsHook.ConductorUpdate(@this);
        }
#if ALPHA_2_9_8_R136 || RELEASE_2_5_0_R110
        public static void AdjustAngle(scrController controller, ulong tick)
        {
            AsyncInputHook.AdjustAngle(controller, tick);
        }
        public static void AsyncRefreshAngles(scrPlanet planet)
        {
            AsyncInputHook.AsyncRefreshAngles(planet, AsyncInputManager.targetSongTick * 100);
        }
        public static void GetAngle(scrPlanet planet, double snappedLastAngle, ulong nowTick)
        {
            AsyncInputHook.GetAsyncAngle(planet, snappedLastAngle, nowTick * 100);
        }
        public static void GetSongPosition(scrConductor conductor, ulong nowTick)
        {
            AsyncInputHook.GetSongPosition(conductor, nowTick * 100);
        }
#else
        public static void AdjustAngle(scrPlayer player, ulong tick)
        {
            AsyncInputHook.AdjustAngle(player, tick);
        }
        public static void AsyncRefreshAngles(scrPlanet planet)
        {
            AsyncInputHook.AsyncRefreshAngles(planet, AsyncInputManager.targetSongTick * 100);
        }
        public static void GetAngle(scrPlanet planet, double snappedLastAngle, ulong nowTick)
        {
            AsyncInputHook.GetAsyncAngle(planet, snappedLastAngle, nowTick * 100);
        }
        public static void GetSongPosition(scrConductor conductor, ulong nowTick)
        {
            AsyncInputHook.GetSongPosition(conductor, nowTick * 100);
        }
#endif
    }
}
