/*
 * Copy in Cover Mod
 * Assembly: Cover.dll
 * NameSpace: Cover._Core
 * Categoty: StaticClass
 * Name: DSPTimeInterpolation
 * Flag: public auto ansi abstract sealed beforefieldinit flag(200000)
 * Extends: [mscorlib]System.Object
 */
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

namespace AsyncInputOptimize
{
    public static unsafe class DSPTimeInterpolation
    {
        [StructLayout(LayoutKind.Explicit, Pack = 8, Size = 8)]
        private struct VolatileDouble
        {
            public VolatileDouble(double value)
            {
                this.value = (void*)0;
                interlocked = value;
            }
            [FieldOffset(0)]
            private double interlocked;
            [FieldOffset(0)]
            private volatile void* value;
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Set(double value)
            {
                this.value = *(void**)&value;
            }
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly double Get()
            {
                void* val = value;
                return *(double*)&val;
            }
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Add(double value)
            {
                void* val = this.value;
                double val2 = *(double*)&val + value;
                this.value = *(void**)&val2;
            }
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void AddInterlocked(double value)
            {
                Interlocked.Exchange(ref interlocked, interlocked + value);
            }
        }
        [StructLayout(LayoutKind.Explicit, Pack = 8, Size = 8)]
        private struct VolatileLong
        {
            public VolatileLong(long value)
            {
                this.value = (void*)0;
                interlocked = value;
            }
            [FieldOffset(0)]
            private long interlocked;
            [FieldOffset(0)]
            private volatile void* value;
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Set(long value)
            {
                this.value = (void*)value;
            }
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly long Get()
            {
                return (long)value;
            }
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Add(long value)
            {
                this.value = (void*)((long)this.value + value);
            }
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void AddInterlocked(long value)
            {
                Interlocked.Add(ref interlocked, value);
            }
        }

        private static VolatileDouble m_dspTime;
        private static VolatileDouble m_lastDSPTime;
        private static VolatileDouble m_delta;
        private static long m_realTime;
        private static double m_maxOffset;
        private static double m_minOffset;
        // private static readonly ModsTagLib.Unity.VirtualModLog m_log = new("Cover.DSPTimeSimulater");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double TryOutF32Offset(float val)
        {
            return ((long)(val * 1E6 + 0.1)) * 1E-6;
        }
        private static double GetAudioFrequency()
        {
            AudioConfiguration ac = AudioSettings.GetConfiguration();
            return (double)ac.dspBufferSize / (double)ac.sampleRate;
        }
        // private static double Multiply() => Time.captureFramerate != 0
        //     ? scrMisc_64.TryOutF32Offset(Time.unscaledDeltaTime) / (1 / scrMisc_64.TryOutF32Offset(Time.captureDeltaTime))
        //     : scrMisc_64.TryOutF32Offset(Time.timeScale);
        private static double Multiply() => Time.captureFramerate != 0
            ? TryOutF32Offset(Time.unscaledDeltaTime) / (1 / TryOutF32Offset(Time.captureDeltaTime))
            : TryOutF32Offset(Time.timeScale);
        internal static void TUpdate()
        {
            double curr_dsptime = AudioSettings.dspTime;
            m_dspTime.Set(curr_dsptime);
            double last_dsptime = m_lastDSPTime.Get();
            if (curr_dsptime != last_dsptime)
            {
                m_realTime = CppBrige.GetSystemTick();
                double delta = m_realTime - (curr_dsptime * 10_000_000.0);
                if (delta > m_maxOffset)
                {
                    m_minOffset += delta - m_maxOffset;
                    m_maxOffset = delta;
                }
                m_minOffset = delta > m_minOffset ? m_minOffset : delta;
                m_delta.Set(delta - ((m_maxOffset + m_minOffset) / 2));
                m_lastDSPTime.Set(curr_dsptime);
                return;
            }
            m_delta.Add(WorkerThread.MainUpdate.Result.deltaTime * Multiply());
        }

        public static double dspTime
        {
            get
            {
                double d = m_dspTime.Get();
                double ld = m_lastDSPTime.Get();
                while (d != ld)
                {
                    d = m_dspTime.Get();
                    ld = m_lastDSPTime.Get();
                }
                return d + m_delta.Get() / 10_000_000.0;
            }
        }

        public static double dspTimeAsFileTime
        {
            get
            {
                double d = m_dspTime.Get();
                double ld = m_lastDSPTime.Get();
                while (d != ld)
                {
                    d = m_dspTime.Get();
                    ld = m_lastDSPTime.Get();
                }
                return d * 10_000_000.0 + m_delta.Get();
            }
        }
    }
}
