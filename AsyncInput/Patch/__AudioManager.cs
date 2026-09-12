using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AsyncInput.Patch
{
    public static class __AudioManager
    {
        public static IEnumerable<CodeInstruction> Transpiler_Play(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SafeDSPTime), nameof(SafeDSPTime.GetOffset)));
            yield return new CodeInstruction(OpCodes.Conv_R8);
            yield return new CodeInstruction(OpCodes.Ldc_I4, 1000000000);
            yield return new CodeInstruction(OpCodes.Conv_R8);
            yield return new CodeInstruction(OpCodes.Div);
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Add);
            yield return new CodeInstruction(OpCodes.Starg, 1);
            foreach (CodeInstruction ci in instructions)
            {
                yield return ci;
            }
            yield break;
        }
    }
}
