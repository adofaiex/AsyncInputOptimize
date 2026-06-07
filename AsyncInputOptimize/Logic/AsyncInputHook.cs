using UnityEngine;

using static AsyncInputOptimize.SemiADOToolsLib.ADORef_scrConductor;

namespace AsyncInputOptimize.Logic
{
    public static unsafe class AsyncInputHook
    {
        public static void ResetTime()
        {
            SafeDSPTime.SetOffset(0);
            AsyncInputData.prevFrameTick = AsyncInputData.currFrameTick;
            AsyncInputData.currFrameTick = (ulong)CppBrige.GetSystemTick() + AsyncInputData.START_TIME;
            AsyncInputData.offsetTick = AsyncInputData.currFrameTick - (ulong)SafeDSPTime.InterpolationDSPTimeAsFileTime;
            AsyncInputData.dspTime = SafeDSPTime.InterpolationDSPTime;
        }
        public static void PauseTime()
        {
            AsyncInputData.prevFrameTick = AsyncInputData.currFrameTick;
            AsyncInputData.currFrameTick = (ulong)CppBrige.GetSystemTick() + AsyncInputData.START_TIME;
            AsyncInputData.offsetTick = AsyncInputData.currFrameTick - (ulong)SafeDSPTime.InterpolationDSPTimeAsFileTime;
            AsyncInputData.dspTime = SafeDSPTime.InterpolationDSPTime;
        }
        public static void ConductorUpdate(scrConductor @this)
        {
        JMP_RELOAD:
            double dspTime = SafeDSPTime.InterpolationDSPTime;
            double time = Time.unscaledTimeAsDouble;
            @this.dspTime = dspTime;
            lastReportedPlayheadPosition.SetValue(@this, dspTime);
            previousFrameTime.SetValue(@this, time);
            if (AsyncInputManager.isActive)
            {
                if (scrController.instance?.paused ?? true)
                {
                    PauseTime();
                    return;
                }
                double audio_precise = SafeDSPTime.GetAuidoPrecise();
                AsyncInputData.prevFrameTick = AsyncInputData.currFrameTick;
                AsyncInputData.currFrameTick = (ulong)CppBrige.GetSystemTick() + AsyncInputData.START_TIME;
                AsyncInputData.dspTime = (AsyncInputData.currFrameTick - AsyncInputData.offsetTick) / 10000000.0;
                AsyncInputData.offsetTick_REAL = AsyncInputData.currFrameTick - (ulong)SafeDSPTime.InterpolationDSPTimeAsFileTime;
                AsyncInputData.offsetTicks[AsyncInputData.offsetTicksIndex++] = AsyncInputData.offsetTick_REAL;
                long delta = (long)AsyncInputData.offsetTick_REAL - (long)AsyncInputData.offsetTick;

                if (System.Math.Abs(delta) > audio_precise * 10000000 * 4)
                {
                    AsyncInputData.offsetTicksIndex = 0;
                    SafeDSPTime.AddOffset(delta);
                    EntryPoint.logger.Warning("DSPTime XRUN Error");
                    goto JMP_RELOAD;
                }
                if (AsyncInputData.offsetTicksIndex == 30)
                {
                    AsyncInputData.offsetTicksIndex = 0;
                    ulong datas = 0;
                    foreach (ulong val in AsyncInputData.offsetTicks)
                        datas += val - AsyncInputData.START_TIME;
                    datas = datas / 30 + AsyncInputData.START_TIME;
                    delta = (long)datas - (long)AsyncInputData.offsetTick;
                    if (System.Math.Abs(delta) > audio_precise * 5000000)
                    {
                        SafeDSPTime.AddOffset(delta);
                        EntryPoint.logger.Log("Offset fix");
                    }
                }

                AsyncInputManager.prevFrameTick = AsyncInputData.prevFrameTick;
                AsyncInputManager.currFrameTick = AsyncInputData.currFrameTick;
                AsyncInputManager.offsetTick = AsyncInputData.offsetTick;
                AsyncInputManager.previousFrameTime = Time.timeAsDouble;
                AsyncInputManager.offsetTickUpdated = true;
#if Alpha_2_9_8_R136
                AsyncInputManager.dspTime = AsyncInputData.dspTime;
                AsyncInputManager.dspTimeSong = (double)dspTimeSong.GetValue(@this);
#endif

                if (ADOBase.controller != null && !ADOBase.controller.paused)
                    ADOBase.controller.UpdateInput();
            }
            @this.prev_dspTime = @this.dspTime;
            @this.prev_unityDspTime = dspTime;
        }
    }
}
