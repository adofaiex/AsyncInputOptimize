using ADOFAI.Common.Platform;
using ModsTagLib;
using ModsTagLib.Unity;
using ModsTagLib.Win32;
using System.Runtime.CompilerServices;
using UnityEngine;

#if RELEASE || BETA || ALPHA
using static AsyncInput.SemiADOToolsLib.ADORef_scrPlayer;
#endif
using static AsyncInput.SemiADOToolsLib.ADORef_scrController;
using static AsyncInput.SemiADOToolsLib.ADORef_scrConductor;

namespace AsyncInput.Logic
{
    public static unsafe class AsyncInputHook
    {
        public static void ResetTime()
        {
            SafeDSPTime.SetOffset(0);
            AsyncInputData.prevFrameTick = AsyncInputData.currFrameTick;
            AsyncInputData.currFrameTick = (ulong)ModsTagCLib.PreciseFileTime();
            AsyncInputData.offsetTick = AsyncInputData.currFrameTick - (ulong)SafeDSPTime.InterpolationDSPTimeAsFileTime;
            AsyncInputData.dspTime = SafeDSPTime.InterpolationDSPTime;
        }
        public static void ConductorUpdate(scrConductor @this)
        {
        JMP_RELOAD:
            double dspTime = SafeDSPTime.InterpolationDSPTime;
            double time = Time.unscaledTimeAsDouble;
            @this.dspTime = dspTime;
            lastReportedPlayheadPosition.set(@this, dspTime);
            previousFrameTime.set(@this, time);
            if (AsyncInputManager.isActive)
            {
                double audio_precise = SafeDSPTime.GetAuidoPrecise();
                AsyncInputData.prevFrameTick = AsyncInputData.currFrameTick;
                AsyncInputData.currFrameTick = (ulong)ModsTagCLib.PreciseFileTime();
                AsyncInputData.dspTime = (AsyncInputData.currFrameTick - AsyncInputData.offsetTick) / 10000000.0;
                AsyncInputData.offsetTick_REAL = AsyncInputData.currFrameTick - (ulong)SafeDSPTime.InterpolationDSPTimeAsFileTime;
                AsyncInputData.offsetTicks[AsyncInputData.offsetTicksIndex++] = AsyncInputData.offsetTick_REAL;
                long delta = (long)AsyncInputData.offsetTick_REAL - (long)AsyncInputData.offsetTick;

                if (System.Math.Abs(delta) > audio_precise * 10000000 * 4)
                {
                    AsyncInputData.offsetTicksIndex = 0;
                    SafeDSPTime.AddOffset(delta);
                    Starter.instance.log.WARN("DSPTime XRUN Error");
                    goto JMP_RELOAD;
                }
                if (AsyncInputData.offsetTicksIndex == 30)
                {
                    AsyncInputData.offsetTicksIndex = 0;
                    ulong datas = 0;
                    foreach (ulong val in AsyncInputData.offsetTicks)
                        datas += val;
                    datas = datas / 30;
                    delta = (long)datas - (long)AsyncInputData.offsetTick;
                    if (System.Math.Abs(delta) > audio_precise * 5000000)
                    {
                        SafeDSPTime.AddOffset(delta);
                        Starter.instance.log.INFO("Offset fix");
                    }
                }

                AsyncInputManager.prevFrameTick = AsyncInputData.prevFrameTick;
                AsyncInputManager.currFrameTick = AsyncInputData.currFrameTick;
                AsyncInputManager.offsetTick = AsyncInputData.offsetTick;
                AsyncInputManager.previousFrameTime = Time.timeAsDouble;
                AsyncInputManager.offsetTickUpdated = true;
#if ALPHA_2_9_8_R136
                AsyncInputManager.dspTime = AsyncInputData.dspTime;
                AsyncInputManager.dspTimeSong = dspTimeSong.get(@this);
#endif

                if (ADOBase.controller != null && !ADOBase.controller.paused)
                    ADOBase.controller.UpdateInput();
            }
            @this.prev_dspTime = @this.dspTime;
            @this.prev_unityDspTime = dspTime;
        }
        public static void Hook(InputManager.FastPackage package)
        {
            if (!AsyncInputData.enabled)
                return;
            AsyncKeyEvent ake = default;
            ake.time = package.time;
            ake.key = (VirtualKeys)package.vkCode;
            ake.state = package.flags == 1;
            AsyncInputData.keyQueue.Enqueue(ake);
        }
        public static unsafe void UnInput()
        {
            AsyncInputData.keyQueue.Clear();
        }
        public static unsafe void UpdateInput(scrController @this)
        {
            if (!_allowDevCached.get())
            {
                _allowDevCached.set(true);
                _allowDebug.set(GCS.allowDebug);
            }
#if RELEASE || BETA || ALPHA
            if (!RDInput.asyncKeyboard.isActive && !RDInput.asyncKeyboardLeft.isActive && !RDInput.asyncKeyboardRight.isActive)
            {
                UnInput();
                return;
            }
#elif ALPHA_2_9_8_R136
            if (!RDInput.asyncKeyboardMouseInput.isActive)
            {
                UnInput();
                return;
            }
#endif
            AsyncInputData.keyDownMask.Clear();
            AsyncInputData.keyUpMask.Clear();
            AsyncInputData.frameDependentKeyDownMask.Clear();
            AsyncInputData.frameDependentKeyUpMask.Clear();
            if (AsyncInputData.keyQueue.Count == 0)
            {
                ProcessKeyInputs(@this);
                return;
            }
            if (!Application.isFocused)
            {
                System.Array.Clear(AsyncInputData.keyMask, 0, AsyncInputData.keyMask.Length);
                AsyncInputData.keyQueue.Clear();
                ProcessKeyInputs(@this);
                return;
            }
            while (AsyncInputData.keyQueue.TryDequeue(out var item))
            {
                ProcessKeyInputs(@this, item.time, item.state);
                VirtualKeys vk = item.key;
                if (!item.state)
                {
                    AsyncInputData.keyMask[(byte)vk] = false;
                    AsyncInputData.keyUpMask.Add(vk);
                    AsyncInputData.frameDependentKeyMask[(byte)vk] = false;
                    AsyncInputData.frameDependentKeyUpMask.Add(vk);
                }
                else if (!AsyncInputData.keyMask[(byte)vk])
                {
                    AsyncInputData.keyMask[(byte)vk] = true;
                    AsyncInputData.keyDownMask.Add(vk);
                    AsyncInputData.frameDependentKeyMask[(byte)vk] = true;
                    AsyncInputData.frameDependentKeyDownMask.Add(vk);
                }
            }
        }
#if RELEASE || BETA || ALPHA
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ProcessKeyInputs(scrController @this)
        {
            if ((@this.state | (States)@this.stateMachine.GetState()) == States.PlayerControl)
            {
                foreach (scrPlayer player in ADOBase.playerManager)
                {
                    SimulatedPlayerUpdate(player, AsyncInputData.currFrameTick);
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ProcessKeyInputs(scrController @this, ulong value, bool state)
        {
            if ((@this.state | (States)@this.stateMachine.GetState()) == States.PlayerControl)
            {
                foreach (scrPlayer player in ADOBase.playerManager)
                {
                    Fast_SimulatedPlayerUpdate(player, value, state);
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WhileFloorNotChange(scrPlayer @this, delegate*<scrPlayer, ulong?, void> ptr, ulong? targetTick)
        {
            int num = -1;
            while (num != @this.currFloor.seqID)
            {
                num = @this.currFloor.seqID;
                ptr(@this, targetTick);
            }
        }
        public static unsafe void SimulatedPlayerUpdate(scrPlayer @this, ulong targetTick)
        {
            scrController ctrl = ADOBase.controller;
            if (!@this.alive || @this.currFloor == null || ctrl.isCutscene)
            {
                return;
            }

            __nextTileIsHoldCached.set(@this, false);
            validInputWasReleasedThisFrame.set(@this, @this.ValidInputWasReleased());
            cachedCamyToPos.set(@this, ctrl.camy.topos);
            if (@this.currFloor.nextfloor is not null)
            {
                scrFloor nextfloor = @this.currFloor.nextfloor;
                while (nextfloor.midSpin && (bool)nextfloor.nextfloor)
                {
                    nextfloor = nextfloor.nextfloor;
                }
                __nextTileIsHoldCached.set(@this, nextfloor.holdLength > -1);
            }

            WhileFloorNotChange(@this, CheckPostHoldFail.method, targetTick);
            WhileFloorNotChange(@this, OttoHoldHit.method, targetTick);
            UpdateHoldBehavior.method(@this, targetTick);
            WhileFloorNotChange(@this, HitHoldFloorsIfStartedAtHold.method, targetTick);
            WhileFloorNotChange(@this, CheckPreHoldFail.method, targetTick);
            if (RDInput.GetMain(ButtonState.WentUp) > 0)
            {
                @this.HitInputEvent(isAuto: false, InputEventState.Up);
            }
            Vector2 topos = ADOBase.controller.camy.topos;
            if (cachedCamyToPos.get(@this) != topos)
            {
                scrPlayer.shouldReplaceCamyToPos = true;
                scrPlayer.overrideCamyToPos = topos;
            }
        }
        public static unsafe void Fast_SimulatedPlayerUpdate(scrPlayer @this, ulong targetTick, bool state)
        {
            scrController ctrl = ADOBase.controller;
            if (!@this.alive || @this.currFloor == null || ctrl.isCutscene)
            {
                return;
            }

            __nextTileIsHoldCached.set(@this, false);
            validInputWasReleasedThisFrame.set(@this, !state);
            cachedCamyToPos.set(@this, ctrl.camy.topos);
            if (@this.currFloor.nextfloor is not null)
            {
                scrFloor nextfloor = @this.currFloor.nextfloor;
                while (nextfloor.midSpin && (bool)nextfloor.nextfloor)
                {
                    nextfloor = nextfloor.nextfloor;
                }
                __nextTileIsHoldCached.set(@this, nextfloor.holdLength > -1);
            }

            CheckPostHoldFail.method(@this, targetTick);
            if (state)
                @this.keyTimes.Add(Time.timeAsDouble);
            UpdateHoldBehavior.method(@this, targetTick);
            HitHoldFloorsIfStartedAtHold.method(@this, targetTick);
            CheckPreHoldFail.method(@this, targetTick);
            UpdateHoldKeys.method(@this, targetTick);
            if (RDInput.GetMain(ButtonState.WentUp) > 0)
            {
                @this.HitInputEvent(isAuto: false, InputEventState.Up);
            }
            Vector2 topos = ADOBase.controller.camy.topos;
            if (cachedCamyToPos.get(@this) != topos)
            {
                scrPlayer.shouldReplaceCamyToPos = true;
                scrPlayer.overrideCamyToPos = topos;
            }
        }
#elif ALPHA_2_9_8_R136
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ProcessKeyInputs(scrController @this)
        {
            if ((@this.state | (States)@this.stateMachine.GetState()) == States.PlayerControl)
            {
                SimulatedPlayerUpdate(@this, AsyncInputData.currFrameTick);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void ProcessKeyInputs(scrController @this, ulong value, bool state)
        {
            if ((@this.state | (States)@this.stateMachine.GetState()) == States.PlayerControl)
            {
                Fast_SimulatedPlayerUpdate(@this, value, state);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void WhileFloorNotChange(scrController @this, delegate*<scrController, ulong?, void> ptr, ulong? targetTick)
        {
            int num = -1;
            while (num != @this.currFloor.seqID)
            {
                num = @this.currFloor.seqID;
                ptr(@this, targetTick);
            }
        }
        public static unsafe void SimulatedPlayerUpdate(scrController @this, ulong targetTick)
        {
            if (@this.currFloor == null || @this.isCutscene)
            {
                return;
            }

            __nextTileIsHoldCached.set(@this, false);
            validInputWasReleasedThisFrame.set(@this, @this.ValidInputWasReleased());
            cachedCamyToPos.set(@this, @this.camy.topos);
            if (@this.currFloor.nextfloor is not null)
            {
                scrFloor nextfloor = @this.currFloor.nextfloor;
                while (nextfloor.midSpin && (bool)nextfloor.nextfloor)
                {
                    nextfloor = nextfloor.nextfloor;
                }
                __nextTileIsHoldCached.set(@this, nextfloor.holdLength > -1);
            }

            WhileFloorNotChange(@this, CheckPostHoldFail.method, targetTick);
            WhileFloorNotChange(@this, OttoHoldHit.method, targetTick);
            UpdateHoldBehavior.method(@this, targetTick);
            WhileFloorNotChange(@this, HitHoldFloorsIfStartedAtHold.method, targetTick);
            WhileFloorNotChange(@this, CheckPreHoldFail.method, targetTick);
            if (RDInput.GetMain(ButtonState.WentUp) > 0)
            {
                @this.HitInputEvent(isAuto: false, InputEventState.Up);
            }
            Vector3 topos = ADOBase.controller.camy.topos;
            if (cachedCamyToPos.get(@this) != topos)
            {
                scrController.shouldReplaceCamyToPos = true;
                scrController.overrideCamyToPos = topos;
            }
        }
        public static unsafe void Fast_SimulatedPlayerUpdate(scrController @this, ulong targetTick, bool state)
        {
            if (@this.currFloor == null || @this.isCutscene)
            {
                return;
            }

            __nextTileIsHoldCached.set(@this, false);
            validInputWasReleasedThisFrame.set(@this, !state);
            cachedCamyToPos.set(@this, @this.camy.topos);
            if (@this.currFloor.nextfloor is not null)
            {
                scrFloor nextfloor = @this.currFloor.nextfloor;
                while (nextfloor.midSpin && (bool)nextfloor.nextfloor)
                {
                    nextfloor = nextfloor.nextfloor;
                }
                __nextTileIsHoldCached.set(@this, nextfloor.holdLength > -1);
            }

            CheckPostHoldFail.method(@this, targetTick);
            if (state)
                @this.keyTimes.Add(Time.timeAsDouble);
            UpdateHoldBehavior.method(@this, targetTick);
            HitHoldFloorsIfStartedAtHold.method(@this, targetTick);
            CheckPreHoldFail.method(@this, targetTick);
            UpdateHoldKeys.method(@this, targetTick);
            if (RDInput.GetMain(ButtonState.WentUp) > 0)
            {
                @this.HitInputEvent(isAuto: false, InputEventState.Up);
            }
            Vector3 topos = ADOBase.controller.camy.topos;
            if (cachedCamyToPos.get(@this) != topos)
            {
                scrController.shouldReplaceCamyToPos = true;
                scrController.overrideCamyToPos = topos;
            }
        }
#endif
    }
}
