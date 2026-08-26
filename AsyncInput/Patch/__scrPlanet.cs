using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AsyncInput.Patch
{
    public static class __scrPlanet
    {
        public static IEnumerable<CodeInstruction> Transpiler_AsyncRefreshAngles(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PatchMidLayer), nameof(PatchMidLayer.AsyncRefreshAngles)));
            yield break;
        }
    }
}
