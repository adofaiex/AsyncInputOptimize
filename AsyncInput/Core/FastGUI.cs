using ModsTagLib.Storage;
using ModsTagLib.Unity.ModLayout;
using System;
using UnityEngine;

namespace AsyncInput.Core
{
    public static class FastGUI
    {
        public struct SubParamter
        {
            static SubParamter()
            {
                Default480W = new()
                {
                    wEx = false,
                    w = 480,
                    wMin = 480,
                    wMax = -1,
                    hEx = true,
                    h = -1,
                    hMin = -1,
                    hMax = -1
                };
                Default480WEx = new()
                {
                    wEx = true,
                    w = 480,
                    wMin = 480,
                    wMax = -1,
                    hEx = true,
                    h = -1,
                    hMin = -1,
                    hMax = -1
                };
                Default640W = new()
                {
                    wEx = false,
                    w = 640,
                    wMin = 640,
                    wMax = -1,
                    hEx = true,
                    h = -1,
                    hMin = -1,
                    hMax = -1
                };
                Default640WEx = new()
                {
                    wEx = true,
                    w = 640,
                    wMin = 640,
                    wMax = -1,
                    hEx = true,
                    h = -1,
                    hMin = -1,
                    hMax = -1
                };
                EmptyW = new()
                {
                    wEx = false,
                    w = 0,
                    wMin = 0,
                    wMax = -1,
                    hEx = true,
                    h = 0,
                    hMin = 0,
                    hMax = -1
                };
                EmptyWEx = new()
                {
                    wEx = true,
                    w = 0,
                    wMin = 0,
                    wMax = -1,
                    hEx = true,
                    h = 0,
                    hMin = 0,
                    hMax = -1
                };
            }
            public static readonly SubParamter Default480W;
            public static readonly SubParamter Default480WEx;
            public static readonly SubParamter Default640W;
            public static readonly SubParamter Default640WEx;
            public static readonly SubParamter EmptyW;
            public static readonly SubParamter EmptyWEx;

            public static SubParamter CreateW(float val, float min, float max, bool ex)
            {
                return new()
                {
                    wEx = ex,
                    w = val,
                    wMin = min,
                    wMax = max,
                    hEx = true,
                    h = -1,
                    hMin = -1,
                    hMax = -1
                };
            }

            public bool wEx;
            public float wMin;
            public float wMax;
            public float w;
            public bool hEx;
            public float hMin;
            public float hMax;
            public float h;

            internal readonly GUILayoutOption[] GLO()
            {
                System.Collections.Generic.List<GUILayoutOption> glo = new(8);
                if (wMin is >= 0)
                    glo.Add(GUILayout.MinWidth(wMin));
                if (wMax is >= 0)
                    glo.Add(GUILayout.MaxWidth(wMax));
                if (w is >= 0)
                    glo.Add(GUILayout.Width(w));
                glo.Add(GUILayout.ExpandWidth(wEx));
                if (hMin is >= 0)
                    glo.Add(GUILayout.MinHeight(hMin));
                if (hMax is >= 0)
                    glo.Add(GUILayout.MaxHeight(hMax));
                if (h is >= 0)
                    glo.Add(GUILayout.Height(h));
                glo.Add(GUILayout.ExpandWidth(hEx));
                return glo.ToArray();
            }
        }
        private static Texture2D sba_t;
        private static GUIStyle sba_style;
        private static Texture2D sbtl_t;
        private static GUIStyle sbtl_style;
        private static GUIStyle title;
        internal static void Load()
        {
            if (sba_t is null)
            {
                sba_t = new(12, 12);
                Color w = new(1f, 1f, 1f, 1f);
                Color n = new(1f, 1f, 1f, 0f);
                Color[] ccccccc = new Color[12 * 12]
                {
                    n,n,n,w,w,w,w,w,w,n,n,n,
                    n,n,w,n,n,n,n,n,n,w,n,n,
                    n,w,n,n,n,n,n,n,n,n,w,n,
                    w,n,n,n,n,n,n,n,n,n,n,w,
                    w,n,n,n,n,n,n,n,n,n,n,w,
                    w,n,n,n,n,n,n,n,n,n,n,w,
                    w,n,n,n,n,n,n,n,n,n,n,w,
                    w,n,n,n,n,n,n,n,n,n,n,w,
                    w,n,n,n,n,n,n,n,n,n,n,w,
                    n,w,n,n,n,n,n,n,n,n,w,n,
                    n,n,w,n,n,n,n,n,n,w,n,n,
                    n,n,n,w,w,w,w,w,w,n,n,n,
                };
                sba_t.SetPixels(0, 0, 12, 12, ccccccc);
                sba_t.Apply();
            }
            if (sbtl_t is null)
            {
                sbtl_t = new(12, 12);
                Color w = new(1f, 1f, 0.25f, 1f);
                Color n = new(1f, 1f, 1f, 0f);
                Color[] ccccccc = new Color[12 * 12]
                {
                    n,n,n,n,n,n,n,n,n,n,n,n,
                    n,n,n,n,n,n,n,n,n,n,n,n,
                    n,n,n,n,n,n,n,n,n,n,n,n,
                    n,n,n,n,n,n,n,n,n,n,n,n,
                    n,n,n,n,n,n,n,n,n,n,n,n,
                    n,n,n,w,w,w,w,w,w,w,n,n,
                    n,n,w,n,n,n,w,w,w,n,n,n,
                    n,w,n,n,n,n,w,w,n,n,n,n,
                    w,n,n,n,n,n,n,n,n,n,n,n,
                    w,n,n,n,n,n,n,n,n,n,n,n,
                    w,n,n,n,n,n,n,n,n,n,n,n,
                    w,n,n,n,n,n,n,n,n,n,n,n,
                };
                sbtl_t.SetPixels(0, 0, 12, 12, ccccccc);
                sbtl_t.Apply();
            }
            GUIStyleState gss = new();
            gss.background = sba_t;
            sba_style = new(Starter.instance.guiInstance.transparent_window);
            sba_style.normal = gss;
            sba_style.hover = gss;
            sba_style.active = gss;
            sba_style.focused = gss;
            sba_style.border = new RectOffset(4, 4, 4, 4);

            gss = new();
            gss.background = sbtl_t;
            sbtl_style = new(Starter.instance.guiInstance.transparent_window);
            sbtl_style.normal = gss;
            sbtl_style.hover = gss;
            sbtl_style.active = gss;
            sbtl_style.focused = gss;
            sbtl_style.padding = new RectOffset((int)(12 * GUIL.WSize), 0, 0, 0);
            sbtl_style.border = new RectOffset(11, 1, 1, 11);

            title = new(Starter.instance.guiInstance.label);
            title.fontStyle = FontStyle.Bold;
            title.normal.textColor = new(0.25f, 1f, 1f, 1f);
            title.onNormal.textColor = new(0.25f, 1f, 1f, 1f);
            title.active.textColor = new(0.25f, 1f, 1f, 1f);
            title.onActive.textColor = new(0.25f, 1f, 1f, 1f);
            title.hover.textColor = new(0.25f, 1f, 1f, 1f);
            title.onHover.textColor = new(0.25f, 1f, 1f, 1f);
            title.focused.textColor = new(0.25f, 1f, 1f, 1f);
            title.onFocused.textColor = new(0.25f, 1f, 1f, 1f);
        }

