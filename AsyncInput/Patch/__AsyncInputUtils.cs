using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AsyncInput.Patch
{
    public static class __AsyncInputUtils
    {
        public static IEnumerable<CodeInstruction> Transpiler_AdjustAngle(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PatchMidLayer), nameof(PatchMidLayer.AdjustAngle)));
            yield return new CodeInstruction(OpCodes.Ret);
            yield break;
        }
        public static IEnumerable<CodeInstruction> Transpiler_GetAngle(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Ldarg_2);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PatchMidLayer), nameof(PatchMidLayer.GetAngle)));
            yield return new CodeInstruction(OpCodes.Ret);
            yield break;
        }
        public static IEnumerable<CodeInstruction> Transpiler_GetSongPosition(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PatchMidLayer), nameof(PatchMidLayer.GetSongPosition)));
            yield return new CodeInstruction(OpCodes.Ret);
            yield break;
        }
    }
}
