using HarmonyLib;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;

namespace AsyncInputOptimize
{
    public static class EntryPoint
    {
        public static Harmony harmony;
        public static bool cover_is_installer;
        public static void Setup(ModEntry me)
        {
            harmony = new Harmony(me.Info.Id);
            me.OnToggle = Toggle;
            me.OnUpdate = Update;
            me.OnGUI = GUI;
            foreach (ModEntry modEntry in modEntries)
            {
                if ((modEntry.Info.DisplayName == "Cover" || modEntry.Info.Id == "Cover" || modEntry.Info.AssemblyName == "Cover.dll") && modEntry.Active)
                {
                    cover_is_installer = true;
                    break;
                }
            }
        }
        public static bool Toggle(ModEntry me, bool a)
        {
            foreach (ModEntry modEntry in modEntries)
            {
                if ((modEntry.Info.DisplayName == "Cover" || modEntry.Info.Id == "Cover" || modEntry.Info.AssemblyName == "Cover.dll") && modEntry.Active)
                {
                    cover_is_installer = true;
                    return true;
                }
            }
            if (a)
            {
                harmony.PatchAll();
            }
            else
            {
                harmony.UnpatchAll();
            }
            return true;
        }
        public static void Update(ModEntry me, float dt)
        {
            AudioDSPManager.Update();
        }
        public static void GUI(ModEntry me)
        {
            GUILayout.Label("当前调整的偏移: ");
            GUILayout.Label("  dspTime: " + AudioDSPManager.GetDSPTime());
            GUILayout.Label("  cpy_dspTime: " + AudioDSPManager.cpy_dspTime);
            if (cover_is_installer)
            {
                GUILayout.Label("你已经安装了Cover了 直接用Cover的就行了");
            }
        }
    }
}
