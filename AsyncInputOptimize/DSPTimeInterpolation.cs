/*
 * Copy in Cover Mod
 * Assembly: Cover.dll
 * NameSpace: Cover._Core
 * Categoty: StaticClass
 * Name: DSPTimeInterpolation
 * Flag: public auto ansi abstract sealed beforefieldinit flag(200000)
 * Extends: [mscorlib]System.Object
 */
using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

namespace AsyncInputOptimize
{
    public static unsafe class DSPTimeInterpolation
    {
        [StructLayout(LayoutKind.Explicit, Pack = 64, Size = 64)]
        private struct VolatileTime64
        {
            public VolatileTime64(long timeunit)
            {
                this.value = (void*)0;
                interlocked = 0;
                unit = timeunit;
            }
            [FieldOffset(0)]
            private long interlocked;
            [FieldOffset(0)]
            private volatile void* value;
            [FieldOffset(8)]
            private readonly long unit;
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Set(long value)
            {
                this.value = (void*)value;
            }
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetTimeF(float value)
            {
                this.value = (void*)(long)(value * unit);
            }
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetTimeD(double value)
            {
                this.value = (void*)(long)(value * unit);
            }
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly long Get()
            {
                return (long)value;
            }
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly float GetTimeF()
            {
                return (float)(long)value / unit;
            }
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly double GetTimeD()
            {
                return (double)(long)value / unit;
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

        private static VolatileTime64 m_dspTime = new(1000000000);
        private static VolatileTime64 m_dspLastTime = new(1000000000);
        private static VolatileTime64 m_dspDelta = new(1000000000);
        private static double m_dspMaxOffset;
        private static double m_dspMinOffset;

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
            double tick = WorkerThread.MainUpdate.Result.deltaTime * Multiply();

            double dsp = AudioSettings.dspTime;
            if (dsp >= m_dspTime.GetTimeD())
            {
                m_dspTime.SetTimeD(dsp);
            }
            double dsp_curr = m_dspTime.GetTimeD();
            double dsp_last = m_dspLastTime.GetTimeD();
            if (dsp_curr != dsp_last)
            {
                double delta = CppBrige.GetSystemTick() - dsp_curr * 10_000_000.0;
                if (delta > m_dspMaxOffset)
                {
                    m_dspMinOffset += delta - m_dspMaxOffset;
                    m_dspMaxOffset = delta;
                }
                m_dspMinOffset = delta > m_dspMinOffset ? m_dspMinOffset : delta;
                m_dspDelta.SetTimeD(delta - ((m_dspMaxOffset + m_dspMinOffset) / 2));
                AudioConfiguration ac = AudioSettings.GetConfiguration();
                if (dsp_curr - dsp_last - (ac.dspBufferSize * 2 / (double)ac.sampleRate) > 0.0001) // 0.1ms
                {
                    EntryPoint.logger.Log("dsp XRUN " + (dsp_curr - dsp_last));
                    m_dspDelta.SetTimeD(m_dspDelta.GetTimeD() + dsp_last - dsp_curr);
                }
                m_dspLastTime.SetTimeD(dsp);
            }
            else
            {
                m_dspDelta.SetTimeD(m_dspDelta.GetTimeD() + tick);
            }
        }

        public static double dspTime
        {
            get
            {
                double ct, lt;
                do
                {
                    ct = m_dspTime.GetTimeD();
                    lt = m_dspLastTime.GetTimeD();
                } while (Math.Abs(ct - lt) > 0.0000001);
                return ct + m_dspDelta.GetTimeD() / 10_000_000.0;
            }
        }

        public static long dspTimeAsFileTime
        {
            get
            {
                long ct, lt;
                do
                {
                    ct = m_dspTime.Get();
                    lt = m_dspLastTime.Get();
                } while (Math.Abs(ct - lt) > 100);
                return (long)(ct / 100.0 + m_dspDelta.GetTimeD());
            }
        }
    }
}