        #region sub
        public sealed class SubArea : IDisposable
        {
            public SubArea()
            {
                GUIL.SpacePixel(2 * GUIL.WSize);
                GUIL.BeginHorizontal();
                GUIL.BeginVertical(sba_style ?? Starter.instance.guiInstance.transparent_window, SubParamter.Default640W.GLO());
            }
            public SubArea(float chr_size)
            {
                GUIL.SpacePixel(2 * GUIL.WSize);
                GUIL.BeginHorizontal();
                GUIL.SpaceChar(chr_size);
                GUIL.BeginVertical(sba_style ?? Starter.instance.guiInstance.transparent_window, SubParamter.Default640W.GLO());
            }
            public SubArea(SubParamter sp)
            {
                GUIL.SpacePixel(2 * GUIL.WSize);
                GUIL.BeginHorizontal();
                GUIL.BeginVertical(sba_style ?? Starter.instance.guiInstance.transparent_window, sp.GLO());
            }
            public SubArea(float chr_size, SubParamter sp)
            {
                GUIL.SpacePixel(2 * GUIL.WSize);
                GUIL.BeginHorizontal();
                GUIL.SpaceChar(chr_size);
                GUIL.BeginVertical(sba_style ?? Starter.instance.guiInstance.transparent_window, sp.GLO());
            }
            public void Dispose()
            {
                GUIL.EndVertical();
                GUIL.EndHorizontal();
            }
        }
        public sealed class SubTabList : IDisposable
        {
            public SubTabList()
            {
                GUIL.SpacePixel(3 * GUIL.WSize);
                GUIL.BeginHorizontal();
                GUIL.BeginVertical(sbtl_style ?? Starter.instance.guiInstance.transparent_window, SubParamter.EmptyWEx.GLO());
            }
            public SubTabList(float chr_size)
            {
                GUIL.SpacePixel(3 * GUIL.WSize);
                GUIL.BeginHorizontal();
                GUIL.SpaceChar(chr_size);
                GUIL.BeginVertical(sbtl_style ?? Starter.instance.guiInstance.transparent_window, SubParamter.EmptyWEx.GLO());
            }
            public SubTabList(SubParamter sp)
            {
                GUIL.SpacePixel(3 * GUIL.WSize);
                GUIL.BeginHorizontal();
                GUIL.BeginVertical(sbtl_style ?? Starter.instance.guiInstance.transparent_window, sp.GLO());
            }
            public SubTabList(float chr_size, SubParamter sp)
            {
                GUIL.SpacePixel(3 * GUIL.WSize);
                GUIL.BeginHorizontal();
                GUIL.SpaceChar(chr_size);
                GUIL.BeginVertical(sbtl_style ?? Starter.instance.guiInstance.transparent_window, sp.GLO());
            }
            public void Dispose()
            {
                GUIL.SpacePixel(5 * GUIL.WSize);
                GUIL.EndVertical();
                GUIL.EndHorizontal();
            }
        }
        #endregion

