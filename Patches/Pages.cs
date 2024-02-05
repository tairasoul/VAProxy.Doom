using Devdog.General.UI;
using HarmonyLib;
using System;

namespace DoomInProxy.Patches
{
    [HarmonyPatch(typeof(UIWindow))]
    internal class PagesPatches
    {
        [HarmonyPatch("Show", new Type[] {})]
        [HarmonyPrefix]
        static bool show_prefix(UIWindow __instance)
        {
            if (__instance.name == "Pages")
            {
                return !Plugin.isRunning;
            }
            return true;
        }
        [HarmonyPatch("Hide", new Type[] { })]
        [HarmonyPrefix]
        static bool hide_prefix(UIWindow __instance)
        {
            if (__instance.name == "Pages")
            {
                return !Plugin.isRunning;
            }
            return true;
        }
    }
}
