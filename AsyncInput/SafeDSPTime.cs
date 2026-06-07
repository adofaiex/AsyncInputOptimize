using HarmonyLib;
using ModsTagLib;
using System.Threading;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace AsyncInput
{
    [RequireComponent(typeof(AudioSource))]
    public sealed unsafe class SafeDSPTime : MonoBehaviour
    {
        private static SafeDSPTime instane;
        internal static void Init()
        {
            if (instane != null)
                return;
            GameObject obj = new("[AsyncInput.dll]InterpolationTime");
            DontDestroyOnLoad(obj);
            instane = obj.AddComponent(typeof(SafeDSPTime)) as SafeDSPTime;
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
                if (preUpdate.type == typeof(TimeUpdate))
                {
                    var subSystems = new System.Collections.Generic.List<PlayerLoopSystem>(preUpdate.subSystemList);

                    // 创建你的自定义系统
                    PlayerLoopSystem myEarlySystem = new PlayerLoopSystem
                    {
                        type = typeof(SafeDSPTime),
                        updateDelegate = SafeDSPTime.UnityUpdate
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
            double dsp_time = AudioSettings.dspTime;
            Volatile.Write(ref at_dsptime, dsp_time);
            Volatile.Write(ref at_time, ModsTagCLib.PreciseFileTime());
        }

        private static void UnityUpdate()
        {
            AudioConfiguration ac = AudioSettings.GetConfiguration();
            Volatile.Write(ref ut_precise, ac.dspBufferSize / (double)ac.sampleRate);
            Volatile.Write(ref ut_lastmultiply, ut_multiply);
            Volatile.Write(ref ut_multiply, Time.captureFramerate != 0
            ? ((int)(Time.unscaledDeltaTime * 1E7 + 0.1) * 1E-7) / ((int)(Time.captureDeltaTime * 1E7 + 0.1) * 1E-7)
            : ((int)(Time.timeScale * 1E6 + 0.1) * 1E-6));
            Volatile.Write(ref ut_time, ModsTagCLib.PreciseFileTime());
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
        public static unsafe double InterpolationDSPTime
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
                long time = ModsTagCLib.PreciseFileTime();
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
        public static unsafe long InterpolationDSPTimeAsFileTime
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
                long time = ModsTagCLib.PreciseFileTime();
                if (ut_time > at_time)
                {
                    return (long)(dsp * 10_000_000.0 + (ut_time - at_time) * lastmultiply + (time - ut_time) * multiply + offset);
                }
                return (long)(dsp * 10_000_000.0 + (time - at_time) * multiply + offset);
            }
        }
    }
}