        #region Draw Base
        public static void DrawCustomArea(string title, Action act)
        {
            using (new SubArea(SubParamter.Default480W))
            {
                GUIL.TempStyle = FastGUI.title;
                GUIL.Label(title, false, GUIL.TEMP);
                using (new Using.Indent())
                {
                    act();
                }
            }
        }
        public static void DrawCustomArea(string title, int size, Action act)
        {
            using (new SubArea(SubParamter.CreateW(size, size, -1, false)))
            {
                GUIL.TempStyle = FastGUI.title;
                GUIL.Label(title, false, GUIL.TEMP);
                using (new Using.Indent())
                {
                    act();
                }
            }
        }
        public static void DrawCustomArea(string title, Action remove, Action act)
        {
            using (new SubArea(SubParamter.Default480W))
            {
                GUIL.TempStyle = FastGUI.title;
                GUIL.BeginHorizontal();
                GUIL.Label(title, false, GUIL.TEMP);
                GUIL.Space();
                if (remove is not null && GUIL.Button("X"))
                {
                    remove();
                    GUIL.EndHorizontal();
                    return;
                }
                GUIL.EndHorizontal();
                using (new Using.Indent())
                {
                    act();
                }
            }
        }
        public static void DrawCustomArea(string title, int size, Action remove, Action act)
        {
            using (new SubArea(SubParamter.CreateW(size, size, -1, false)))
            {
                GUIL.TempStyle = FastGUI.title;
                GUIL.BeginHorizontal();
                GUIL.Label(title, false, GUIL.TEMP);
                GUIL.Space();
                if (remove is not null && GUIL.Button("X"))
                {
                    remove();
                    GUIL.EndHorizontal();
                    return;
                }
                GUIL.EndHorizontal();
                using (new Using.Indent())
                {
                    act();
                }
            }
        }
        public static void DrawCustomAreaAs<T>(string title, Action act) where T : IDisposable, new()
        {
            using (new SubArea(SubParamter.Default480W))
            {
                GUIL.TempStyle = FastGUI.title;
                GUIL.Label(title, false, GUIL.TEMP);
                using (new T())
                {
                    act();
                }
            }
        }
        public static void DrawCustomAreaAs<T>(string title, int size, Action act) where T : IDisposable, new()
        {
            using (new SubArea(SubParamter.CreateW(size, size, -1, false)))
            {
                GUIL.TempStyle = FastGUI.title;
                GUIL.Label(title, false, GUIL.TEMP);
                using (new T())
                {
                    act();
                }
            }
        }
        public static void DrawCustomAreaAs<T>(string title, Action remove, Action act) where T : IDisposable, new()
        {
            using (new SubArea(SubParamter.Default480W))
            {
                GUIL.TempStyle = FastGUI.title;
                GUIL.BeginHorizontal();
                GUIL.Label(title, false, GUIL.TEMP);
                GUIL.Space();
                if (remove is not null && GUIL.Button("X"))
                {
                    remove();
                    GUIL.EndHorizontal();
                    return;
                }
                GUIL.EndHorizontal();
                using (new T())
                {
                    act();
                }
            }
        }
        public static void DrawCustomAreaAs<T>(string title, int size, Action remove, Action act) where T : IDisposable, new()
        {
            using (new SubArea(SubParamter.CreateW(size, size, -1, false)))
            {
                GUIL.TempStyle = FastGUI.title;
                GUIL.BeginHorizontal();
                GUIL.Label(title, false, GUIL.TEMP);
                GUIL.Space();
                if (remove is not null && GUIL.Button("X"))
                {
                    remove();
                    GUIL.EndHorizontal();
                    return;
                }
                GUIL.EndHorizontal();
                using (new T())
                {
                    act();
                }
            }
        }
        #endregion
        #region Draw Float
        public static float DrawSizeFloatArea(float value, string title)
        {
            using (new SubArea(SubParamter.Default480W))
            {
                GUIL.TempStyle = FastGUI.title;
                GUIL.Label(title, false, GUIL.TEMP);
                using (new Using.Indent())
                {
                    GUIL.BeginHorizontal();
                    GUIL.LabelChar("- | ", 4);
                    value = GUILEx.HorizontalSliderPlus(value, new(0, 1024, 16));
                    value = GUIL.F32Field(value, new(0, 1024, "F2"));
                    GUIL.EndHorizontal();
                    GUIL.BeginHorizontal();
                    GUIL.LabelChar("- | ", 4);
                    _ = GUILEx.HorizontalSliderPlus(0, new(-1, 1, 0));
                    _ = GUIL.F32Field(0, new(-1, 1, "F2"));
                    GUIL.EndHorizontal();
                }
            }
            return value;
        }
        public static float DrawScaleFloatArea(float value, string title)
        {
            using (new SubArea(SubParamter.Default480W))
            {
                GUIL.TempStyle = FastGUI.title;
                GUIL.Label(title, false, GUIL.TEMP);
                using (new Using.Indent())
                {
                    GUIL.BeginHorizontal();
                    GUIL.LabelChar("- | ", 4);
                    value = GUILEx.HorizontalSliderPlus(value, new(0, 2, 0.125f));
                    value = GUIL.F32Field(value, new(0, 4, "F4"));
                    GUIL.EndHorizontal();
                    GUIL.BeginHorizontal();
                    GUIL.LabelChar("- | ", 4);
                    _ = GUILEx.HorizontalSliderPlus(0, new(-1, 1, 0));
                    _ = GUIL.F32Field(0, new(-1, 1, "F4"));
                    GUIL.EndHorizontal();
                }
            }
            return value;
        }
        #endregion
        #region Draw Vector
        public static Vector2 DrawWWindowsVectorArea(Vector2 vector, string title)
        {
            using (new SubArea(SubParamter.Default480W))
            {
                GUIL.TempStyle = FastGUI.title;
                GUIL.Label(title, false, GUIL.TEMP);
                using (new Using.Indent())
                {
                    GUIL.BeginHorizontal();
                    GUIL.LabelChar("X | ", 4);
                    vector.x = GUILEx.HorizontalSliderPlus(vector.x, new(-(Screen.width * 0.5f), Screen.width * 1.5f, Screen.width / 8));
                    vector.x = GUIL.F32Field(vector.x, new(-Screen.width, Screen.width * 2f, "F2"));
                    GUIL.EndHorizontal();
                    GUIL.BeginHorizontal();
                    GUIL.LabelChar("Y | ", 4);
                    vector.y = GUILEx.HorizontalSliderPlus(vector.y, new(-(Screen.height * 0.5f), Screen.height * 1.5f, Screen.height / 8));
                    vector.y = GUIL.F32Field(vector.y, new(-Screen.height, Screen.height * 2f, "F2"));
                    GUIL.EndHorizontal();
                }
            }
            return vector;
        }
        public static Vector2 DrawWindowsVectorArea(Vector2 vector, string title)
        {
            using (new SubArea(SubParamter.Default480W))
            {
                GUIL.TempStyle = FastGUI.title;
                GUIL.Label(title, false, GUIL.TEMP);
                using (new Using.Indent())
                {
                    GUIL.BeginHorizontal();
                    GUIL.LabelChar("X | ", 4);
                    vector.x = GUILEx.HorizontalSliderPlus(vector.x, new(0, Screen.width, Screen.width / 8));
                    vector.x = GUIL.F32Field(vector.x, new(0, Screen.width, "F2"));
                    GUIL.EndHorizontal();
                    GUIL.BeginHorizontal();
                    GUIL.LabelChar("Y | ", 4);
                    vector.y = GUILEx.HorizontalSliderPlus(vector.y, new(0, Screen.height, Screen.height / 8));
                    vector.y = GUIL.F32Field(vector.y, new(0, Screen.height, "F2"));
                    GUIL.EndHorizontal();
                }
            }
            return vector;
        }
        public static Vector2 DrawSizeVectorArea(Vector2 vector, string title)
        {
            using (new SubArea(SubParamter.Default480W))
            {
                GUIL.TempStyle = FastGUI.title;
                GUIL.Label(title, false, GUIL.TEMP);
                using (new Using.Indent())
                {
                    GUIL.BeginHorizontal();
                    GUIL.LabelChar("X | ", 4);
                    vector.x = GUILEx.HorizontalSliderPlus(vector.x, new(0, 1024, 16));
                    vector.x = GUIL.F32Field(vector.x, new(0, 1024, "F2"));
                    GUIL.EndHorizontal();
                    GUIL.BeginHorizontal();
                    GUIL.LabelChar("Y | ", 4);
                    vector.y = GUILEx.HorizontalSliderPlus(vector.y, new(0, 1024, 16));
                    vector.y = GUIL.F32Field(vector.y, new(0, 1024, "F2"));
                    GUIL.EndHorizontal();
                }
            }
            return vector;
        }
        public static Vector2 DrawScaleVectorArea(Vector2 vector, string title)
        {
            using (new SubArea(SubParamter.Default480W))
            {
                GUIL.TempStyle = FastGUI.title;
                GUIL.Label(title, false, GUIL.TEMP);
                using (new Using.Indent())
                {
                    GUIL.BeginHorizontal();
                    GUIL.LabelChar("X | ", 4);
                    vector.x = GUILEx.HorizontalSliderPlus(vector.x, new(0, 2, 0.125f));
                    vector.x = GUIL.F32Field(vector.x, new(0, 4, "F4"));
                    GUIL.EndHorizontal();
                    GUIL.BeginHorizontal();
                    GUIL.LabelChar("Y | ", 4);
                    vector.y = GUILEx.HorizontalSliderPlus(vector.y, new(0, 2, 0.125f));
                    vector.y = GUIL.F32Field(vector.y, new(0, 4, "F4"));
                    GUIL.EndHorizontal();
                }
            }
            return vector;
        }
        public static Vector2 DrawPositionVectorArea(Vector2 vector, string title, float xmin, float xmax, float ymin, float ymax)
        {
            return DrawPositionVectorArea(vector, title, xmin, xmax, 16, ymin, ymax, 16, 2);
        }
        public static Vector2 DrawPositionVectorArea(Vector2 vector, string title, float xmin, float xmax, float xclip, float ymin, float ymax, float yclip, int precise)
        {
            using (new SubArea(SubParamter.Default480W))
            {
                GUIL.TempStyle = FastGUI.title;
                GUIL.Label(title, false, GUIL.TEMP);
                string p = "F" + precise;
                using (new Using.Indent())
                {
                    GUIL.BeginHorizontal();
                    GUIL.LabelChar("X | ", 4);
                    vector.x = GUILEx.HorizontalSliderPlus(vector.x, new(xmin, xmax, (xmax - xmin) / xclip));
                    vector.x = GUIL.F32Field(vector.x, new(xmin, xmax, p));
                    GUIL.EndHorizontal();
                    GUIL.BeginHorizontal();
                    GUIL.LabelChar("Y | ", 4);
                    vector.y = GUILEx.HorizontalSliderPlus(vector.y, new(ymin, ymax, (ymax - ymin) / yclip));
                    vector.y = GUIL.F32Field(vector.y, new(ymin, ymax, p));
                    GUIL.EndHorizontal();
                }
            }
            return vector;
        }
        public static Vector3 DrawPositionVectorArea(Vector3 vector, string title, float xmin, float xmax, float ymin, float ymax, float zmin, float zmax)
        {
            return DrawPositionVectorArea(vector, title, xmin, xmax, 16, ymin, ymax, 16, zmin, zmax, 16, 2);
        }
        public static Vector3 DrawPositionVectorArea(Vector3 vector, string title, float xmin, float xmax, float xclip, float ymin, float ymax, float yclip, float zmin, float zmax, float zclip, int precise)
        {
            using (new SubArea(SubParamter.Default480W))
            {
                GUIL.TempStyle = FastGUI.title;
                GUIL.Label(title, false, GUIL.TEMP);
                string p = "F" + precise;
                using (new Using.Indent())
                {
                    GUIL.BeginHorizontal();
                    GUIL.LabelChar("X | ", 4);
                    vector.x = GUILEx.HorizontalSliderPlus(vector.x, new(xmin, xmax, (xmax - xmin) / xclip));
                    vector.x = GUIL.F32Field(vector.x, new(xmin, xmax, p));
                    GUIL.EndHorizontal();
                    GUIL.BeginHorizontal();
                    GUIL.LabelChar("Y | ", 4);
                    vector.y = GUILEx.HorizontalSliderPlus(vector.y, new(ymin, ymax, (ymax - ymin) / yclip));
                    vector.y = GUIL.F32Field(vector.y, new(ymin, ymax, p));
                    GUIL.EndHorizontal();
                    GUIL.BeginHorizontal();
                    GUIL.LabelChar("Z | ", 4);
                    vector.z = GUILEx.HorizontalSliderPlus(vector.z, new(zmin, zmax, (zmax - zmin) / zclip));
                    vector.z = GUIL.F32Field(vector.z, new(zmin, zmax, p));
                    GUIL.EndHorizontal();
                }
            }
            return vector;
        }
        public static Color DrawColorArea(Color color, string title)
        {
            using (new SubArea(SubParamter.Default480W))
            {
                GUIL.TempStyle = FastGUI.title;
                GUIL.Label(title, false, GUIL.TEMP);
                GUILEx.LimitSlider<float> slider = new(0f, 1f, 0.0625f);
                LimitFormat<float> field = new(0f, 4f, "F4");
                GUIL.TempStyle = new(Starter.instance.guiInstance.float32Field);
                GUIL.TempStyle.fixedHeight = Starter.instance.guiInstance.byteField.fixedHeight;
                using (new Using.Indent())
                {
                    GUIL.BeginHorizontal();
                    GUIL.LabelChar("<color=#ff0000ff>R</color> | ", 4);
                    color.r = GUILEx.HorizontalSliderPlus(color.r, slider);
                    color.r = GUIL.F32Field(color.r, field, GUIL.TEMP);
                    GUIL.EndHorizontal();
                    GUIL.BeginHorizontal();
                    GUIL.LabelChar("<color=#00ff00ff>G</color> | ", 4);
                    color.g = GUILEx.HorizontalSliderPlus(color.g, slider);
                    color.g = GUIL.F32Field(color.g, field, GUIL.TEMP);
                    GUIL.EndHorizontal();
                    GUIL.BeginHorizontal();
                    GUIL.LabelChar("<color=#0000ffff>B</color> | ", 4);
                    color.b = GUILEx.HorizontalSliderPlus(color.b, slider);
                    color.b = GUIL.F32Field(color.b, field, GUIL.TEMP);
                    GUIL.EndHorizontal();
                    GUIL.BeginHorizontal();
                    GUIL.LabelChar("<color=#ffffffff>A</color> | ", 4);
                    color.a = GUILEx.HorizontalSliderPlus(color.a, slider);
                    color.a = GUIL.F32Field(color.a, field, GUIL.TEMP);
                    GUIL.EndHorizontal();
                    GUIL.BeginHorizontal();
                    Color32 c = color;
                    GUIL.Label("#" + Convert.ToString(c.r, 16).PadLeft(2, '0') + Convert.ToString(c.g, 16).PadLeft(2, '0') + Convert.ToString(c.b, 16).PadLeft(2, '0') + Convert.ToString(c.a, 16).PadLeft(2, '0'));
                    GUIL.SpaceChar(4);
                    GUIL.PopupColor(ref color);
                    GUIL.EndHorizontal();
                }
            }
            return color;
        }
        public static Color32 DrawColorArea(Color32 color, string title)
        {
            using (new SubArea(SubParamter.Default480W))
            {
                GUIL.TempStyle = FastGUI.title;
                GUIL.Label(title, false, GUIL.TEMP);
                GUILEx.LimitSlider<int> slider = new(0, 255, 16);
                Limit<byte> field = new(0, 255);
                using (new Using.Indent())
                {
                    GUIL.BeginHorizontal();
                    GUIL.LabelChar("<color=#ff0000ff>R</color> | ", 4);
                    color.r = (byte)GUILEx.HorizontalSliderPlus(color.r, slider);
                    color.r = GUIL.ByteField(color.r, field);
                    GUIL.EndHorizontal();
                    GUIL.BeginHorizontal();
                    GUIL.LabelChar("<color=#00ff00ff>G</color> | ", 4);
                    color.g = (byte)GUILEx.HorizontalSliderPlus(color.g, slider);
                    color.g = GUIL.ByteField(color.g, field);
                    GUIL.EndHorizontal();
                    GUIL.BeginHorizontal();
                    GUIL.LabelChar("<color=#0000ffff>B</color> | ", 4);
                    color.b = (byte)GUILEx.HorizontalSliderPlus(color.b, slider);
                    color.b = GUIL.ByteField(color.b, field);
                    GUIL.EndHorizontal();
                    GUIL.BeginHorizontal();
                    GUIL.LabelChar("<color=#ffffffff>A</color> | ", 4);
                    color.a = (byte)GUILEx.HorizontalSliderPlus(color.a, slider);
                    color.a = GUIL.ByteField(color.a, field);
                    GUIL.EndHorizontal();
                    GUIL.BeginHorizontal();
                    GUIL.Label("#" + Convert.ToString(color.r, 16).PadLeft(2, '0') + Convert.ToString(color.g, 16).PadLeft(2, '0') + Convert.ToString(color.b, 16).PadLeft(2, '0') + Convert.ToString(color.a, 16).PadLeft(2, '0'));
                    GUIL.SpaceChar(4);
                    Color c = color;
                    GUIL.PopupColor(ref c);
                    color = c;
                    GUIL.EndHorizontal();
                }
            }
            return color;
        }
        #endregion


