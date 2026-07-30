using HarmonyLib;
using System.Threading;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace AsyncInputOptimize
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class SafeDSPTime : MonoBehaviour
    {
        private static SafeDSPTime instane;
        internal static void Init(bool _) => Init();
        internal static void Init()
        {
            GameObject obj;
            if (instane != null)
            {
                obj = instane.gameObject;
                Destroy(instane);
            }
            else
            {
                obj = new("[AsyncInputOptimize.dll]InterpolationTime");
            }
            // EntryPoint.logger.Log("Safe DSP Timer Reload");
            DontDestroyOnLoad(obj);
            instane = obj.AddComponent(typeof(SafeDSPTime)) as SafeDSPTime;
        }

        private AudioSource m_source;
        private void Awake()
        {
            PlayerLoopSystem loop = PlayerLoop.GetCurrentPlayerLoop();
            for (int i = 0; i < loop.subSystemList.Length; i++)
            {
                PlayerLoopSystem preUpdate = loop.subSystemList[i];
                if (preUpdate.type == typeof(TimeUpdate))
                {
                    var subSystems = new System.Collections.Generic.List<PlayerLoopSystem>(preUpdate.subSystemList);

                    PlayerLoopSystem myEarlySystem = new PlayerLoopSystem
                    {
                        type = typeof(SafeDSPTime),
                        updateDelegate = SafeDSPTime.UnityUpdate
                    };

                    subSystems.Insert(1, myEarlySystem);
                    preUpdate.subSystemList = subSystems.ToArray();
                    loop.subSystemList[i] = preUpdate;
                    break;
                }
            }
            PlayerLoop.SetPlayerLoop(loop);
        }
        private void Update()
        {
            if (m_source == null || m_source.clip == null)
            {
                m_source = GetComponent<AudioSource>();

                m_source.clip = AudioClip.Create("Runner", 1, 1, 48000, false);
                m_source.loop = true;
                m_source.volume = 0;
                m_source.Play();
            }
        }
        private void OnAudioFilterRead(float[] data, int channels)
        {
            // EntryPoint.logger.Log("Update!!");
            double dsp_time = AudioSettings.dspTime;
            Volatile.Write(ref at_dsptime, dsp_time);
            Volatile.Write(ref at_time, (long)CppBrige.GetSystemTick());
        }

        private static void UnityUpdate()
        {
            AudioConfiguration ac = AudioSettings.GetConfiguration();
            Volatile.Write(ref ut_precise, ac.dspBufferSize / (double)ac.sampleRate);
            Volatile.Write(ref ut_lastmultiply, ut_multiply);
            Volatile.Write(ref ut_multiply, Time.captureFramerate != 0
            ? ((int)(Time.unscaledDeltaTime * 1E7 + 0.1) * 1E-7) / ((int)(Time.captureDeltaTime * 1E7 + 0.1) * 1E-7)
            : ((int)(Time.timeScale * 1E6 + 0.1) * 1E-6));
            Volatile.Write(ref ut_time, (long)CppBrige.GetSystemTick());
        }
        private static double at_dsptime;
        private static long at_time;
        private static double ut_precise;
        private static double ut_multiply;
        private static double ut_lastmultiply;
        private static long ut_time;
        private static long offset;

        public static double GetAuidoPrecise()
        {
            return Volatile.Read(ref ut_precise);
        }

        public static long GetOffset()
        {
            return Volatile.Read(ref offset);
        }
        public static void SetOffset(long value)
        {
            Volatile.Write(ref offset, value);
        }
        public static void AddOffset(long value)
        {
            Volatile.Write(ref offset, Volatile.Read(ref offset) + value);
        }
        public static CodeInstruction ReplaceDSPTime(CodeInstruction ci)
        {
            if (ci.opcode == System.Reflection.Emit.OpCodes.Call && (ci.operand as System.Reflection.MethodInfo) == typeof(AudioSettings).GetProperty("dspTime").GetMethod)
                ci.operand = typeof(SafeDSPTime).GetProperty(nameof(InterpolationDSPTime)).GetMethod;
            return ci;
        }

        public static double DSPTime
        {
            get
            {
                return Volatile.Read(ref at_dsptime) + Volatile.Read(ref SafeDSPTime.offset) / 10_000_000;
            }
        }
        public static double InterpolationDSPTime
        {
            get
            {
                // 其实就是dowhile 但是我不喜欢 所以用goto
            RepeatType:
                long at_time = Volatile.Read(ref SafeDSPTime.at_time);
                long ut_time = Volatile.Read(ref SafeDSPTime.ut_time);
                double dsp = Volatile.Read(ref at_dsptime);
                double multiply = Volatile.Read(ref ut_multiply);
                double lastmultiply = Volatile.Read(ref ut_lastmultiply);
                long offset = Volatile.Read(ref SafeDSPTime.offset);
                long at_time_check = Volatile.Read(ref SafeDSPTime.at_time);
                long ut_time_check = Volatile.Read(ref SafeDSPTime.ut_time);
                if (at_time != at_time_check || ut_time != ut_time_check)
                    goto RepeatType;
                long time = (long)CppBrige.GetSystemTick();
                if (ut_time > at_time)
                {
                    return dsp + ((ut_time - at_time) * lastmultiply + (time - ut_time) * multiply + offset) / 10_000_000.0;
                }
                return dsp + ((time - at_time) * multiply + offset) / 10_000_000.0;
            }
        }

        public static long DSPTimeAsFileTime
        {
            get
            {
                return (long)(Volatile.Read(ref at_dsptime) * 10_000_000.0) + Volatile.Read(ref SafeDSPTime.offset);
            }
        }
        public static long InterpolationDSPTimeAsFileTime
        {
            get
            {
            // 其实就是dowhile 但是我不喜欢 所以用goto
            RepeatType:
                long at_time = Volatile.Read(ref SafeDSPTime.at_time);
                long ut_time = Volatile.Read(ref SafeDSPTime.ut_time);
                double dsp = Volatile.Read(ref at_dsptime);
                double multiply = Volatile.Read(ref ut_multiply);
                double lastmultiply = Volatile.Read(ref ut_lastmultiply);
                long offset = Volatile.Read(ref SafeDSPTime.offset);
                long at_time_check = Volatile.Read(ref SafeDSPTime.at_time);
                long ut_time_check = Volatile.Read(ref SafeDSPTime.ut_time);
                if (at_time != at_time_check || ut_time != ut_time_check)
                    goto RepeatType;
                long time = (long)CppBrige.GetSystemTick();
                if (ut_time > at_time)
                {
                    return (long)(dsp * 10_000_000.0 + (ut_time - at_time) * lastmultiply + (time - ut_time) * multiply + offset);
                }
                return (long)(dsp * 10_000_000.0 + (time - at_time) * multiply + offset);
            }
        }
    }
}
