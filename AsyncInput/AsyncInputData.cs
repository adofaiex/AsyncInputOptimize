using ModsTagLib.VarArray;
using ModsTagLib.Win32;
using System.Collections.Generic;

namespace AsyncInput
{
    public static class AsyncInputData
    {
        public static bool enabled;

        public static ulong currFrameTick;
        public static ulong prevFrameTick;
        public static ulong offsetTick;
        public static double dspTime;

        public static readonly SinglePCCircularQueue_Const<AsyncKeyEvent> keyQueue = new(16);
        public static readonly bool[] keyMask = new bool[256];
        public static readonly bool[] frameDependentKeyMask = new bool[256];
        public static readonly HashSet<VirtualKeys> keyDownMask = new();
        public static readonly HashSet<VirtualKeys> keyUpMask = new();
        public static readonly HashSet<VirtualKeys> frameDependentKeyDownMask = new();
        public static readonly HashSet<VirtualKeys> frameDependentKeyUpMask = new();

    }
}
