using HarmonyLib;

namespace AsyncInputOptimize.Patch
{
    [HarmonyPatch]
    public static class __scnSplash
    {
        [HarmonyPatch(typeof(scnSplash), "GoToMenu")]
        [HarmonyPostfix]
        public static void Postfix_GoToMenu()
        {
            SafeDSPTime.Init();
        }
    }
}
