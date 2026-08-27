using HarmonyLib;
using ModsTagLib.Time;
using System.Threading;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace AsyncInput
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class SafeDSPTime : MonoBehaviour
    {
        static SafeDSPTime()
        {
            pls = new PlayerLoopSystem
            {
                type = typeof(SafeDSPTime),
                updateDelegate = SafeDSPTime.UnityUpdate
            };
        }
        private static PlayerLoopSystem pls;
        private static SafeDSPTime instane;
        internal static void Init(bool _) => Init();
        internal static void Init()
        {
            at_dsptime = 0;
            at_time = 0;
            ut_precise = 0;
            ut_multiply = 0;
            ut_lastmultiply = 0;
            ut_time = 0;
            offset = 0;
            Thread.MemoryBarrier();


            GameObject obj;
            if (instane != null)
            {
                obj = instane.gameObject;
                Destroy(instane);
                Destroy(obj.GetComponent<AudioSource>());
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
            int time_index = -1;
            for (int i = 0; i < loop.subSystemList.Length; i++)
            {
                if (loop.subSystemList[i].type == typeof(TimeUpdate))
                {
                    time_index = i;
                    break;
                }
            }
            if (time_index == -1)
            {
                Starter.instance.log.WARN("TimeUpdate not found");
                return;
            }
            PlayerLoopSystem time_update = loop.subSystemList[time_index];
            PlayerLoopSystem[] time_update_sub = new PlayerLoopSystem[time_update.subSystemList.Length + 1];
            int subtime_index = -1;
            for (int j = 0; j < time_update.subSystemList.Length; j++)
            {
                if (time_update.subSystemList[j].type == typeof(TimeUpdate.WaitForLastPresentationAndUpdateTime))
                    subtime_index = j;
                if (time_update.subSystemList[j].type == typeof(SafeDSPTime))
                    return;
            }
            if (subtime_index == -1)
            {
                Starter.instance.log.WARN("TimeUpdate.WaitForLastPresentationAndUpdateTime not found");
                return;
            }

            for (int j = 0; j < subtime_index; j++)
                time_update_sub[j] = time_update.subSystemList[j];
            for (int j = subtime_index; j < time_update.subSystemList.Length; j++)
                time_update_sub[j + 1] = time_update.subSystemList[j];

            time_update_sub[subtime_index] = pls;
            time_update.subSystemList = time_update_sub;
            loop.subSystemList[time_index] = time_update;
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
            Volatile.Write(ref at_time, TimeInstance.PTime.U_NanoSecond());
        }

        private static void UnityUpdate()
        {
            AudioConfiguration ac = AudioSettings.GetConfiguration();
            Volatile.Write(ref ut_precise, ac.dspBufferSize / (double)ac.sampleRate);
            Volatile.Write(ref ut_lastmultiply, ut_multiply);
            Volatile.Write(ref ut_multiply, Time.captureFramerate != 0
            ? ((int)(Time.unscaledDeltaTime * 1E7 + 0.1) * 1E-7) / ((int)(Time.captureDeltaTime * 1E7 + 0.1) * 1E-7)
            : ((int)(Time.timeScale * 1E6 + 0.1) * 1E-6));
            Volatile.Write(ref ut_time, TimeInstance.PTime.U_NanoSecond());
        }
        private static double at_dsptime;
        private static ulong at_time;
        private static double ut_precise;
        private static double ut_multiply;
        private static double ut_lastmultiply;
        private static ulong ut_time;
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
                return Volatile.Read(ref at_dsptime) + Volatile.Read(ref offset) / TimeConvert.D_Second_Nano;
            }
        }
        public static double InterpolationDSPTime
        {
            get
            {
                // 其实就是dowhile 但是我不喜欢 所以用goto
            RepeatType:
                ulong at_time = Volatile.Read(ref SafeDSPTime.at_time);
                ulong ut_time = Volatile.Read(ref SafeDSPTime.ut_time);
                double dsp = Volatile.Read(ref at_dsptime);
                double multiply = Volatile.Read(ref ut_multiply);
                double lastmultiply = Volatile.Read(ref ut_lastmultiply);
                long offset = Volatile.Read(ref SafeDSPTime.offset);
                ulong at_time_check = Volatile.Read(ref SafeDSPTime.at_time);
                ulong ut_time_check = Volatile.Read(ref SafeDSPTime.ut_time);
                if (at_time != at_time_check || ut_time != ut_time_check)
                    goto RepeatType;
                ulong time = TimeInstance.PTime.U_NanoSecond();
                if (ut_time > at_time)
                {
                    return dsp + ((ut_time - at_time) * lastmultiply + (time - ut_time) * multiply + offset) / TimeConvert.D_Second_Nano;
                }
                return dsp + ((time - at_time) * multiply + offset) / TimeConvert.D_Second_Nano;
            }
        }

        public static long DSPTimeAsNanoTime
        {
            get
            {
                return (long)(Volatile.Read(ref at_dsptime) * TimeConvert.D_Second_Nano) + Volatile.Read(ref offset);
            }
        }
        public static long InterpolationDSPTimeAsNanoTime
        {
            get
            {
                // 其实就是dowhile 但是我不喜欢 所以用goto
            RepeatType:
                ulong at_time = Volatile.Read(ref SafeDSPTime.at_time);
                ulong ut_time = Volatile.Read(ref SafeDSPTime.ut_time);
                double dsp = Volatile.Read(ref at_dsptime);
                double multiply = Volatile.Read(ref ut_multiply);
                double lastmultiply = Volatile.Read(ref ut_lastmultiply);
                long offset = Volatile.Read(ref SafeDSPTime.offset);
                ulong at_time_check = Volatile.Read(ref SafeDSPTime.at_time);
                ulong ut_time_check = Volatile.Read(ref SafeDSPTime.ut_time);
                if (at_time != at_time_check || ut_time != ut_time_check)
                    goto RepeatType;
                ulong time = TimeInstance.PTime.U_NanoSecond();
                if (ut_time > at_time)
                {
                    return (long)(dsp * TimeConvert.D_Second_Nano + (ut_time - at_time) * lastmultiply + (time - ut_time) * multiply + offset);
                }
                return (long)(dsp * TimeConvert.D_Second_Nano + (time - at_time) * multiply + offset);
            }
        }
    }
}
