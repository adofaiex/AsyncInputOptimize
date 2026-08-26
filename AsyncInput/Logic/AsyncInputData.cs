using ModsTagLib.VarArray;
using ModsTagLib.Win32;
using System.Collections.Generic;

namespace AsyncInput.Logic
{
    public static class AsyncInputData
    {
        public static bool enabled;

        public static ulong currFrameNano;
        public static ulong prevFrameNano;
        public static ulong offsetNano;
        public static ulong offsetNano_REAL;
        public static ulong[] offsetNanos = new ulong[30];
        public static int offsetNanosIndex;
        public static double dspTime;

        public static ulong clickTime;

        public static readonly FixedSPSCCircularQueue<AsyncKeyEvent> keyQueue = new(16);
        public static readonly bool[] keyMask = new bool[256];
        public static readonly bool[] frameDependentKeyMask = new bool[256];
        public static readonly HashSet<VirtualKeys> keyDownMask = new();
        public static readonly HashSet<VirtualKeys> keyUpMask = new();
        public static readonly HashSet<VirtualKeys> frameDependentKeyDownMask = new();
        public static readonly HashSet<VirtualKeys> frameDependentKeyUpMask = new();

    }
}
