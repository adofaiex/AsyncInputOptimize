#pragma once
#ifndef PCH_ONCE
# define PCH_ONCE

# ifndef OUT_WINDOWS
#  include <windows.h>
#  include <winternl.h>
#  include <timeapi.h>
# endif
# include <immintrin.h>
# include <intrin.h>

# if defined(__GNUC__) && !defined(__clang__)
#  pragma GCC target("avx2")
#  pragma GCC optimize("O3")
#  pragma GCC optimize("unroll-loops")
#  pragma GCC optimize("inline")
#  pragma GCC optimize("tree-vectorize")
# elif defined(_MSC_VER)
#  pragma optimize("", on)
#  pragma inline_depth(255)
#  pragma inline_recursion(on)
#  pragma auto_inline(on)
# endif

# if defined(__x86_64__) || defined(_M_X64) || defined(__i386__) || defined(_M_IX86)


# define abstract extern __declspec(nothrow)
# define sio inline

# if defined(__AVX512__) || defined(__AVX512F__) || defined(__AVX2__) || defined(__AVX__)
#  define MT_QTIME 1
# else
#  define MT_QTIME 0
# endif

#endif

namespace System
{
    typedef unsigned char Byte;
    typedef signed char SByte;
    typedef signed short Int16;
    typedef unsigned short UInt16;
    typedef signed int Int32;
    typedef unsigned int UInt32;
    typedef signed long Int32l;
    typedef unsigned long UInt32l;
    typedef signed long long Int64;
    typedef unsigned long long UInt64;
    __declspec(align(16)) typedef struct Int128 {
        UInt64 low;
        UInt64 high;
    };
    __declspec(align(16)) typedef struct UInt128 {
        UInt64 low;
        UInt64 high;
    };
    typedef void* IntPtr;
    typedef void* UIntPtr;
    typedef float Single;
    typedef double Double;
    typedef double long Doublel;
    typedef unsigned char Boolean;
    typedef void Void;
}
#endif //PCH_ONCE