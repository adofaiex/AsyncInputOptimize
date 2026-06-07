using AsyncInput.Logic;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AsyncInput.Patch
{
    public static class SkyHook__SkyHookManager
    {
        public static IEnumerable<CodeInstruction> Transpiler__StartHook(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ldc_I4_1);
            yield return new CodeInstruction(OpCodes.Stsfld, AccessTools.Field(typeof(AsyncInputData), nameof(AsyncInputData.enabled)));
            yield return new CodeInstruction(OpCodes.Ret);
            yield break;
        }
        public static IEnumerable<CodeInstruction> Transpiler__StopHook(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ldc_I4_0);
            yield return new CodeInstruction(OpCodes.Stsfld, AccessTools.Field(typeof(AsyncInputData), nameof(AsyncInputData.enabled)));
            yield return new CodeInstruction(OpCodes.Ret);
            yield break;
        }
        public static IEnumerable<CodeInstruction> Transpiler_get_isHookActive(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(AsyncInputData), nameof(AsyncInputData.enabled)));
            yield return new CodeInstruction(OpCodes.Ret);
            yield break;
        }
    }
}
