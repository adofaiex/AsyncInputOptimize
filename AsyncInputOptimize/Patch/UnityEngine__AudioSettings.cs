using HarmonyLib;
using UnityEngine;

namespace AsyncInputOptimize.Patch
{
    [HarmonyPatch]
    public static class UnityEngine__AudioSettings
    {
        [HarmonyPatch(typeof(AudioSettings), "Reset")]
        [HarmonyPostfix]
        public static void Postfix_GoToMenu()
        {
            SafeDSPTime.Init();
        }
    }
}
