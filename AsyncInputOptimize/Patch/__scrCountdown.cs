using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AsyncInputOptimize.Patch
{
    [HarmonyPatch]
    public static class __scrCountdown
    {
        [HarmonyPatch(typeof(scrCountdown), "Update")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler_Update(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PatchMidLayer), nameof(PatchMidLayer.CountdownUpdate)));
            foreach (CodeInstruction ci in instructions)
            {
                yield return SafeDSPTime.ReplaceDSPTime(ci);
            }
            yield break;
        }
    }
}
