using HarmonyLib;
using System;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;

namespace AsyncInputOptimize
{
    public static class EntryPoint
    {
        public static ModEntry.ModLogger logger;
        public static Texture2D ui2d;
        public static GUIStyleState gss;
        public static Harmony harmony;
        public static string path;
        public static bool cover_is_installer;
        public static bool mtlib_is_installer;
        public static bool FindMod(string name)
        {
            foreach (ModEntry modEntry in modEntries)
            {
                if ((modEntry.Info.DisplayName == name || modEntry.Info.Id == name) && modEntry.Active)
                {
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
                    return true;
                }
            }
            return false;
        }
        public static void Setup(ModEntry me)
        {
            ui2d = new(12, 12);
            Color s = Color.white;
            Color n = new(1f, 1f, 1f, 0f);
            ui2d.SetPixels(0, 0, 12, 12, new Color[144]
            {
                s,n,n,n,n,n,n,n,n,n,n,n,
                s,n,n,n,n,n,n,n,n,n,n,n,
                s,n,n,n,n,n,n,n,n,n,n,n,
                s,n,n,n,n,n,n,n,n,n,n,n,
                s,n,n,n,n,n,n,n,n,n,n,n,
                s,n,n,n,n,n,n,n,n,n,n,n,
                s,n,n,n,n,n,n,n,n,n,n,n,
                s,n,n,n,n,n,n,n,n,n,n,n,
                s,n,n,n,n,n,n,n,n,n,n,n,
                s,n,n,n,n,n,n,n,n,n,n,n,
                s,n,n,n,n,n,n,n,n,n,n,n,
                s,n,n,n,n,n,n,n,n,n,n,n,
            });
            ui2d.Apply();
            gss = new() { background = ui2d };
            harmony = new Harmony(me.Info.Id);
            logger = me.Logger;
            path = me.Path;
            me.OnToggle = Toggle;
            me.OnUpdate = Update;
            me.OnGUI = GUI;
            cover_is_installer = FindMod("Cover", "Cover.dll");
            mtlib_is_installer = FindMod("ModsTagLib.Unity", "ModsTagLib.__Bootstrap.dll");
            CppBrige.Init(me);
        }
        public static bool Toggle(ModEntry me, bool a)
        {
            cover_is_installer = FindMod("Cover", "Cover.dll");
            mtlib_is_installer = FindMod("ModsTagLib.Unity", "ModsTagLib.__Bootstrap.dll");
            if (a)
            {
                harmony.PatchAll();
                SafeDSPTime.Init();
            }
            else
            {
                harmony.UnpatchAll(me.Info.Id);
            }
            return true;
        }
        public static void Update(ModEntry me, float _)
        {
        }
        public static void GUI(ModEntry me)
        {
            GUILayout.Label("当前DSPTime状态: ");
            GUILayout.BeginHorizontal();
            GUILayout.Space(16);

            GUILayout.BeginVertical();
            GUILayout.Label("插值DSPTime");
            GUILayout.Label("音频DSPTime");
            GUILayout.Label("DSPTime相差");
            GUILayout.Label("");
            GUILayout.Label("音频缓冲大小");
            GUILayout.Label("音频采样率");
            GUILayout.Label("音频DSPTime精度");
            GUILayout.EndVertical();

            GUILayout.BeginVertical(new GUIStyle(GUIStyle.none) { normal = gss, hover = gss, focused = gss, active = gss, onNormal = gss, onHover = gss, onFocused = gss, onActive = gss, border = new RectOffset(4, 4, 4, 4) });
            GUILayout.Label("");
            GUILayout.Label("");
            GUILayout.Label("");
            GUILayout.Label("");
            GUILayout.Label("");
            GUILayout.Label("");
            GUILayout.Label("");
            GUILayout.Label("");
            GUILayout.EndVertical();

            double sim_dsptime = SafeDSPTime.InterpolationDSPTime;
            double dsptime = AudioSettings.dspTime;
            int buffer_size = AudioSettings.GetConfiguration().dspBufferSize;
            int sample_rate = AudioSettings.GetConfiguration().sampleRate;

            GUILayout.BeginVertical();
            GUILayout.Label(sim_dsptime.ToString("f12"));
            GUILayout.Label(dsptime.ToString("f12"));
            GUILayout.Label((dsptime - sim_dsptime).ToString("f12"));
            GUILayout.Label("");
            GUILayout.Label(buffer_size.ToString());
            GUILayout.Label(sample_rate.ToString());
            GUILayout.Label((buffer_size / (double)sample_rate).ToString("f17") + "s");
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}
