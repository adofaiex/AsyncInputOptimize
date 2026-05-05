// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "_PCH.h";
#include "core_function.h"

extern "C"
{
    namespace AsyncInputOptimize_CppCore_Var
    {
        System::UInt64 QueryPerfMul;
        System::UInt64 QueryPerfDiv;
        System::UInt64 QueryPerfFreq;
    }
    namespace AsyncInputOptimize_CppCore
    {
        __declspec(dllexport) void GOGOGO_GO_TO_CRASH______________()
        {
            [&]() { return *((volatile int*)nullptr); }();
        }
        __declspec(dllexport) System::Int64 GetSystemTick()
        {
            System::Int64 t;
            GetSystemTimePreciseAsFileTime(reinterpret_cast<FILETIME*>(&t));
            return t;
        }
        __declspec(dllexport) System::Int64 LowSleep(System::Int64 tick)
        {
            System::Int64 i64a = GetPreciseTimeAsFileTime();
            WaitSleep(tick, TRUE);
            System::Int64 i64b = GetPreciseTimeAsFileTime();
            return tick - (i64b - i64a);
        }
        __declspec(dllexport) System::Int64 HighSleep(System::Int64 tick)
        {
            System::Int64 i64a = GetPreciseTimeAsFileTime();
            if (tick > 0)
            {
                if (tick > SLEEPMIX_OFFSET1)
                    Sleep((tick - SLEEPMIX_OFFSET1) / 10000);
                System::Int64 due = GetPreciseTimeAsFileTime();
                System::Int64 last = (due - i64a);
                if (tick - last > (SLEEPMIX_OFFSET2 << 1))
                    WaitSleep(tick - last - SLEEPMIX_OFFSET2 - SLEEPMIX_OFFSET2, TRUE);
            }
            System::Int64 i64b;
            System::Int64 delta = 0;
            do
            {
                SwitchToThread();
                i64b = GetPreciseTimeAsFileTime();
                delta = (i64b - i64a);
            } while (tick > delta + 2);
            return tick - delta;
        }
    }
}

static BOOL __stdcall DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
    HMODULE hNtdll;
    System::UInt64 freq;
    System::UInt64 m;
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        QueryPerformanceFrequency(reinterpret_cast<LARGE_INTEGER*>(&freq));
        AsyncInputOptimize_CppCore_Var::QueryPerfFreq = freq;
        m = 10000000ULL / freq;
        if (m * freq != 10000000ULL) { AsyncInputOptimize_CppCore_Var::QueryPerfMul = 0ULL; }
        else { AsyncInputOptimize_CppCore_Var::QueryPerfMul = m; }
        m = freq / 10000000ULL;
        if (m * 10000000ULL != freq) { AsyncInputOptimize_CppCore_Var::QueryPerfDiv = 0ULL; }
        else { AsyncInputOptimize_CppCore_Var::QueryPerfDiv = m; }
        break;
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    default:
        break;
    }
    return TRUE;
}