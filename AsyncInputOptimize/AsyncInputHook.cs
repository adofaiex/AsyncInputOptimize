using ADOFAI.Common.Platform;
using UnityEngine;

using static AsyncInputOptimize.SemiADOToolsLib.ADORef_scrConductor;

namespace AsyncInputOptimize
{
    public static unsafe class AsyncInputHook
    {
        public static void ResetTime()
        {
            AsyncInputData.prevFrameTick = AsyncInputData.currFrameTick;
            AsyncInputData.currFrameTick = (ulong)CppBrige.GetSystemTick() + 504911520000000000;
            AsyncInputData.offsetTick = AsyncInputData.currFrameTick - (ulong)SafeDSPTime.InterpolationDSPTimeAsFileTime;
            AsyncInputData.dspTime = SafeDSPTime.InterpolationDSPTime;
        }
        public static void ConductorUpdate(scrConductor @this)
        {
            double dspTime = SafeDSPTime.InterpolationDSPTime;
            double time = Time.unscaledTimeAsDouble;
            if (scrConductor.isAudioOutputDeviceChanged)
            {
                scrController.CheckForAudioOutputChange();
                scrConductor.isAudioOutputDeviceChanged = false;
            }
            PlatformHelper.instance.Update();
            if (dspTime != (double)lastReportedPlayheadPosition.GetValue(@this))
            {
                @this.dspTime = dspTime;
                lastReportedPlayheadPosition.SetValue(@this, dspTime);
            }
            else if (!AudioListener.pause && Application.isFocused && time - (double)previousFrameTime.GetValue(@this) < 0.1)
            {
                @this.dspTime += time - (double)previousFrameTime.GetValue(@this);
            }
            previousFrameTime.SetValue(@this, time);
            if (AsyncInputManager.isActive)
            {
                UpdateTime(@this);
            }
            @this.prev_dspTime = @this.dspTime;
            @this.prev_unityDspTime = dspTime;
        }
        public static void UpdateTime(scrConductor @this)
        {
            AsyncInputData.prevFrameTick = AsyncInputData.currFrameTick;
            AsyncInputData.currFrameTick = (ulong)CppBrige.GetSystemTick() + 504911520000000000;
            AsyncInputData.dspTime = (AsyncInputData.currFrameTick - AsyncInputData.offsetTick) / 10000000.0;

            AsyncInputManager.prevFrameTick = AsyncInputData.prevFrameTick;
            AsyncInputManager.currFrameTick = AsyncInputData.currFrameTick;
            AsyncInputManager.offsetTick = AsyncInputData.offsetTick;
            AsyncInputManager.previousFrameTime = Time.timeAsDouble;
            AsyncInputManager.offsetTickUpdated = true;

            if (ADOBase.controller != null && !ADOBase.controller.paused)
            {
                ADOBase.controller.UpdateInput();
            }
        }
    }
}
