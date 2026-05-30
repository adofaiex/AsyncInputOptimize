namespace AsyncInputOptimize
{
    public static class AsyncInputData
    {
        public static ulong currFrameTick;
        public static ulong prevFrameTick;
        public static ulong offsetTick;
        public static double dspTime;

        public static readonly bool[] keyMask = new bool[256];
        public static readonly bool[] frameDependentKeyMask = new bool[256];

    }
}
