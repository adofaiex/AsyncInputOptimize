/*
 * Copy in Cover Mod
 * Assembly: Cover.dll
 * NameSpace: Cover.Tweaks.Patches
 * Categoty: StaticClass
 * Name: __scrConductor
 * Flag: public auto ansi abstract sealed beforefieldinit flag(200000)
 * Extends: [mscorlib]System.Object
 */
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace AsyncInputOptimize.Patch
{
    [HarmonyPatch(typeof(scrConductor), "Update")]
    public static class __scrConductor
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long Update_1() => DateTime.Now.Ticks - DateTime.UtcNow.Ticks + CppBrige.GetSystemTick() + 504911232000000000L;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Update_2() => InterpolationTime.dspTime;
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler_Update(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            /*移除IL代码
             *  (ulong)DateTime.Now.Ticks;
             */
            /*添加IL代码
             *  Update_1();
             */
            /*修改IL代码
             *  AudioSetting.dspTime;
             *  
             *  Update_2();
             */
            bool patch = false;
            int skip = 0;
            foreach (CodeInstruction ci in instructions)
            {
                if (patch)
                {
                    patch = false;
                    yield return new CodeInstruction(OpCodes.Call, typeof(__scrConductor).GetMethod(nameof(Update_1), AccessTools.all));
                }
                if (skip > 0)
                {
                    skip--;
                    continue;
                }
                if (ci.opcode == OpCodes.Call && ((MethodInfo)ci.operand).Name == "get_Now")
                {
                    skip = 3;
                    patch = true;
                    continue;
                }
                if (ci.opcode == OpCodes.Call && ((MethodInfo)ci.operand).Name == "get_dspTime")
                {
                    ci.operand = typeof(__scrConductor).GetMethod(nameof(Update_2), AccessTools.all);
                }

                // 其余返回
                yield return ci;
            }
            yield break;
        }
    }
}
