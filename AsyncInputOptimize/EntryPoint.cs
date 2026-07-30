using AsyncInputOptimize.Logic;
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
        }
        public static bool Toggle(ModEntry me, bool a)
        {
            cover_is_installer = FindMod("Cover", "Cover.dll");
            mtlib_is_installer = FindMod("ModsTagLib.Unity", "ModsTagLib.__Bootstrap.dll");
            if (a)
            {
                AudioSettings.OnAudioConfigurationChanged += SafeDSPTime.Init;
                harmony.PatchAll();
            }
            else
            {
                AudioSettings.OnAudioConfigurationChanged -= SafeDSPTime.Init;
                harmony.UnpatchAll(me.Info.Id);
            }
            return true;
        }
        public static void Update(ModEntry me, float _)
        {
        }
        public static void GUI(ModEntry me)
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.MinWidth(160));
            GUILayout.Label("DSPTime Inter. ");
            GUILayout.Label("        Original");
            GUILayout.Label("        Delta");
            GUILayout.Label("        Offset");
            GUILayout.Label("        Precise");
            GUILayout.Label("Audio Buffer Size");
            GUILayout.Label("      Simple Rate");
            GUILayout.Label("");
            GUILayout.Label("AID RealOffsetTick");
            GUILayout.Label("AID OffsetTick");
            GUILayout.Label("    Delta");
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
            GUILayout.Label("");
            GUILayout.Label("");
            GUILayout.Label("");
            GUILayout.EndVertical();

            double DSPTime_Inter = SafeDSPTime.InterpolationDSPTime;
            double DSPTime_Original = AudioSettings.dspTime;
            double DSPTime_Offset = SafeDSPTime.GetOffset() / 10_000d;
            double DSPTime_Precise = SafeDSPTime.GetAuidoPrecise() * 1000;
            int Audio_BufferSize = AudioSettings.GetConfiguration().dspBufferSize;
            int Audio_SimpleRate = AudioSettings.GetConfiguration().sampleRate;
            long AID_RealOffsetTick = (long)AsyncInputData.offsetTick_REAL;
            long AID_OffsetTick = (long)AsyncInputData.offsetTick;

            GUILayout.BeginVertical(GUILayout.MinWidth(320));
            GUILayout.Label(DSPTime_Inter.ToString("f12"));
            GUILayout.Label(DSPTime_Original.ToString("f12"));
            GUILayout.Label((DSPTime_Inter - DSPTime_Original).ToString("f12"));
            GUILayout.Label(DSPTime_Offset.ToString() + "ms  ");
            GUILayout.Label(DSPTime_Precise.ToString() + "ms  ");
            GUILayout.Label(Audio_BufferSize.ToString());
            GUILayout.Label(Audio_SimpleRate.ToString());
            GUILayout.Label("");
            GUILayout.Label(AID_RealOffsetTick.ToString());
            GUILayout.Label(AID_OffsetTick.ToString());
            GUILayout.Label((AID_RealOffsetTick - AID_OffsetTick).ToString().PadLeft(10));
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}
