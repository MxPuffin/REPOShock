using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace REPOShock.src.Patches
{
    [HarmonyPatch(typeof(PhysGrabObject))]
    public static class PhysGrabObjectPatch
    {

        [HarmonyPostfix, HarmonyPatch(nameof(PhysGrabObject.GrabEnded))]
        private static void GrabEndedPostFix(PhysGrabObject __instance)
        {
            if (!__instance.isValuable)
                return;
            if (!__instance.heldByLocalPlayer)
                return;

            ModGlobals.LastHeldItemPickupTime = Time.time;
            ModGlobals.LastOffensiveGracePeriodTime = 0;

            if (!ModGlobals.RecentlyHeldObjects.ContainsKey(__instance.gameObject))
            {
                ModGlobals.RecentlyHeldObjects.Add(__instance.gameObject, Time.time);
                REPOShock.Logger.LogInfo($"Added {__instance.gameObject.name} to recently held");
            }
            else
            {
                ModGlobals.RecentlyHeldObjects[__instance.gameObject] = Time.time;
                REPOShock.Logger.LogInfo($"Updated {__instance.gameObject.name} in recently held");
            }

        }
    }
}
