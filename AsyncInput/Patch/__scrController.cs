using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AsyncInput.Patch
{
    public static class __scrController
    {
        public static IEnumerable<CodeInstruction> Transpiler_UpdateInput(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PatchMidLayer), nameof(PatchMidLayer.UpdateInput)));
            yield return new CodeInstruction(OpCodes.Ret);
            yield break;
        }
    }
}
