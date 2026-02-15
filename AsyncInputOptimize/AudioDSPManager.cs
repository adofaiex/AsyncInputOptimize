using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace AsyncInputOptimize
{
    public static class AudioDSPManager
    {
        static AudioDSPManager()
        {
            QueryPerformanceFrequency(out FREQ);
            FREQ = 10000000 / FREQ;
        }
        [DllImport("Kernel32.dll")]
        public static extern void GetSystemTimePreciseAsFileTime(out long val);
        [DllImport("Kernel32.dll")]
        public static extern int QueryPerformanceFrequency(out long val);
        [DllImport("Kernel32.dll")]
        public static extern int QueryPerformanceCounter(out long val);
        private const double HIGH_PRECISE = 1.0 / 750.0;
        private const double MID_PRECISE = 1.0 / 150.0;
        private const double LOW_PRECISE = 1.0 / 30.0;
        private const double SEC_2_TICK = 10000000.0;
        private const int MAX_ERROR_COUNT = 8;
        private const int SINGLE_ROUND = 750;
        private static readonly long FREQ;

        private static double lastTime;
        private static double dspTime;
        private static double dspErrorCounter;
        private static double dspDeltaTime;
        private static int dspDeltaTimeCount;
        private static bool safe;
        public static double cpy_dspTime;
        private static double GetPreciseTime()
        {
            double res;

            QueryPerformanceCounter(out long l);
            l *= FREQ;
            res = Time.captureFramerate > 0
                ? 1.0 / Time.captureFramerate
                : (l - lastTime) / SEC_2_TICK * (((long)(Time.timeScale * 1E6 + 0.1)) * 1E-6);
            lastTime = l;
            return res;
        }
        internal static void Update()
        {
            dspTime += GetPreciseTime();
            dspDeltaTime += AudioSettings.dspTime - dspTime;
            dspDeltaTimeCount++;
            double avg = dspDeltaTime / dspDeltaTimeCount;
            cpy_dspTime = dspTime + avg;
            if (avg > LOW_PRECISE || avg < (-LOW_PRECISE))
            {
                safe = false;
                dspTime = AudioSettings.dspTime - MID_PRECISE / 2;
                dspDeltaTime = 0;
                dspDeltaTimeCount = 0;
                dspErrorCounter = 0;
            }
            else if (avg > MID_PRECISE || avg < (-MID_PRECISE))
            {
                dspErrorCounter++;
                safe = false;
                if (dspErrorCounter >= MAX_ERROR_COUNT)
                {
                    dspTime = AudioSettings.dspTime - MID_PRECISE / 2;
                    dspDeltaTime = 0;
                    dspDeltaTimeCount = 0;
                    dspErrorCounter = 0;
                }
            }
            else if (dspDeltaTimeCount >= SINGLE_ROUND)
            {
                if (!safe && (avg > HIGH_PRECISE || avg< (-HIGH_PRECISE)))
                {
                    dspTime += avg;
                }
                else
                {
                    safe = true;
                }
                dspErrorCounter = 0;
                dspDeltaTime = 0;
                dspDeltaTimeCount = 0;
            }
        }
        public static double GetDSPTime()
        {
            QueryPerformanceCounter(out long l);
            l *= FREQ;
            return dspTime + (l - lastTime) / SEC_2_TICK;
        }
        public static long GetDSPTimeAsFileTime()
        {
            QueryPerformanceCounter(out long l);
            l *= FREQ;
            double curr_dsp = dspTime + (l - lastTime) / SEC_2_TICK;
            return (long)(curr_dsp * SEC_2_TICK);
        }
    }
}
