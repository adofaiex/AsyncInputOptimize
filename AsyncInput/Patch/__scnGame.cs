using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AsyncInput.Patch
{
    public static class __scnGame
    {
        public static IEnumerable<CodeInstruction> Transpiler_Play(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PatchMidLayer), nameof(PatchMidLayer.StartOrPlay)));
            foreach (CodeInstruction ci in instructions)
            {
                yield return SafeDSPTime.ReplaceDSPTime(ci);
            }
            yield break;
        }
    }
}
