using System;
using System.Runtime.InteropServices;
using System.Security;

namespace AsyncInputOptimize
{
    public static class CppBrige
    {
#if WIN32
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        private static extern IntPtr LoadLibrary(string lpFileName);
        internal static void Init(UnityModManagerNet.UnityModManager.ModEntry me)
        {
            LoadLibrary($"{me.Path}\\AsyncInputOptimize_CppCore.dll");
        }

        [DllImport("AsyncInputOptimize_CppCore.dll"), SuppressUnmanagedCodeSecurity]
        public static extern long HighSleep(long value);
        [DllImport("AsyncInputOptimize_CppCore.dll"), SuppressUnmanagedCodeSecurity]
        public static extern long LowSleep(long value);
        [DllImport("AsyncInputOptimize_CppCore.dll"), SuppressUnmanagedCodeSecurity]
        public static extern long GetSystemTick();
        [DllImport("AsyncInputOptimize_CppCore.dll"), SuppressUnmanagedCodeSecurity]
        public static extern void GOGOGO_GO_TO_CRASH______________();
#endif
    }
}
