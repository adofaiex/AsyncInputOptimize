using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace AsyncInputOptimize
{
    public static class CppBrige
    {
#if WIN32
        [DllImport("Kernel32.dll"), SuppressUnmanagedCodeSecurity]
        public static extern void GetSystemTimePreciseAsFileTime(out ulong lpTime);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetSystemTick()
        {
            GetSystemTimePreciseAsFileTime(out var res);
            return res;
        }
#endif
    }
}
