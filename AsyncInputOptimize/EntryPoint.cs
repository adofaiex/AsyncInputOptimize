using HarmonyLib;
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
            me.OnToggle = Toggle;
            me.OnUpdate = Update;
            me.OnGUI = GUI;
            cover_is_installer = FindMod("Cover", "Cover.dll");
            CppBrige.Init(me);
        }
        public static bool Toggle(ModEntry me, bool a)
        {
            cover_is_installer = FindMod("Cover", "Cover.dll");
            if (cover_is_installer) return true;
            if (a)
            {
                harmony.PatchAll();
                WorkerThread.Start();
            }
            else
            {
                harmony.UnpatchAll(me.Info.Id);
            }
            return true;
        }
        public static void Update(ModEntry me, float _)
        {
            WorkerThread.Start();
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
            GUILayout.Label("");

            GUILayout.Label("插值线程");
            GUILayout.BeginHorizontal();
            GUILayout.Space(16);
            GUILayout.BeginVertical();
            {
                GUILayout.Label("Mode");
                GUILayout.Label("Mode");
                GUILayout.Label("更新消耗时间");
                GUILayout.Label("每秒更新次数");
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

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
            GUILayout.BeginHorizontal();
            GUILayout.Space(16);
            GUILayout.BeginVertical();
            {
                GUILayout.Label("");
                GUILayout.Label("");
                GUILayout.Label("");
                GUILayout.Label("");
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            double sim_dsptime = DSPTimeInterpolation.dspTime;
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
            GUILayout.Label("");

            GUILayout.Label("");
            GUILayout.BeginHorizontal();
            GUILayout.Space(16);
            GUILayout.BeginVertical();
            {
                if (GUILayout.Button(WorkerThread._unsafeMode ? "<color=#3fffff><b>超高速更新 (<color=#ff0000><b>不安全</b></color>)</b></color>" : "超高速更新 (<color=#ff0000>不安全</color>)"))
                {
                    WorkerThread._unsafeMode = !WorkerThread._unsafeMode;
                    WorkerThread._highMode = false;
                }
                if (GUILayout.Button(WorkerThread._highMode ? "<color=#3fffff><b>高速更新</b></color>" : "高速更新"))
                {
                    WorkerThread._unsafeMode = false;
                    WorkerThread._highMode = !WorkerThread._highMode;
                }
                GUILayout.Label((WorkerThread.MainUpdate.Result.deltaTime / 10000f).ToString("F5") + "ms");
                GUILayout.Label(WorkerThread.MainUpdate.Result.updatePerSecond.ToString().PadLeft(5));
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

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
