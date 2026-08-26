using AsyncInput.Core;
using AsyncInput.Logic;
using AsyncInput.Patch;
using ModsTagLib.FileFormat.Json;
using ModsTagLib.Unity;
using ModsTagLib.Unity.ModLayout;
using ModsTagLib.Win32;
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
            Config.JObject = JExtend.Load(GetPath(Config.FILE_NAME));

            bool active = AsyncInputManager.isActive;
            if (active)
            {
                AsyncInputManager.ToggleHook(false);
            }
            AudioSettings.OnAudioConfigurationChanged += SafeDSPTime.Init;
#if RELEASE_2_5_0_R110
            SafeDSPTime.Init();
#endif

            dmpch = new(this, "DynamicPatch");
            dmpch.Add(BasePatch.New(typeof(SkyHook__SkyHookManager), typeof(SkyHook.SkyHookManager), "_StartHook", PatchTypes.Transpiler));
            dmpch.Add(BasePatch.New(typeof(SkyHook__SkyHookManager), typeof(SkyHook.SkyHookManager), "_StopHook", PatchTypes.Transpiler));
            dmpch.Add(BasePatch.New(typeof(SkyHook__SkyHookManager), typeof(SkyHook.SkyHookManager), "get_isHookActive", PatchTypes.Transpiler));
            dmpch.Add(BasePatch.New(typeof(__AsyncInputUtils), typeof(AsyncInputUtils), "AdjustAngle", PatchTypes.Transpiler));
            dmpch.Add(BasePatch.New(typeof(__AsyncInputUtils), typeof(AsyncInputUtils), "GetAngle", PatchTypes.Transpiler));
            dmpch.Add(BasePatch.New(typeof(__AsyncInputUtils), typeof(AsyncInputUtils), "GetSongPosition", PatchTypes.Transpiler));
            dmpch.Add(BasePatch.New(typeof(__scrPlanet), typeof(scrPlanet), "AsyncRefreshAngles", PatchTypes.Transpiler));
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
        protected override void OpenGUI()
        {
            guiInstance.StyleType = GUILInstance.UIType.ColorUI_V1;
            FastGUI.Load();
        }
        protected override void OptionGUI()
        {
            AsyncInput.OptionGUI.Main();
        }
        protected override void Patch()
        {
        }
        protected override void Save()
        {
            JExtend.Save(Config.JObject, GetPath(Config.FILE_NAME));
        }
        protected override void TUpdate()
        {
            if (GlobalVar.ClearAllKeys)
            {
                Config.SelectKeys.Clear();
                GlobalVar.ClearAllKeys = false;
            }
        }
        protected override void TInputUpdate()
        {
            InputEventManager.KeyPackage pkg = InputEventManager.LastPackage;
            if (GlobalVar.SelectKeysMode)
            {
                if (pkg.flags == 0)
                    return;
                if (pkg.vkCode > (byte)VirtualKeys.VK_NONAME_07)
                {
                    if (Config.SelectKeys.Contains((VirtualKeys)pkg.vkCode))
                    {
                        Config.SelectKeys.Remove((VirtualKeys)pkg.vkCode);
                    }
                    else
                    {
                        Config.SelectKeys.Add((VirtualKeys)pkg.vkCode);
                    }
                }
                return;
            }

            if (!AsyncInputData.enabled)
                return;
            bool match = false;
            for (int i = 0; i < Config.SelectKeys.Count && !match; i++)
            {
                match = pkg.vkCode == (byte)Config.SelectKeys[i];
            }

            if (match && Config.BlackListMode)
            {
                return; // invalid input
            }
            else if (!match && !Config.BlackListMode)
            {
                return; // invalid input
            }

            AsyncKeyEvent ake = default;
            ake.time = pkg.time;
            ake.key = (VirtualKeys)pkg.vkCode;
            ake.state = pkg.flags != 0;
            AsyncInputData.keyQueue.Enqueue(ake);
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
