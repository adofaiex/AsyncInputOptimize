/*
 * Copy in ModsTagLib.Unity Mod
 * Assembly: ModsTagLib.dll
 * NameSpace: ModsTagLib.Time
 * Categoty: InstanceClass
 * Name: CalculateThread
 * Flag: public auto ansi abstract sealed beforefieldinit flag(200000)
 * Extends: [mscorlib]System.Object
 */

using System;
using System.Runtime.InteropServices;

namespace AsyncInputOptimize
{
    [StructLayout(LayoutKind.Sequential, Size = sizeof(long) * 8, Pack = 8)]
    public struct TimeCounter_Data
    {
        /*
        /// <summary>
        /// 函数运行消耗时间
        /// </summary>
        public long runTime;
        /// <summary>
        /// 等待函数运行时间
        /// </summary>
        public long waitTime;
        */
        /// <summary>
        /// 总等待时间
        /// </summary>
        public long deltaTime;
        /// <summary>
        /// 每秒更新次数
        /// </summary>
        public long updatePerSecond;
        /*
        /// <summary>
        /// 平均函数运行消耗时间
        /// </summary>
        public double avg_runTime;
        /// <summary>
        /// 平均等待函数运行时间
        /// </summary>
        public double avg_waitTime;
        /// <summary>
        /// 平均总等待时间
        /// </summary>
        public double avg_deltaTime;
        /// <summary>
        /// 平均每秒更新次数
        /// </summary>
        public double avg_updatePerSecond;
        /// <summary>
        /// 函数运行消耗时间 (ms单位)
        /// </summary>
        public readonly double runTime_ms => runTime / 10_000_000.0;
        /// <summary>
        /// 等待函数运行时间 (ms单位)
        /// </summary>
        public readonly double waitTime_ms => waitTime / 10_000_000.0;
        /// <summary>
        /// 总等待时间 (ms单位)
        /// </summary>
        public readonly double deltaTime_ms => deltaTime / 10_000_000.0;
        /// <summary>
        /// 平均函数运行消耗时间 (ms单位)
        /// </summary>
        public readonly double avg_runTime_ms => avg_runTime / 10_000_000.0;
        /// <summary>
        /// 平均等待函数运行时间 (ms单位)
        /// </summary>
        public readonly double avg_waitTime_ms => avg_waitTime / 10_000_000.0;
        /// <summary>
        /// 平均总等待时间 (ms单位)
        /// </summary>
        public readonly double avg_deltaTime_ms => avg_deltaTime / 10_000_000.0;
        */
    }
    public unsafe sealed class TimeMethodCounter
    {
        [StructLayout(LayoutKind.Sequential, Size = 16 + 8, Pack = 4)]
        private struct RepeatQueue
        {
            internal readonly int __capacity;
            internal long[] _array;
            internal int _head;
            internal int _last;
            internal int _size;
            internal RepeatQueue(int capacity)
            {
                __capacity = capacity;
                _array = new long[__capacity];
                _head = 0;
                _last = 0;
                _size = 0;
            }
            internal void Init()
            {
                _array = new long[__capacity];
                _head = 0;
                _last = 0;
                _size = 0;
            }
            internal void ExtendSize()
            {
                long[] array = new long[_array.Length + __capacity];
                if (_last > 0)
                    Array.Copy(_array, 0, array, 0, _last);
                Array.Copy(_array, _head, array, __capacity + _head, _array.Length - _head);

                _head += __capacity;
                _array = array;
            }
            internal void Enqueue(long value)
            {
                _array[_last] = value;
                _last = (_last + 1) % _array.Length;
                _size++;
                if (_last == _head)
                    ExtendSize();
            }
            internal void Remove()
            {
                _head = (_head + 1) % _array.Length;
                _size--;
            }
            internal long Peek()
            {
                return _array[_head];
            }
        }
        public TimeMethodCounter(delegate* managed<void> param)
        {
            m_update = param;
            m_timeArray = new(32);
            // avg_update = 2500000;
            Reload();
        }
        public TimeMethodCounter(delegate* managed<void> param, int len)
        {
            m_update = param;
            m_timeArray = new(len);
            // avg_update = 2500000;
            Reload();
        }
        private long m_lastTimeS;
        // private long m_lastTimeE;
        private RepeatQueue m_timeArray;
        private TimeCounter_Data m_result;
        private readonly delegate* managed<void> m_update;

        // private long avg_updateCounter;
        // private long avg_update;
        // private long avg_runTimes;
        // private long avg_waitTimes;
        // private long avg_deltaTimes;
        // private long avg_updatePerSeconds;
        // private long avg_counter;

        /// <summary>
        /// 返回的结果, 以file time为单位
        /// </summary>
        public TimeCounter_Data Result => m_result;
        public int ArrayLength => m_timeArray._array.Length;
        // public int AVGUpdate { get => (int)avg_update; set => avg_update = value > 100000000 ? 10000000 : value < 1000000 ? 1000000 : value; }

        public void Reload()
        {
            m_result = new();
            m_timeArray.Init();
            // avg_updateCounter = 0;
            // avg_runTimes = 0;
            // avg_waitTimes = 0;
            // avg_deltaTimes = 0;
            // avg_updatePerSeconds = 0;
            // avg_counter = 0;
            m_lastTimeS = CppBrige.GetSystemTick();
            // m_lastTimeE = CppBrige.GetSystemTick();
        }
        public void Update()
        {
            // run
            long start = CppBrige.GetSystemTick();
            m_update();
            // long end = CppBrige.GetSystemTick();

            // calc
            m_timeArray.Enqueue(start + 10_000_000);

            while (start > m_timeArray.Peek()) m_timeArray.Remove();

            m_result.deltaTime = start - m_lastTimeS;
            m_result.updatePerSecond = m_timeArray._size;
            // avg_runTimes += (m_result.runTime = end - start);
            // avg_waitTimes += (m_result.waitTime = start - m_lastTimeE);
            // avg_deltaTimes += (m_result.deltaTime = start - m_lastTimeS);
            // avg_updatePerSeconds += (m_result.updatePerSecond = m_timeArray._size);
            // avg_counter++;
            // // avg update
            // avg_updateCounter += m_result.deltaTime;
            // if (avg_updateCounter >= avg_update)
            // {
            //     avg_updateCounter = 0;
            //     m_result.avg_runTime = avg_runTimes / (double)avg_counter;
            //     m_result.avg_waitTime = avg_waitTimes / (double)avg_counter;
            //     m_result.avg_deltaTime = avg_deltaTimes / (double)avg_counter;
            //     m_result.avg_updatePerSecond = avg_updatePerSeconds / (double)avg_counter;
            //     avg_runTimes = 0;
            //     avg_waitTimes = 0;
            //     avg_deltaTimes = 0;
            //     avg_updatePerSeconds = 0;
            //     avg_counter = 0;
            // }
            // update times
            m_lastTimeS = start;
            // m_lastTimeE = end;
        }
    }
}
