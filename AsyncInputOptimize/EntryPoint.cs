using HarmonyLib;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;

namespace AsyncInputOptimize
{
    public static class EntryPoint
    {
        public static Harmony harmony;
        public static bool cover_is_installer;
        public static bool FindMod(string name)
        {
            foreach (ModEntry modEntry in modEntries)
            {
                if ((modEntry.Info.DisplayName == name || modEntry.Info.Id == name) && modEntry.Active)
                {
                    cover_is_installer = true;
                    return true;
                }
            }
            return false;
        }
        public static bool FindMod(string name, string assembly_name)
        {
            foreach (ModEntry modEntry in modEntries)
            {
                if ((modEntry.Info.DisplayName == name || modEntry.Info.Id == name || modEntry.Info.AssemblyName == assembly_name) && modEntry.Active)
                {
                    cover_is_installer = true;
                    return true;
                }
            }
            return false;
        }
        public static void Setup(ModEntry me)
        {
            harmony = new Harmony(me.Info.Id);
            me.OnToggle = Toggle;
            me.OnUpdate = Update;
            me.OnGUI = GUI;
            cover_is_installer = FindMod("Cover", "Cover.dll");
        }
        public static bool Toggle(ModEntry me, bool a)
        {
            cover_is_installer = FindMod("Cover", "Cover.dll");
            if (cover_is_installer) return true;
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
            DSPTimeSimulater.Update();
        }
        public static void GUI(ModEntry me)
        {
            GUILayout.Label("当前DSPTime状态: ");
            GUILayout.BeginHorizontal();
            GUILayout.Space(16);

            GUILayout.BeginVertical();
            GUILayout.Label("模拟DSPTime(动态)");
            GUILayout.Label("模拟DSPTime");
            GUILayout.Label("音频DSPTime");
            GUILayout.Label("DSPTime相差");
            GUILayout.Label("DSPTime相差(平滑后)");
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label(" | ");
            GUILayout.Label(" | ");
            GUILayout.Label(" | ");
            GUILayout.Label(" | ");
            GUILayout.Label(" | ");
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            double sim_dsptime = DSPTimeSimulater.GetDSPTime();
            double sim_dydsptime = DSPTimeSimulater.GetDynamicDSPTime();
            double dsptime = AudioSettings.dspTime;
            (double, int) c = DSPTimeSimulater.GetDebugContent();
            GUILayout.Label(sim_dsptime.ToString("f12"));
            GUILayout.Label(sim_dydsptime.ToString("f12"));
            GUILayout.Label(dsptime.ToString("f12"));
            GUILayout.Label((dsptime - sim_dsptime).ToString("f12"));
            GUILayout.Label((c.Item1 / c.Item2).ToString("f12"));
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Label("");
            if (cover_is_installer)
            {
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                GUILayout.Label("(,,•́ . •̀,,)");
                GUILayout.Label("(^・ω・^ ).....");
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                GUILayout.Label(" | ");
                GUILayout.Label(" | ");
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                GUILayout.Label("喵? 你已经安装了<color=#3fffff>Cover</color>了喵, 直接用<color=#3fffff>Cover</color>的就行了喵");
                GUILayout.Label("Mod补丁我就给你禁用了喵~");
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            GUILayout.Label("");
        }
    }
}
