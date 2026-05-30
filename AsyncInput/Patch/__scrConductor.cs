using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace AsyncInput.Patch
{
    public static class __scrConductor
    {
        public static IEnumerable<CodeInstruction> Transpiler_Start(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AsyncInputHook), nameof(AsyncInputHook.ResetTime)));
            foreach (CodeInstruction ci in instructions)
            {
                yield return ci;
            }
            yield break;
        }
        public static IEnumerable<CodeInstruction> Transpiler_Rewind(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AsyncInputHook), nameof(AsyncInputHook.ResetTime)));
            foreach (CodeInstruction ci in instructions)
            {
                yield return ci;
            }
            yield break;
        }
        public static IEnumerable<CodeInstruction> Transpiler_Update(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AsyncInputHook), nameof(AsyncInputHook.ConductorUpdate)));

            bool skip = true;
            foreach (CodeInstruction ci in instructions)
            {
                if (ci.opcode == OpCodes.Stfld && (ci.operand as FieldInfo).Name == "prev_unityDspTime")
                {
                    skip = false;
                    continue;
                }
                if (skip)
                {
                    continue;
                }
                yield return ci;
            }
            yield break;
        }
    }
}
