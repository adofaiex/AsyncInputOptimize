using AsyncInput.Logic;
using AsyncInput.Patch;
using ModsTagLib.Unity;
using System;
using UnityModManagerNet;

namespace AsyncInput
{
    public sealed class Starter : ModsTagLib.Unity.Starter
    {
        internal Starter(UnityModManager.ModEntry me) : base(me)
        {
        }

        public static DynamicPatch dmpch;
        public static Starter instance;
        private static int id = -1;

        public static void __Bootstrap(UnityModManager.ModEntry me)
        {
            instance = new Starter(me);
        }

        protected override void Awake()
        {
            SafeDSPTime.Init();
        }
        protected override void EnabledMod()
        {
            log.AllowDebug = true;
            log.OptimizeDataType = true;
            log.WriteParams = true;
            log.MethodType = LogMethod.All;
            bool active = AsyncInputManager.isActive;
            if (active)
            {
                AsyncInputManager.ToggleHook(false);
            }

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
            dmpch.UnPatch();
        }
        protected override void Patch()
        {
        }
        protected override void TUpdate(double tick)
        {
            if (id == -1)
                id = InputManager.AddHook(AsyncInputHook.Hook);
        }
        protected override void ExceptionReload(Exception e, ExceptionIn e_in)
        {
        }
    }
}
