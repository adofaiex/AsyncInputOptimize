#pragma once

#include "_PCH.h"

extern "C"
{
    namespace AsyncInputOptimize_CppCore_Var
    {
        extern System::UInt64 QueryPerfMul;
        extern System::UInt64 QueryPerfDiv;
        extern System::UInt64 QueryPerfFreq;
    }
    sio System::UInt64 GetPreciseTimeAsFileTime()
    {
        System::UInt64 i64;
#if MT_QTIME == 1
        QueryPerformanceCounter(reinterpret_cast<LARGE_INTEGER*>(&i64));
        if (AsyncInputOptimize_CppCore_Var::QueryPerfMul)
            return i64 * AsyncInputOptimize_CppCore_Var::QueryPerfMul;
        if (AsyncInputOptimize_CppCore_Var::QueryPerfDiv)
            return i64 / AsyncInputOptimize_CppCore_Var::QueryPerfDiv;

        System::UInt64 hi;
        System::UInt64 lo = _umul128(i64, 10000000ULL, &hi);
        System::UInt64 rem;
        return _udiv128(hi, lo, AsyncInputOptimize_CppCore_Var::QueryPerfFreq, &rem);
#else 
        GetSystemTimePreciseAsFileTime(reinterpret_cast<FILETIME*>(&i64));
        return (i64 - 126227808000000000ULL);
#endif
    }
}