        public static void Horizontal(Action action)
        {
            GUIL.BeginHorizontal();
            action?.Invoke();
            GUIL.EndHorizontal();
        }
        public static void HorizontalSpace(Action action)
        {
            GUIL.BeginHorizontal();
            action?.Invoke();
            GUIL.Space();
            GUIL.EndHorizontal();
        }
        public static void Horizontal(SStorage<string, int> label, Action action)
        {
            GUIL.BeginHorizontal();
            GUIL.LabelChar(label.Item1, label.Item2);
            action?.Invoke();
            GUIL.EndHorizontal();
        }
        public static void HorizontalSpace(SStorage<string, int> label, Action action)
        {
            GUIL.BeginHorizontal();
            GUIL.LabelChar(label.Item1, label.Item2);
            action?.Invoke();
            GUIL.Space();
            GUIL.EndHorizontal();
        }
        public static void Horizontal(string text, int size = 0, Action action = null)
        {
            GUIL.BeginHorizontal();
            GUIL.LabelChar(text, size);
            action?.Invoke();
            GUIL.EndHorizontal();
        }
        public static void HorizontalSpace(string text, int size = 0, Action action = null)
        {
            GUIL.BeginHorizontal();
            GUIL.LabelChar(text, size);
            action?.Invoke();
            GUIL.Space();
            GUIL.EndHorizontal();
        }
        public static void nh_TextTitle(string text, int size = 0)
        {
            GUIStyle s = new(Starter.instance.guiInstance.label)
            {
                fontStyle = FontStyle.Bold,
                fixedWidth = GUIL.WidthCharSize * size
            };
            s.normal.textColor = new(0.25f, 0.75f, 1f);
            s.hover.textColor = new(0.25f, 0.75f, 1f);
            s.focused.textColor = new(0.25f, 0.75f, 1f);
            s.active.textColor = new(0.25f, 0.75f, 1f);
            GUIL.TempStyle = s;
            GUIL.Label(text, false, GUIL.TEMP);
        }
        public static void SelectButton<T>(ref T in_out, T trigger, string name) where T : struct, Enum
        {
            if (GUIL.ButtonLarge(in_out.Equals(trigger) ? $"[{name}]" : $" {name} "))
                in_out = trigger;
            GUIL.SpacePixel(1);
        }
        #region struct
        public static SStorage<int, int> Position(SStorage<int, int> __result, int delta_size)
        {
            SStorage<float, float> pos = new(__result.Item1 / 100f, __result.Item2 / 100f);
            pos = Position(pos, "Pos", delta_size);
            pos.Item1 *= 100;
            pos.Item2 *= 100;
            __result.Update((int)pos.Item1, (int)pos.Item2);
            return __result;
        }
        public static SStorage<float, float> Position(SStorage<float, float> __result, string name, int delta_size)
        {
            GUIL.BeginHorizontal();
            GUIL.LabelChar($"XY {name}", delta_size);
            __result.Item1 = GUILEx.HorizontalSliderPlus(__result.Item1, new(0, Screen.width, 60));
            __result.Item1 = GUIL.F32Field(__result.Item1, new(-Screen.width, Screen.width * 2, "f4"));
            __result.Item2 = GUILEx.HorizontalSliderPlus(__result.Item2, new(0, Screen.height, 60));
            __result.Item2 = GUIL.F32Field(__result.Item2, new(-Screen.height, Screen.height * 2, "f4"));
            GUIL.EndHorizontal();
            return __result;
        }
        public static SStorage<float, float, float> Position3D(SStorage<float, float, float> __result, int delta_size)
        {
            return Position3D(__result, "Pos", delta_size);
        }
        public static SStorage<float, float, float> Position3D(SStorage<float, float, float> __result, string name, int delta_size)
        {
            GUIL.BeginHorizontal();
            GUIL.LabelChar($"XYZ {name}", delta_size);
            __result.Item1 = GUILEx.HorizontalSliderPlus(__result.Item1, new(0, Screen.width, 60));
            __result.Item1 = GUIL.F32Field(__result.Item1, new(-Screen.width, Screen.width * 2, "f4"));
            __result.Item2 = GUILEx.HorizontalSliderPlus(__result.Item2, new(0, Screen.height, 60));
            __result.Item2 = GUIL.F32Field(__result.Item2, new(-Screen.height, Screen.height * 2, "f4"));
            __result.Item3 = GUILEx.HorizontalSliderPlus(__result.Item3, new(-256, 256, 4));
            __result.Item3 = GUIL.F32Field(__result.Item3, new(-256, 256, "f4"));
            GUIL.EndHorizontal();
            return __result;
        }
        public static SStorage<float, float> SizePerWindows(SStorage<float, float> __result, string name, int delta_size)
        {
            GUIL.BeginHorizontal();
            GUIL.LabelChar($"XY {name}", delta_size);
            __result.Item1 = GUILEx.HorizontalSliderPlus(__result.Item1, new(0, Screen.width, 60));
            __result.Item1 = GUIL.F32Field(__result.Item1, new(0, Screen.width * 2, "f2"));
            __result.Item2 = GUILEx.HorizontalSliderPlus(__result.Item2, new(0, Screen.height, 60));
            __result.Item2 = GUIL.F32Field(__result.Item2, new(0, Screen.height * 2, "f2"));
            GUIL.EndHorizontal();
            return __result;
        }
        public static SStorage<float, float> SizePer100(SStorage<float, float> __result, string name, int delta_size)
        {
            return Game2D(__result, name, delta_size, new(0, 2, "f8"), new(0, 1, 0.0675f));
        }
        public static SStorage<float, float> Game2D(SStorage<float, float> __result, string name, int delta_size, LimitFormat<float> limit = null, GUILEx.LimitSlider<float> sliderlimit = null)
        {
            sliderlimit ??= new(0, 512, 32);
            limit ??= new(0, 1024, "f4");
            GUIL.BeginHorizontal();
            GUIL.LabelChar($"XY {name}", delta_size);
            __result.Item1 = GUILEx.HorizontalSliderPlus(__result.Item1, sliderlimit);
            __result.Item1 = GUIL.F32Field(__result.Item1, limit);
            __result.Item2 = GUILEx.HorizontalSliderPlus(__result.Item2, sliderlimit);
            __result.Item2 = GUIL.F32Field(__result.Item2, limit);
            GUIL.EndHorizontal();
            return __result;
        }
        public static SStorage<ushort, ushort> Game2D(SStorage<ushort, ushort> __result, string name, int delta_size, Limit<ushort> limit = null, GUILEx.LimitSlider<float> sliderlimit = null)
        {
            sliderlimit ??= new(0, 512, 32);
            limit ??= new(0, 1024);
            GUIL.BeginHorizontal();
            GUIL.LabelChar($"XY {name}", delta_size);
            __result.Item1 = (ushort)GUILEx.HorizontalSliderPlus(__result.Item1, sliderlimit);
            __result.Item1 = GUIL.U16Field(__result.Item1, limit);
            __result.Item2 = (ushort)GUILEx.HorizontalSliderPlus(__result.Item2, sliderlimit);
            __result.Item2 = GUIL.U16Field(__result.Item2, limit);
            GUIL.EndHorizontal();
            return __result;
        }
        public static SStorage<byte, byte> Game2D(SStorage<byte, byte> __result, string name, int delta_size, Limit<byte> limit = null, GUILEx.LimitSlider<float> sliderlimit = null)
        {
            sliderlimit ??= new(0, 255, 32);
            limit ??= new(0, 255);
            GUIL.BeginHorizontal();
            GUIL.LabelChar($"XY {name}", delta_size);
            __result.Item1 = (byte)GUILEx.HorizontalSliderPlus(__result.Item1, sliderlimit);
            __result.Item1 = GUIL.ByteField(__result.Item1, limit);
            __result.Item2 = (byte)GUILEx.HorizontalSliderPlus(__result.Item2, sliderlimit);
            __result.Item2 = GUIL.ByteField(__result.Item2, limit);
            GUIL.EndHorizontal();
            return __result;
        }
        public static SStorage<float, float, float> Game3D(SStorage<float, float, float> __result, string name, int delta_size, LimitFormat<float> limit = null, GUILEx.LimitSlider<float> sliderlimit = null)
        {
            sliderlimit ??= new(0, 512, 32);
            limit ??= new(0, 1024, "f4");
            GUIL.BeginHorizontal();
            GUIL.LabelChar($"XYZ {name}", delta_size);
            __result.Item1 = GUILEx.HorizontalSliderPlus(__result.Item1, sliderlimit);
            __result.Item1 = GUIL.F32Field(__result.Item1, limit);
            __result.Item2 = GUILEx.HorizontalSliderPlus(__result.Item2, sliderlimit);
            __result.Item2 = GUIL.F32Field(__result.Item2, limit);
            __result.Item3 = GUILEx.HorizontalSliderPlus(__result.Item3, sliderlimit);
            __result.Item3 = GUIL.F32Field(__result.Item3, limit);
            GUIL.EndHorizontal();
            return __result;
        }
        public static SStorage<ushort, ushort, ushort> Game3D(SStorage<ushort, ushort, ushort> __result, string name, int delta_size, Limit<ushort> limit = null, GUILEx.LimitSlider<float> sliderlimit = null)
        {
            sliderlimit ??= new(0, 512, 32);
            limit ??= new(0, 1024);
            GUIL.BeginHorizontal();
            GUIL.LabelChar($"XYZ {name}", delta_size);
            __result.Item1 = (ushort)GUILEx.HorizontalSliderPlus(__result.Item1, sliderlimit);
            __result.Item1 = GUIL.U16Field(__result.Item1, limit);
            __result.Item2 = (ushort)GUILEx.HorizontalSliderPlus(__result.Item2, sliderlimit);
            __result.Item2 = GUIL.U16Field(__result.Item2, limit);
            __result.Item3 = (ushort)GUILEx.HorizontalSliderPlus(__result.Item3, sliderlimit);
            __result.Item3 = GUIL.U16Field(__result.Item3, limit);
            GUIL.EndHorizontal();
            return __result;
        }
        public static SStorage<byte, byte, byte> Game3D(SStorage<byte, byte, byte> __result, string name, int delta_size, Limit<byte> limit = null, GUILEx.LimitSlider<float> sliderlimit = null)
        {
            sliderlimit ??= new(0, 255, 32);
            limit ??= new(0, 255);
            GUIL.BeginHorizontal();
            GUIL.LabelChar($"XYZ {name}", delta_size);
            __result.Item1 = (byte)GUILEx.HorizontalSliderPlus(__result.Item1, sliderlimit);
            __result.Item1 = GUIL.ByteField(__result.Item1, limit);
            __result.Item2 = (byte)GUILEx.HorizontalSliderPlus(__result.Item2, sliderlimit);
            __result.Item2 = GUIL.ByteField(__result.Item2, limit);
            __result.Item3 = (byte)GUILEx.HorizontalSliderPlus(__result.Item3, sliderlimit);
            __result.Item3 = GUIL.ByteField(__result.Item3, limit);
            GUIL.EndHorizontal();
            return __result;
        }
        #endregion
        #region class
        public static void Position(CStorage<int, int> __result, int delta_size)
        {
            SStorage<float, float> pos = new(__result.Item1 / 100f, __result.Item2 / 100f);
            pos = Position(pos, "Pos", delta_size);
            pos.Item1 *= 100;
            pos.Item2 *= 100;
            __result.Update((int)pos.Item1, (int)pos.Item2);
        }
        public static void Position(CStorage<float, float> __result, string name, int delta_size)
        {
            GUIL.BeginHorizontal();
            GUIL.LabelChar($"XY {name}", delta_size);
            __result.Item1 = GUILEx.HorizontalSliderPlus(__result.Item1, new(0, Screen.width, 60));
            __result.Item1 = GUIL.F32Field(__result.Item1, new(-Screen.width, Screen.width * 2, "f4"));
            __result.Item2 = GUILEx.HorizontalSliderPlus(__result.Item2, new(0, Screen.height, 60));
            __result.Item2 = GUIL.F32Field(__result.Item2, new(-Screen.height, Screen.height * 2, "f4"));
            GUIL.EndHorizontal();
        }
        public static void Position3D(CStorage<float, float, float> __result, int delta_size)
        {
            Position3D(__result, "Pos", delta_size);
        }
        public static void Position3D(CStorage<float, float, float> __result, string name, int delta_size)
        {
            GUIL.BeginHorizontal();
            GUIL.LabelChar($"XYZ {name}", delta_size);
            __result.Item1 = GUILEx.HorizontalSliderPlus(__result.Item1, new(0, Screen.width, 60));
            __result.Item1 = GUIL.F32Field(__result.Item1, new(-Screen.width, Screen.width * 2, "f4"));
            __result.Item2 = GUILEx.HorizontalSliderPlus(__result.Item2, new(0, Screen.height, 60));
            __result.Item2 = GUIL.F32Field(__result.Item2, new(-Screen.height, Screen.height * 2, "f4"));
            __result.Item3 = GUILEx.HorizontalSliderPlus(__result.Item3, new(-256, 256, 4));
            __result.Item3 = GUIL.F32Field(__result.Item3, new(-256, 256, "f4"));
            GUIL.EndHorizontal();
        }
        public static void SizePerWindows(CStorage<float, float> __result, string name, int delta_size)
        {
            GUIL.BeginHorizontal();
            GUIL.LabelChar($"XY {name}", delta_size);
            __result.Item1 = GUILEx.HorizontalSliderPlus(__result.Item1, new(0, Screen.width, 60));
            __result.Item1 = GUIL.F32Field(__result.Item1, new(0, Screen.width * 2, "f2"));
            __result.Item2 = GUILEx.HorizontalSliderPlus(__result.Item2, new(0, Screen.height, 60));
            __result.Item2 = GUIL.F32Field(__result.Item2, new(0, Screen.height * 2, "f2"));
            GUIL.EndHorizontal();
        }
        public static void SizePer100(CStorage<float, float> __result, string name, int delta_size)
        {
            Game2D(__result, name, delta_size, new(0, 2, "f8"), new(0, 1, 0.0675f));
        }
        public static void Game2D(CStorage<float, float> __result, string name, int delta_size, LimitFormat<float> limit = null, GUILEx.LimitSlider<float> sliderlimit = null)
        {
            sliderlimit ??= new(0, 512, 32);
            limit ??= new(0, 1024, "f4");
            GUIL.BeginHorizontal();
            GUIL.LabelChar($"XY {name}", delta_size);
            __result.Item1 = GUILEx.HorizontalSliderPlus(__result.Item1, sliderlimit);
            __result.Item1 = GUIL.F32Field(__result.Item1, limit);
            __result.Item2 = GUILEx.HorizontalSliderPlus(__result.Item2, sliderlimit);
            __result.Item2 = GUIL.F32Field(__result.Item2, limit);
            GUIL.EndHorizontal();
        }
        public static void Game2D(CStorage<ushort, ushort> __result, string name, int delta_size, Limit<ushort> limit = null, GUILEx.LimitSlider<float> sliderlimit = null)
        {
            sliderlimit ??= new(0, 512, 32);
            limit ??= new(0, 1024);
            GUIL.BeginHorizontal();
            GUIL.LabelChar($"XY {name}", delta_size);
            __result.Item1 = (ushort)GUILEx.HorizontalSliderPlus(__result.Item1, sliderlimit);
            __result.Item1 = GUIL.U16Field(__result.Item1, limit);
            __result.Item2 = (ushort)GUILEx.HorizontalSliderPlus(__result.Item2, sliderlimit);
            __result.Item2 = GUIL.U16Field(__result.Item2, limit);
            GUIL.EndHorizontal();
        }
        public static void Game2D(CStorage<byte, byte> __result, string name, int delta_size, Limit<byte> limit = null, GUILEx.LimitSlider<float> sliderlimit = null)
        {
            sliderlimit ??= new(0, 255, 32);
            limit ??= new(0, 255);
            GUIL.BeginHorizontal();
            GUIL.LabelChar($"XY {name}", delta_size);
            __result.Item1 = (byte)GUILEx.HorizontalSliderPlus(__result.Item1, sliderlimit);
            __result.Item1 = GUIL.ByteField(__result.Item1, limit);
            __result.Item2 = (byte)GUILEx.HorizontalSliderPlus(__result.Item2, sliderlimit);
            __result.Item2 = GUIL.ByteField(__result.Item2, limit);
            GUIL.EndHorizontal();
        }
        public static void Game3D(CStorage<float, float, float> __result, string name, int delta_size, LimitFormat<float> limit = null, GUILEx.LimitSlider<float> sliderlimit = null)
        {
            sliderlimit ??= new(0, 512, 32);
            limit ??= new(0, 1024, "f4");
            GUIL.BeginHorizontal();
            GUIL.LabelChar($"XYZ {name}", delta_size);
            __result.Item1 = GUILEx.HorizontalSliderPlus(__result.Item1, sliderlimit);
            __result.Item1 = GUIL.F32Field(__result.Item1, limit);
            __result.Item2 = GUILEx.HorizontalSliderPlus(__result.Item2, sliderlimit);
            __result.Item2 = GUIL.F32Field(__result.Item2, limit);
            __result.Item3 = GUILEx.HorizontalSliderPlus(__result.Item3, sliderlimit);
            __result.Item3 = GUIL.F32Field(__result.Item3, limit);
            GUIL.EndHorizontal();
        }
        public static void Game3D(CStorage<ushort, ushort, ushort> __result, string name, int delta_size, Limit<ushort> limit = null, GUILEx.LimitSlider<float> sliderlimit = null)
        {
            sliderlimit ??= new(0, 512, 32);
            limit ??= new(0, 1024);
            GUIL.BeginHorizontal();
            GUIL.LabelChar($"XYZ {name}", delta_size);
            __result.Item1 = (ushort)GUILEx.HorizontalSliderPlus(__result.Item1, sliderlimit);
            __result.Item1 = GUIL.U16Field(__result.Item1, limit);
            __result.Item2 = (ushort)GUILEx.HorizontalSliderPlus(__result.Item2, sliderlimit);
            __result.Item2 = GUIL.U16Field(__result.Item2, limit);
            __result.Item3 = (ushort)GUILEx.HorizontalSliderPlus(__result.Item3, sliderlimit);
            __result.Item3 = GUIL.U16Field(__result.Item3, limit);
            GUIL.EndHorizontal();
        }
        public static void Game3D(CStorage<byte, byte, byte> __result, string name, int delta_size, Limit<byte> limit = null, GUILEx.LimitSlider<float> sliderlimit = null)
        {
            sliderlimit ??= new(0, 255, 32);
            limit ??= new(0, 255);
            GUIL.BeginHorizontal();
            GUIL.LabelChar($"XYZ {name}", delta_size);
            __result.Item1 = (byte)GUILEx.HorizontalSliderPlus(__result.Item1, sliderlimit);
            __result.Item1 = GUIL.ByteField(__result.Item1, limit);
            __result.Item2 = (byte)GUILEx.HorizontalSliderPlus(__result.Item2, sliderlimit);
            __result.Item2 = GUIL.ByteField(__result.Item2, limit);
            __result.Item3 = (byte)GUILEx.HorizontalSliderPlus(__result.Item3, sliderlimit);
            __result.Item3 = GUIL.ByteField(__result.Item3, limit);
            GUIL.EndHorizontal();
        }
        #endregion
    }
}
