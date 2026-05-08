/*
 * Copy in Cover Mod
 * Assembly: Cover.dll
 * NameSpace: Cover._Core
 * Categoty: StaticClass
 * Name: DSPTimeInterpolation
 * Flag: public auto ansi abstract sealed beforefieldinit flag(200000)
 * Extends: [mscorlib]System.Object
 */
using ADOFAI;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace AsyncInputOptimize
{
    public static unsafe class InterpolationTime
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
        [RequireComponent(typeof(AudioSource))]
        private sealed class Script : MonoBehaviour
        {
            private static Script instane;
            internal static void Init()
            {
                if (instane != null)
                    return;
                GameObject obj = new GameObject("CNM");
                instane = obj.AddComponent(typeof(Script)) as Script;
                DontDestroyOnLoad(obj);
                DontDestroyOnLoad(instane);
            }
            private void Start()
            {
                var source = GetComponent<AudioSource>();

                source.clip = AudioClip.Create("Runner", 1, 1, 48000, false); ;
                source.loop = true;
                source.volume = 0;
                source.Play();
            }
            private void Awake()
            {
                PlayerLoopSystem loop = PlayerLoop.GetCurrentPlayerLoop();
                // 找到 PreUpdate 阶段
                for (int i = 0; i < loop.subSystemList.Length; i++)
                {
                    PlayerLoopSystem preUpdate = loop.subSystemList[i];
                    if (preUpdate.type == typeof(PreUpdate))
                    {
                        var subSystems = new System.Collections.Generic.List<PlayerLoopSystem>(preUpdate.subSystemList);

                        // 创建你的自定义系统
                        PlayerLoopSystem myEarlySystem = new PlayerLoopSystem
                        {
                            type = typeof(InterpolationTime),
                            updateDelegate = InterpolationTime.UnityUpdate
                        };

                        // 插入在 UpdateTime 之后（UpdateTime 通常是 PreUpdate 的第一个子系统）
                        subSystems.Insert(1, myEarlySystem);
                        preUpdate.subSystemList = subSystems.ToArray();
                        loop.subSystemList[i] = preUpdate;
                        break;
                    }
                }
                PlayerLoop.SetPlayerLoop(loop);
            }
            private void OnAudioFilterRead(float[] data, int channels)
            {
                DSPUpdate();
            }
        }

        private static VolatileTime64 m_dspMainUpdateTime = new(1000000000);
        private static VolatileTime64 m_dspTime = new(1000000000);
        private static VolatileTime64 m_dspLastTime = new(1000000000);
        private static VolatileTime64 m_dspDelta = new(1000000000);
        private static double m_dspMaxOffset;
        private static double m_dspMinOffset;
        internal static void DSPUpdate()
        {
            m_dspMainUpdateTime.SetTimeD(AudioSettings.dspTime);
        }

        private static VolatileTime64 m_unityMainUpdateTime = new(1000000000);
        private static VolatileTime64 m_unityTime = new(1000000000);
        private static VolatileTime64 m_unityLastTime = new(1000000000);
        private static VolatileTime64 m_unityDelta = new(1000000000);
        private static double m_unityError;
        private static int m_unitySkip;
        internal static void UnityUpdate()
        {
            m_unityMainUpdateTime.SetTimeD(Time.timeAsDouble);
            m_multiply.SetTimeD(Time.captureFramerate != 0
            ? TryOutF32Offset(Time.unscaledDeltaTime) / TryOutF32Offset(Time.captureDeltaTime)
            : TryOutF32Offset(Time.timeScale));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double TryOutF32Offset(float val)
        {
            return ((long)(val * 1E6 + 0.1)) * 1E-6;
        }

        // private static readonly ModsTagLib.Unity.VirtualModLog m_log = new("Cover.InterpTime");
        private static VolatileTime64 m_multiply = new(1000000000000);

        internal static void Awake()
        {
            Script.Init();
        }

        internal static void TUpdate()
        {
            double tick = WorkerThread.MainUpdate.Result.deltaTime * m_multiply.GetTimeD();

            double dsp = m_dspMainUpdateTime.GetTimeD();
            m_dspTime.SetTimeD(dsp);
            double dsp_curr = m_dspTime.GetTimeD();
            double dsp_last = m_dspLastTime.GetTimeD();
            if (dsp_curr != dsp_last)
            {
                double delta = CppBrige.GetSystemTick() - (dsp_curr * TimeConvert.D_Second_Tick);
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
                    // m_log.WARN("dsp XRUN " + (dsp_curr - dsp_last));
                    m_dspDelta.SetTimeD(m_dspDelta.GetTimeD() + dsp_last - dsp_curr);
                }
                m_dspLastTime.SetTimeD(dsp);
            }
            else
            {
                m_dspDelta.SetTimeD(m_dspDelta.GetTimeD() + tick);

            }

            double unity = m_unityMainUpdateTime.GetTimeD();
            m_unityTime.SetTimeD(unity);
            long unity_curr = m_unityTime.Get();
            long unity_last = m_unityLastTime.Get();
            if (unity_curr < unity_last)
            {
                m_unitySkip++;
            }
            if (unity_curr > unity_last && m_unitySkip > 0)
            {
                m_unitySkip--;
            }
            if (unity_curr > unity_last && m_unitySkip == 0)
            {
                double delta = m_unityDelta.GetTimeD();
                if (unity_last + delta * TimeConvert.I_Tick_Nano > unity_curr)
                {
                    m_unityError = (unity_last + delta * TimeConvert.I_Tick_Nano - unity_curr) / TimeConvert.I_Second_Nano + 0.0000001;
                }
                m_unityDelta.SetTimeD(m_unityError);
                m_unityLastTime.SetTimeD(unity);
            }
            else
            {
                if (m_unityError >= tick)
                {
                    m_unityError -= tick;
                }
                else if (m_unityError != 0)
                {
                    m_unityDelta.SetTimeD(m_unityDelta.GetTimeD() + tick - m_unityError);
                    m_unityError = 0;
                }
                else
                {
                    m_unityDelta.SetTimeD(m_unityDelta.GetTimeD() + tick);
                }
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
                return ct + m_dspDelta.GetTimeD() / TimeConvert.D_Second_Tick;
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
                } while (Math.Abs(ct - lt) > TimeConvert.I_Tick_Nano);
                return (long)(ct / TimeConvert.D_Tick_Nano + m_dspDelta.GetTimeD());
            }
        }
        public static double unityTime
        {
            get
            {
                double ct, lt;
                do
                {
                    ct = m_unityTime.GetTimeD();
                    lt = m_unityLastTime.GetTimeD();
                } while (Math.Abs(ct - lt) > 0.0000001);
                return ct + m_unityDelta.GetTimeD() / TimeConvert.D_Second_Tick;
            }
        }
        public static long unityTimeAsFileTIme
        {
            get
            {
                long ct, lt;
                do
                {
                    ct = m_unityTime.Get();
                    lt = m_unityLastTime.Get();
                } while (Math.Abs(ct - lt) > TimeConvert.I_Tick_Nano);
                return (long)(ct / TimeConvert.D_Tick_Nano + m_unityDelta.GetTimeD());
            }
        }
    }
}
