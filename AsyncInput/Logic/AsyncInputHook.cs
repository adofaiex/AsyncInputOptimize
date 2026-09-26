using ModsTagLib.Win32;
using System.Runtime.CompilerServices;
using UnityEngine;
using ModsTagLib.Time;

#if ALPHA_2_9_8_R136 || RELEASE_2_5_0_R110
using static AsyncInput.SemiADOToolsLib.ADORef_scrPlanet;
#else
using static AsyncInput.SemiADOToolsLib.ADORef_scrPlayer;
#endif
using static AsyncInput.SemiADOToolsLib.ADORef_scrController;
using static AsyncInput.SemiADOToolsLib.ADORef_scrConductor;
using ModsTagLib;

namespace AsyncInput.Logic
{
    public static unsafe class AsyncInputHook
    {
        public static void ResetTime()
        {
            SafeDSPTime.SetOffset(0);
            AsyncInputData.prevFrameNano = AsyncInputData.currFrameNano;
            AsyncInputData.currFrameNano = TimeInstance.PTime.U_NanoSecond();
            AsyncInputData.dspTime = SafeDSPTime.InterpolationDSPTime;
            AsyncInputData.offsetNano = AsyncInputData.currFrameNano - (ulong)(AsyncInputData.dspTime * TimeConvert.D_Second_Nano);
            SwapArea.audioDelta = 0;
        }
        public static void PauseTime()
        {
            AsyncInputData.prevFrameNano = AsyncInputData.currFrameNano;
            AsyncInputData.currFrameNano = TimeInstance.PTime.U_NanoSecond();
            AsyncInputData.dspTime = SafeDSPTime.InterpolationDSPTime;
            AsyncInputData.offsetNano = AsyncInputData.currFrameNano - (ulong)(AsyncInputData.dspTime * TimeConvert.D_Second_Nano);
            SwapArea.audioDelta = 0;
        }
        public static void CountdownUpdate()
        {
            scrController ctrl = scrController.instance;
            scrConductor cdtr = scrConductor.instance;
            if (ctrl.goShown || cdtr.fastTakeoff || !(ctrl.state == States.PlayerControl || ctrl.state == States.Countdown || ctrl.state == States.Checkpoint))
                return;
            AsyncInputData.prevFrameNano = AsyncInputData.currFrameNano;
            AsyncInputData.currFrameNano = TimeInstance.PTime.U_NanoSecond();
            AsyncInputData.dspTime = SafeDSPTime.InterpolationDSPTime;
            AsyncInputData.offsetNano = AsyncInputData.currFrameNano - (ulong)(AsyncInputData.dspTime * TimeConvert.D_Second_Nano);
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
                if (scrController.instance != null && !scrController.instance.goShown)
                {
                    CountdownUpdate();
                    goto END;
                }
                if (scrController.instance?.paused ?? true)
                {
                    PauseTime();
                    return;
                }
                double audio_precise = SafeDSPTime.GetAuidoPrecise();
                AsyncInputData.prevFrameNano = AsyncInputData.currFrameNano;
                AsyncInputData.currFrameNano = TimeInstance.PTime.U_NanoSecond();
                AsyncInputData.dspTime = (AsyncInputData.currFrameNano - AsyncInputData.offsetNano) / TimeConvert.D_Second_Nano;
                AsyncInputData.offsetNano_REAL = AsyncInputData.currFrameNano - (ulong)(SafeDSPTime.InterpolationDSPTime * TimeConvert.D_Second_Nano);
                AsyncInputData.offsetNanos[AsyncInputData.offsetNanosIndex++] = AsyncInputData.offsetNano_REAL;
                long delta = (long)(AsyncInputData.offsetNano_REAL - AsyncInputData.offsetNano);

                if (System.Math.Abs(delta) > audio_precise * TimeConvert.I_Second_Nano * 4 && audio_precise != 0)
                {
                    AsyncInputData.offsetNanosIndex = 0;
                    SafeDSPTime.AddOffset(delta);
                    SwapArea.audioDelta += delta;
                    Starter.bootFile.Log.WARN("DSPTime XRUN Error: " + delta);
                    goto JMP_RELOAD;
                }
                if (AsyncInputData.offsetNanosIndex == 30)
                {
                    AsyncInputData.offsetNanosIndex = 0;
                    Int128 datas = 0;
                    foreach (ulong val in AsyncInputData.offsetNanos)
                        datas += val;
                    delta = (long)((datas / 30).Low - AsyncInputData.offsetNano);
                    if (System.Math.Abs(delta) > audio_precise * 500000000)
                    {
                        if (AsyncInputData.lastOffsetModify < 0 && System.Math.Abs(delta + AsyncInputData.lastOffsetModify) < 1000000)
                        {
                            SwapArea.audioDelta -= AsyncInputData.lastOffsetModify;
                            delta = (delta - AsyncInputData.lastOffsetModify) >> 2;
                            Starter.bootFile.Log.INFO("Offset fix(AVG): " + delta);
                        }
                        else
                        {
                            SwapArea.audioDelta += delta;
                            Starter.bootFile.Log.INFO("Offset fix: " + delta);
                        }
                        SafeDSPTime.AddOffset(delta);
                        AsyncInputData.lastOffsetModify = delta;
                    }
                }
            END:

#if ALPHA_2_9_8_R136 || RELEASE_2_5_0_R110 || RELEASE || BETA
                AsyncInputManager.prevFrameTick = AsyncInputData.prevFrameNano / 100;
                AsyncInputManager.currFrameTick = AsyncInputData.currFrameNano / 100;
                AsyncInputManager.offsetTick = AsyncInputData.offsetNano / 100; 
#else
                AsyncInputManager.prevFrameTick = (long)(AsyncInputData.prevFrameNano / 100);
                AsyncInputManager.currFrameTick = (long)(AsyncInputData.currFrameNano / 100);
                AsyncInputManager.offsetTick = (long)(AsyncInputData.offsetNano / 100);
#endif
                AsyncInputManager.previousFrameTime = Time.timeAsDouble;
                AsyncInputManager.offsetTickUpdated = true;
#if ALPHA_2_9_8_R136 || RELEASE_2_5_0_R110
                AsyncInputManager.dspTime = AsyncInputData.dspTime;
                AsyncInputManager.dspTimeSong = dspTimeSong.get(@this);
#else
#endif

                if (ADOBase.controller != null && !ADOBase.controller.paused)
                    UpdateInput(ADOBase.controller);
            }
#if !RELEASE_2_5_0_R110
            @this.prev_dspTime = @this.dspTime;
            @this.prev_unityDspTime = dspTime;
#endif
        }
        public static void UnInput()
        {
            AsyncInputData.keyQueue.Clear();
        }
        public static void UpdateInput(scrController @this)
        {
#if !RELEASE_2_5_0_R110
            if (!_allowDevCached.get())
            {
                _allowDevCached.set(true);
                _allowDebug.set(GCS.allowDebug);
            }
#endif
#if ALPHA_2_9_8_R136 || RELEASE_2_5_0_R110
            if (!RDInput.asyncKeyboardMouseInput.isActive)
            {
                UnInput();
                return;
            }
#else
            if (!RDInput.asyncKeyboard.isActive && !RDInput.asyncKeyboardLeft.isActive && !RDInput.asyncKeyboardRight.isActive)
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
#if ALPHA_2_9_8_R136 || RELEASE_2_5_0_R110
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ProcessKeyInputs(scrController @this)
        {
            if ((@this.state | (States)@this.stateMachine.GetState()) == States.PlayerControl)
            {
                SimulatedPlayerUpdate(@this, AsyncInputData.currFrameNano / 100);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ProcessKeyInputs(scrController @this, ulong value, bool state)
        {
            if ((@this.state | (States)@this.stateMachine.GetState()) == States.PlayerControl && @this.currFloor != null && !@this.isCutscene)
            {
                Fast_SimulatedPlayerUpdate(@this, value, state);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WhileFloorNotChange(scrController @this, delegate*<scrController, ulong?, void> ptr, ulong? targetTick)
        {
            int num = -1;
            while (num != @this.currFloor.seqID)
            {
                num = @this.currFloor.seqID;
                ptr(@this, targetTick);
            }
        }
        public static void SimulatedPlayerUpdate(scrController @this, ulong targetTick)
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
#if !RELEASE_2_5_0_R110
            if (RDInput.GetMain(ButtonState.WentUp) > 0)
            {
                @this.HitInputEvent(isAuto: false, InputEventState.Up);
            }
#endif
            Vector3 topos = @this.camy.topos;
            if (cachedCamyToPos.get(@this) != topos)
            {
                scrController.shouldReplaceCamyToPos = true;
                scrController.overrideCamyToPos = topos;
            }
        }
        public static void Fast_SimulatedPlayerUpdate(scrController @this, ulong targetTick, bool state)
        {
            AsyncInputData.clickTime = targetTick;
            targetTick /= 100;
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
#if !RELEASE_2_5_0_R110
            if (RDInput.GetMain(ButtonState.WentUp) > 0)
            {
                @this.HitInputEvent(isAuto: false, InputEventState.Up);
            }
#endif
            Vector3 topos = @this.camy.topos;
            if (cachedCamyToPos.get(@this) != topos)
            {
                scrController.shouldReplaceCamyToPos = true;
                scrController.overrideCamyToPos = topos;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AdjustAngle(scrController controller, ulong tick)
        {
            if (AsyncInputData.clickTime / 100 == tick)
                tick = AsyncInputData.clickTime;
            else
                tick *= 100;
            if (AsyncInputManager.isActive)
            {
                AsyncInputManager.targetSongTick = (tick - AsyncInputData.offsetNano) / 100;
#if RELEASE_2_5_0_R110
                AsyncRefreshAngles(controller.chosenplanet, tick - AsyncInputData.offsetNano);
#else
                AsyncRefreshAngles(controller.chosenPlanet, tick - AsyncInputData.offsetNano);
#endif
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AsyncRefreshAngles(scrPlanet planet, ulong songtick)
        {
            planet.angle = GetAsyncAngle(planet, snappedLastAngle.get(planet), songtick);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GetAsyncAngle(scrPlanet planet, double snappedLastAngle, ulong nowTick)
        {
            return snappedLastAngle + (GetSongPosition(planet.conductor, nowTick) - planet.conductor.lastHit) / planet.conductor.crotchetAtStart * System.Math.PI * planet.controller.speed * (double)(planet.controller.isCW ? 1 : (-1));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GetSongPosition(scrConductor conductor, ulong nowTick)
        {
            double callibration = scrConductor.currentPreset.inputOffset / 1000.0;
            if (!GCS.d_oldConductor && !GCS.d_webglConductor)
            {
                return (nowTick / 1000000000.0 - AsyncInputManager.dspTimeSong - callibration) * (double)conductor.song.pitch - conductor.addoffset;
            }

            return conductor.song.timeSamples / (double)conductor.song.clip.frequency - callibration - conductor.addoffset / (double)conductor.song.pitch;
        }
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ProcessKeyInputs(scrController @this)
        {
            if ((@this.state | (States)@this.stateMachine.GetState()) == States.PlayerControl)
            {
                foreach (scrPlayer player in ADOBase.playerManager)
                {
                    SimulatedPlayerUpdate(player, AsyncInputData.currFrameNano / 100);
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ProcessKeyInputs(scrController @this, ulong value, bool state)
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
        public static void WhileFloorNotChange(scrPlayer @this, delegate*<scrPlayer, ulong?, void> ptr, ulong? targetTick)
        {
            int num = -1;
            while (num != @this.currFloor.seqID)
            {
                num = @this.currFloor.seqID;
                ptr(@this, targetTick);
            }
        }
        public static void SimulatedPlayerUpdate(scrPlayer @this, ulong targetTick)
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
        public static void Fast_SimulatedPlayerUpdate(scrPlayer @this, ulong targetTick, bool state)
        {
            AsyncInputData.clickTime = targetTick;
            targetTick /= 100;
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
            Vector2 topos = ctrl.camy.topos;
            if (cachedCamyToPos.get(@this) != topos)
            {
                scrPlayer.shouldReplaceCamyToPos = true;
                scrPlayer.overrideCamyToPos = topos;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AdjustAngle(scrPlayer player, ulong tick)
        {
            if (AsyncInputData.clickTime / 100 == tick)
                tick = AsyncInputData.clickTime;
            else
                tick *= 100;
            if (AsyncInputManager.isActive)
            {
#if ALPHA_2_9_8_R136 || RELEASE_2_5_0_R110 || RELEASE || BETA
                AsyncInputManager.targetSongTick = (tick - AsyncInputData.offsetNano) / 100;
#else
                AsyncInputManager.targetSongTick = (long)((tick - AsyncInputData.offsetNano) / 100);
#endif
                AsyncRefreshAngles(player.planetarySystem.chosenPlanet, tick - AsyncInputData.offsetNano);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AsyncRefreshAngles(scrPlanet planet, ulong songtick)
        {
            planet.angle = GetAsyncAngle(planet, planet.snappedLastAngle, songtick);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GetAsyncAngle(scrPlanet planet, double snappedLastAngle, ulong nowTick)
        {
            return snappedLastAngle + (GetSongPosition(planet.conductor, nowTick) - planet.player.lastHit) / planet.conductor.crotchetAtStart * System.Math.PI * planet.planetarySystem.speed * (double)(planet.planetarySystem.isCW ? 1 : (-1));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GetSongPosition(scrConductor conductor, ulong nowTick)
        {
            double callibration = scrConductor.currentPreset.inputOffset / 1000.0;
            if (!GCS.d_oldConductor && !GCS.d_webglConductor)
            {
                return (nowTick / 1000000000.0 - conductor.dspTimeSong - callibration) * (double)conductor.song.pitch - conductor.addoffset;
            }

            return conductor.song.timeSamples / (double)conductor.song.clip.frequency - callibration - conductor.addoffset / (double)conductor.song.pitch;
        }
#endif
    }
}
