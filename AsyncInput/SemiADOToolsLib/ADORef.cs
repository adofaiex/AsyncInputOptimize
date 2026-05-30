using HarmonyLib;
using ModsTagLib.Reflection;
using MonsterLove.StateMachine;
using System;
using UnityEngine;

using static ModsTagLib.Reflection.RefCreateTools;

namespace AsyncInput.SemiADOToolsLib
{
    public static class ADORef_scrConductor
    {
        public static readonly Type @this = typeof(scrConductor);
        public static readonly RFReference<double, scrConductor> dspTimeSong = TryFieldRef<double, scrConductor>(nameof(dspTimeSong));
        public static readonly RFReference<double, scrConductor> previousFrameTime = TryFieldRef<double, scrConductor>(nameof(previousFrameTime));
        public static readonly RFReference<double, scrConductor> lastReportedPlayheadPosition = TryFieldRef<double, scrConductor>(nameof(lastReportedPlayheadPosition));
    }
    public static class ADORef_scrController
    {
        public static readonly Type @this = typeof(scrController);
        public static readonly RFStatic<bool> _allowDevCached = TryFieldStatic<bool>(@this, nameof(_allowDevCached));
        public static readonly RFStatic<bool> _allowDebug = TryFieldStatic<bool>(@this, nameof(_allowDebug));
        public static readonly RPReference<float, scrController> tileSize = TryPropertyRef<float, scrController>(nameof(tileSize));
        public static readonly RPReference<float, scrController> startRadius = TryPropertyRef<float, scrController>(nameof(startRadius));
#if MAIN
    }
    public static class ADORef_scrPlayer
    {
        public static readonly Type @this = typeof(scrPlayer);
        public static readonly RFReference<bool, scrPlayer> __nextTileIsHoldCached = TryFieldRef<bool, scrPlayer>(nameof(__nextTileIsHoldCached));
        public static readonly RFReference<bool, scrPlayer> validInputWasReleasedThisFrame = TryFieldRef<bool, scrPlayer>(nameof(validInputWasReleasedThisFrame));
        public static readonly RFReference<Vector2, scrPlayer> cachedCamyToPos = TryFieldRef<Vector2, scrPlayer>(nameof(cachedCamyToPos));
        public static readonly RMAction<scrPlayer, ulong?> CheckPostHoldFail = MethodAct<scrPlayer, ulong?>(@this.GetMethod(nameof(CheckPostHoldFail), AccessTools.all));
        public static readonly RMAction<scrPlayer, ulong?> OttoHoldHit = MethodAct<scrPlayer, ulong?>(@this.GetMethod(nameof(OttoHoldHit), AccessTools.all));
        public static readonly RMAction<scrPlayer, ulong?> HitAutoFloors = MethodAct<scrPlayer, ulong?>(@this.GetMethod(nameof(HitAutoFloors), AccessTools.all));
        public static readonly RMAction<scrPlayer, ulong?> UpdateHoldBehavior = MethodAct<scrPlayer, ulong?>(@this.GetMethod(nameof(UpdateHoldBehavior), AccessTools.all));
        public static readonly RMAction<scrPlayer, ulong?> HitHoldFloorsIfStartedAtHold = MethodAct<scrPlayer, ulong?>(@this.GetMethod(nameof(HitHoldFloorsIfStartedAtHold), AccessTools.all));
        public static readonly RMAction<scrPlayer, ulong?> CheckPreHoldFail = MethodAct<scrPlayer, ulong?>(@this.GetMethod(nameof(CheckPreHoldFail), AccessTools.all));
        public static readonly RMAction<scrPlayer, ulong?> UpdateHoldKeys = MethodAct<scrPlayer, ulong?>(@this.GetMethod(nameof(UpdateHoldKeys), AccessTools.all));
#elif R136_2_9_8
        public static readonly RFReference<bool, scrController> __nextTileIsHoldCached = TryFieldRef<bool, scrController>(nameof(__nextTileIsHoldCached));
        public static readonly RFReference<bool, scrController> validInputWasReleasedThisFrame = TryFieldRef<bool, scrController>(nameof(validInputWasReleasedThisFrame));
        public static readonly RFReference<Vector3, scrController> cachedCamyToPos = TryFieldRef<Vector3, scrController>(nameof(cachedCamyToPos));
        public static readonly RMAction<scrController, ulong?> CheckPostHoldFail = MethodAct<scrController, ulong?>(@this.GetMethod(nameof(CheckPostHoldFail), AccessTools.all));
        public static readonly RMAction<scrController, ulong?> OttoHoldHit = MethodAct<scrController, ulong?>(@this.GetMethod(nameof(OttoHoldHit), AccessTools.all));
        public static readonly RMAction<scrController, ulong?> HitAutoFloors = MethodAct<scrController, ulong?>(@this.GetMethod(nameof(HitAutoFloors), AccessTools.all));
        public static readonly RMAction<scrController, ulong?> UpdateHoldBehavior = MethodAct<scrController, ulong?>(@this.GetMethod(nameof(UpdateHoldBehavior), AccessTools.all));
        public static readonly RMAction<scrController, ulong?> HitHoldFloorsIfStartedAtHold = MethodAct<scrController, ulong?>(@this.GetMethod(nameof(HitHoldFloorsIfStartedAtHold), AccessTools.all));
        public static readonly RMAction<scrController, ulong?> CheckPreHoldFail = MethodAct<scrController, ulong?>(@this.GetMethod(nameof(CheckPreHoldFail), AccessTools.all));
        public static readonly RMAction<scrController, ulong?> UpdateHoldKeys = MethodAct<scrController, ulong?>(@this.GetMethod(nameof(UpdateHoldKeys), AccessTools.all));
#endif
    }
    public static class ADORef_StateEngine
    {
        public static readonly Type @this = typeof(StateEngine);
        public static readonly RFReference<StateMapping, StateEngine> destinationState = TryFieldRef<StateMapping, StateEngine>(nameof(destinationState));
    }
}
