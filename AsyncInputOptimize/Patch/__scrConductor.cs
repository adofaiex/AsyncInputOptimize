using AsyncInputOptimize.Platform;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace AsyncInputOptimize.Patch
{
    [HarmonyPatch(typeof(scrConductor), "Update")]
    public static class __scrConductor
    {
        private static unsafe long Update_1()
        {
            long l = BaseSelect.GetFileTime();
            return DateTime.Now.Ticks - DateTime.UtcNow.Ticks + l;
        }
        private static double Update_2() => DSPTimeSimulater.GetDSPTime();
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
