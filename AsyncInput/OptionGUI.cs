using AsyncInput.Core;
using AsyncInput.Logic;
using ModsTagLib.Unity.ModLayout;
using ModsTagLib.Win32;
using ModsTagLib.Unity;
using System;
using UnityEngine;

namespace AsyncInput
{
    internal static class OptionGUI
    {
        private static GUIEnum info;
        internal enum GUIEnum : ushort
        {
            MainMenu,
            DebugContent
        }
        internal static void Main()
        {
            using (new Using.Horizontal(Starter.instance.guiInstance.transparent_window))
            {
                FastGUI.SelectButton(ref info, GUIEnum.MainMenu, "初始页面");
                FastGUI.SelectButton(ref info, GUIEnum.DebugContent, "调试内容");
            }
            GUIL.SpacePixel(GUIL.WSize * 4);
            if (info == GUIEnum.MainMenu)
                MainMenu();
            else if (info == GUIEnum.DebugContent)
                DebugContent();
        }
        private static void MainMenu()
        {
            if (GUIL.ButtonLarge("如果球不动就用力敲一下我 | if planet pause, click me!"))
            {
                SafeDSPTime.Init();
            }
            using (new FastGUI.SubArea(FastGUI.SubParamter.Default480WEx))
            {
                using (new Using.Horizontal())
                {
                    GUIL.Label("BlackList Mode");
                    if (GUIL.Button(Config.BlackListMode ? "True" : "False"))
                        Config.BlackListMode = !Config.BlackListMode;
                }
                using (new Using.Horizontal())
                {
                    GUIL.Label("Key Limit");
                    if (GUIL.Button(GlobalVar.SelectKeysMode ? "Finish" : "Edit Keys"))
                        GlobalVar.SelectKeysMode = !GlobalVar.SelectKeysMode;
                    GUIL.SpaceChar(2);
                    if (GUIL.Button("Clear All Keys"))
                        GlobalVar.ClearAllKeys = true;
                }
                VirtualKeys[] keys = Config.SelectKeys.ToArray();
                int max = 0;
                for (int i = 0; i < keys.Length; i++)
                {
                    int tmp = DataProcess.VirtualKeysToString(keys[i]).Length;
                    if (tmp > max)
                        max = tmp;
                }
                using (new Using.Indent())
                {
                    using (new FastGUI.SubTabList(FastGUI.SubParamter.Default480WEx))
                    {
                        GUIL.BeginHorizontal();
                        for (int i = 0; i < keys.Length; i++)
                        {
                            if (i != 0 && (i & 7) == 0)
                            {
                                GUIL.EndHorizontal();
                                GUIL.BeginHorizontal();
                            }
                            GUIL.LabelChar(DataProcess.VirtualKeysToString(keys[i]), max);
                            GUIL.SpaceChar(2);
                        }
                        GUIL.EndHorizontal();
                    }
                }
            }
        }
        private static void DebugContent()
        {
            GUIL.BeginHorizontal();
            GUIL.LabelChar("AIData:enabled", 32);
            GUIL.Label(AsyncInputData.enabled.ToString());
            GUIL.EndHorizontal();
            GUIL.BeginHorizontal();
            GUIL.LabelChar("AIData:currFrameTick", 32);
            GUIL.Label(AsyncInputData.currFrameNano.ToString());
            GUIL.EndHorizontal();
            GUIL.BeginHorizontal();
            GUIL.LabelChar("AIData:prevFrameTick", 32);
            GUIL.Label(AsyncInputData.prevFrameNano.ToString());
            GUIL.EndHorizontal();
            GUIL.BeginHorizontal();
            GUIL.LabelChar("AIData:offsetTick", 32);
            GUIL.Label(AsyncInputData.offsetNano.ToString());
            GUIL.EndHorizontal();
            GUIL.BeginHorizontal();
            GUIL.LabelChar("AIData:offsetTick_REAL", 32);
            GUIL.Label(AsyncInputData.offsetNano_REAL.ToString());
            GUIL.EndHorizontal();
            GUIL.BeginHorizontal();
            GUIL.LabelChar("AIData:offsetTicks ", 32);
            GUIL.Label(AsyncInputData.offsetNanos.ToString());
            GUIL.EndHorizontal();
            GUIL.BeginHorizontal();
            GUIL.LabelChar("AIData:offsetTicksIndex", 32);
            GUIL.Label(AsyncInputData.offsetNanosIndex.ToString());
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
    }
}
