/*
 * Copy in Cover Mod
 * Assembly: Cover.dll
 * NameSpace: Cover.Tweaks.Patches
 * Categoty: StaticClass
 * Name: __scrConductor
 * Flag: public auto ansi abstract sealed beforefieldinit flag(200000)
 * Extends: [mscorlib]System.Object
 */
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AsyncInputOptimize.Patch
{
    [HarmonyPatch]
    public static class UnityEngine__SceneManagement__SceneManager
    {
        [HarmonyPatch(typeof(UnityEngine.SceneManagement.SceneManager), "LoadSceneAsyncNameIndexInternal")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler_LoadSceneAsyncNameIndexInternal(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction ci in instructions)
            {
                yield return ci;
            }
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AsyncInputHook), nameof(AsyncInputHook.ResetTime)));
            yield break;
        }
    }
}
