using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AsyncInputOptimize.Patch
{
#if ALPHA || BETA || RELEASE
    [HarmonyPatch]
    public static class __scnCalibration
    {
        [HarmonyPatch(typeof(scnCalibration), "Start")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler_Start(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ldc_I4_1);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PatchMidLayer), nameof(PatchMidLayer.Calibration)));
            foreach (CodeInstruction ci in instructions)
            {
                yield return ci;
            }
            yield break;
        }
        [HarmonyPatch(typeof(scnCalibration), "Quit")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler_Quit(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ldc_I4_0);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PatchMidLayer), nameof(PatchMidLayer.Calibration)));
            foreach (CodeInstruction ci in instructions)
            {
                yield return ci;
            }
            yield break;
        }
    }
#elif ALPHA_2_9_8_R136
    [HarmonyPatch]
    public static class __scrCalibrationPlanet
    {
        [HarmonyPatch(typeof(scrCalibrationPlanet), "Start")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler_Start(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ldc_I4_1);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PatchMidLayer), nameof(PatchMidLayer.Calibration)));
            foreach (CodeInstruction ci in instructions)
            {
                yield return ci;
            }
            yield break;
        }
        [HarmonyPatch(typeof(scrCalibrationPlanet), "Quit")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler_Quit(IEnumerable<CodeInstruction> instructions)
        {
            yield return new CodeInstruction(OpCodes.Ldc_I4_0);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PatchMidLayer), nameof(PatchMidLayer.Calibration)));
            foreach (CodeInstruction ci in instructions)
            {
                yield return ci;
            }
            yield break;
        }
    }
#endif
}
