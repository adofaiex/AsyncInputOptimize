using AsyncInput.Logic;
using AsyncInput.Patch;
using ModsTagLib.Unity;
using ModsTagLib.Unity.ModLayout;
using System;
using UnityEngine;
using UnityModManagerNet;

namespace AsyncInput
{
    public sealed class Starter : ModsTagLib.Unity.Starter
    {
        internal Starter(UnityModManager.ModEntry me) : base(me.Path, me.Info.Id)
        {
            modEntry = me;
            this.AutoWithUmm(me);
        }

        public static UnityModManager.ModEntry modEntry;
        public static DynamicPatch dmpch;
        public static Starter instance;
        private static int id = -1;

        public static void __Bootstrap(UnityModManager.ModEntry me)
        {
            instance = new Starter(me);
        }

        protected override void Awake()
        {
        }
        protected override void EnabledMod()
        {
            log.allowDebug = true;
            log.optimizeDataType = true;
            log.writeParams = true;
            log.MethodType = LogMethod.All;
            bool active = AsyncInputManager.isActive;
            if (active)
            {
                AsyncInputManager.ToggleHook(false);
            }
            AudioSettings.OnAudioConfigurationChanged += SafeDSPTime.Init;

            dmpch = new(this, "DynamicPatch");
            dmpch.Add(BasePatch.New(typeof(SkyHook__SkyHookManager), typeof(SkyHook.SkyHookManager), "_StartHook", PatchTypes.Transpiler));
            dmpch.Add(BasePatch.New(typeof(SkyHook__SkyHookManager), typeof(SkyHook.SkyHookManager), "_StopHook", PatchTypes.Transpiler));
            dmpch.Add(BasePatch.New(typeof(SkyHook__SkyHookManager), typeof(SkyHook.SkyHookManager), "get_isHookActive", PatchTypes.Transpiler));
            dmpch.Add(BasePatch.New(typeof(__scnGame), typeof(scnGame), "Play", PatchTypes.Transpiler));
            dmpch.Add(BasePatch.New(typeof(__scrConductor), typeof(scrConductor), "Start", PatchTypes.Transpiler));
            dmpch.Add(BasePatch.New(typeof(__scrConductor), typeof(scrConductor), "Rewind", PatchTypes.Transpiler));
            dmpch.Add(BasePatch.New(typeof(__scrConductor), typeof(scrConductor), "Update", PatchTypes.Transpiler));
            dmpch.Add(BasePatch.New(typeof(__scrController), typeof(scrController), "UpdateInput", PatchTypes.Transpiler));
            dmpch.Add(BasePatch.New(typeof(__scrCountdown), typeof(scrCountdown), "Update", PatchTypes.Transpiler));
            dmpch.Add(BasePatch.New(typeof(UnityEngine__SceneManagement__SceneManager), typeof(UnityEngine.SceneManagement.SceneManager), "LoadSceneAsyncNameIndexInternal", PatchTypes.Transpiler));
            dmpch.Patch();

            if (active)
            {
                AsyncInputManager.ToggleHook(true);
            }
        }
        protected override void DisabledMod()
        {
            AudioSettings.OnAudioConfigurationChanged -= SafeDSPTime.Init;
            dmpch.UnPatch();
        }
        protected override void OptionGUI()
        {
            GUIL.BeginHorizontal();
            GUIL.LabelChar("AIData:enabled", 32);
            GUIL.Label(AsyncInputData.enabled.ToString());
            GUIL.EndHorizontal();
            GUIL.BeginHorizontal();
            GUIL.LabelChar("AIData:currFrameTick", 32);
            GUIL.Label(AsyncInputData.currFrameTick.ToString());
            GUIL.EndHorizontal();
            GUIL.BeginHorizontal();
            GUIL.LabelChar("AIData:prevFrameTick", 32);
            GUIL.Label(AsyncInputData.prevFrameTick.ToString());
            GUIL.EndHorizontal();
            GUIL.BeginHorizontal();
            GUIL.LabelChar("AIData:offsetTick", 32);
            GUIL.Label(AsyncInputData.offsetTick.ToString());
            GUIL.EndHorizontal();
            GUIL.BeginHorizontal();
            GUIL.LabelChar("AIData:offsetTick_REAL", 32);
            GUIL.Label(AsyncInputData.offsetTick_REAL.ToString());
            GUIL.EndHorizontal();
            GUIL.BeginHorizontal();
            GUIL.LabelChar("AIData:offsetTicks ", 32);
            GUIL.Label(AsyncInputData.offsetTicks.ToString());
            GUIL.EndHorizontal();
            GUIL.BeginHorizontal();
            GUIL.LabelChar("AIData:offsetTicksIndex", 32);
            GUIL.Label(AsyncInputData.offsetTicksIndex.ToString());
            GUIL.EndHorizontal();
            GUIL.BeginHorizontal();
            GUIL.LabelChar("AIData:dspTime", 32);
            GUIL.Label(AsyncInputData.dspTime.ToString());
            GUIL.EndHorizontal();
            GUIL.NextLine();
            GUIL.BeginHorizontal();
            GUIL.LabelChar("SData:currFrameTick", 32);
            GUIL.Label(SongsData.currFrameTick.ToString());
            GUIL.EndHorizontal();
            GUIL.BeginHorizontal();
            GUIL.LabelChar("SData:song1OffsetTick", 32);
            GUIL.Label(SongsData.song1OffsetTick.ToString());
            GUIL.EndHorizontal();
            GUIL.BeginHorizontal();
            GUIL.LabelChar("SData:song2OffsetTick", 32);
            GUIL.Label(SongsData.song2OffsetTick.ToString());
            GUIL.EndHorizontal();
            GUIL.BeginHorizontal();
            GUIL.LabelChar("SData:song1OffsetTick_REAL", 32);
            GUIL.Label(SongsData.song1OffsetTick_REAL.ToString());
            GUIL.EndHorizontal();
            GUIL.BeginHorizontal();
            GUIL.LabelChar("SData:song2OffsetTick_REAL", 32);
            GUIL.Label(SongsData.song2OffsetTick_REAL.ToString());
            GUIL.EndHorizontal();
        }
        protected override void Patch()
        {
        }
        protected override void TUpdate()
        {
            if (id == -1)
                id = InputManager.AddHook(AsyncInputHook.Hook);
        }
        protected override void ExceptionReload(Exception e, MethodType e_in)
        {
        }

        protected override object CustomEvent(ModsTagLib.Unity.Starter target, long id)
        {
            throw new NotImplementedException();
        }
    }
}
