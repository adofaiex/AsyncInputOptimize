/*
 * Copy in ModsTagLib.Unity Mod
 * Assembly: ModsTagLib.Unity.dll
 * NameSpace: ModsTagLib.Unity.Threading
 * Categoty: StaticClass
 * Name: CalculateThread
 * Flag: public auto ansi abstract sealed beforefieldinit flag(200000)
 * Extends: [mscorlib]System.Object
 */

using System.Threading;

namespace AsyncInputOptimize
{
    internal static unsafe class WorkerThread
    {
        public static readonly TimeMethodCounter MainUpdate = new(&DSPTimeInterpolation.TUpdate);

        internal static Thread CurrentThread;
        internal static byte[] Config
        {
            get
            {
                return new byte[2] { _unsafeMode ? (byte)1 : (byte)0, _highMode ? (byte)1 : (byte)0 };
            }
            set
            {
                _unsafeMode = value[0] != 0;
                _highMode = value[1] != 0;
            }
        }
        public static void Restart()
        {
            Stop();
            Start();
        }
        public static void Start()
        {
            if (m_running) return;
            m_running = true;
            CurrentThread = new Thread(new ThreadStart(Action), 1048576);
            CurrentThread.Start();
        }
        public static void Stop()
        {
            m_stopping = true;
        }

        private static volatile bool m_running;
        private static volatile bool m_stopping;

        internal static volatile bool _unsafeMode;
        internal static volatile bool _highMode;

        private static unsafe void Action()
        {
            long lt = CppBrige.GetSystemTick();
            long ct = CppBrige.GetSystemTick();
            long r = 0;
            MainUpdate.Reload();
            while (!m_stopping)
            {
                try
                {
                    MainUpdate.Update();
                }
                catch (System.Exception e)
                {
                    EntryPoint.logger.LogException(e);
                }
                if (_unsafeMode)
                {
                    r = 0;
                    lt = CppBrige.GetSystemTick();
                }
                else
                {
                    ct = CppBrige.GetSystemTick();
                    r = (_highMode ? 500 : 10_000) - (ct - lt) + r;
                    r = r > 0 ? (_highMode ? CppBrige.HighSleep(r) : CppBrige.LowSleep(r)) : r > -10000000 ? r : 0;
                    lt = CppBrige.GetSystemTick();
                }
            }
            m_running = false;
        }
    }
}
