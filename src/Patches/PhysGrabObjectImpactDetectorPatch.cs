using HarmonyLib;
using PiShockLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace REPOShock.src.Patches;

[HarmonyPatch(typeof(PhysGrabObjectImpactDetector))]
static class PhysGrabObjectImpactDetectorPatch
{
    private static bool _lastHitEnemy = false;

    [HarmonyPrefix, HarmonyPatch(nameof(PhysGrabObjectImpactDetector.OnCollisionStay))]
    private static void OnCollisionStay(ref Collision collision)
    {
        // really dodgy and maybe inefficient way of preventing
        // 'intentional' enemy hits from shocking

        // TODO
        // Create a custom class to store in recently held object
        // To keep track of 
        if (!ConfigHandler.ConfigBreakIgnoreEnemy.Value)
            return;

        if (collision.transform.CompareTag("Enemy"))
        {
            _lastHitEnemy = true;
        }
        else
        {
            _lastHitEnemy = false;
        }
    }

    [HarmonyPostfix, HarmonyPatch(nameof(PhysGrabObjectImpactDetector.BreakRPC))]
    private static void ItemImpactBreakRPC(float valueLost, ref bool _loseValue, int breakLevel, PhysGrabObjectImpactDetector __instance)
    {
        if (!ConfigHandler.ConfigEnableBreakShock.Value)
            return;
        if (!_loseValue)
            return;

        ItemBreakPostfix(valueLost, breakLevel, __instance);
    }

    private static void ItemBreakPostfix(float valueLost, int breakLevel, PhysGrabObjectImpactDetector __instance)
    {
        if (!ModGlobals.IsAlive)
            return;
        if (ModGlobals.CurrentLevel == "Arena")
            return;
        if (!__instance.isValuable)
            return;


        bool isHeldByLocalPlayer = __instance.physGrabObject.heldByLocalPlayer;

        if (isHeldByLocalPlayer)
        {
            if (!ModGlobals.RecentlyHeldObjects.ContainsKey(__instance.gameObject))
            {
                ModGlobals.RecentlyHeldObjects.Add(__instance.gameObject, Time.time);
                REPOShock.Logger.LogInfo($"Object {__instance.gameObject.name} added to held objects.");
            }
            else
            {
                ModGlobals.RecentlyHeldObjects[__instance.gameObject] = Time.time;
            }
        }

        float originalValue = __instance.valuableObject.dollarValueOriginal;

        ShockPlayerIfNecessary(__instance.gameObject, valueLost, originalValue, isHeldByLocalPlayer);

    }

    private static void ShockPlayerIfNecessary(GameObject damagedObject, float valueLost, float originalValue, bool isHeldByLocalPlayer)
    {
        if (!ModGlobals.RecentlyHeldObjects.ContainsKey(damagedObject))
            return;

        var lastHeldTime = ModGlobals.RecentlyHeldObjects[damagedObject];

        if (isHeldByLocalPlayer && _lastHitEnemy)
        {
            ModGlobals.LastOffensiveGracePeriodTime = Time.time;
            ModGlobals.LastOffensiveObject = damagedObject;
            REPOShock.Logger.LogInfo("[Damage Item Event] Enemy hit, aborting shock");
            return;
        }

        if (_lastHitEnemy && Time.time - lastHeldTime <= ConfigHandler.ThrownOffensiveGracePeriod.Value)
        {
            ModGlobals.LastOffensiveGracePeriodTime = Time.time;
            ModGlobals.LastOffensiveObject = damagedObject;
            REPOShock.Logger.LogInfo("[Damage Item Event] Enemy hit in thrown grace period, aborting shock");
            return;
        }

        if (ModGlobals.LastOffensiveObject == damagedObject
            && Time.time - ModGlobals.LastOffensiveGracePeriodTime <= ConfigHandler.HitOffensiveGracePeriod.Value)
        {
            REPOShock.Logger.LogInfo("[Damage Item Event] Recently used 'weapon' took damage, aborting shock");
            return;
        }

        if (Time.time - lastHeldTime <= ConfigHandler.ConfigBreakShockWindow.Value)
        {
            ShockPlayer(valueLost, originalValue, damagedObject.name);
        }

    }

    private static void ShockPlayer(float valueLost, float originalValue, string objName)
    {
        int intensity;

        if (!ConfigHandler.ConfigMapValueToShock.Value)
        {
            intensity = (int)Math.Ceiling(valueLost * ConfigHandler.ConfigBreakValueDmgMult.Value);
        }
        else
        {
            intensity = MapValue(valueLost, 0, originalValue, 0, ConfigHandler.ConfigMaxIntensity.Value);
        }

        intensity = PiShockController.ClampShock(intensity);

        REPOShock.PiShockController?.OperatePiShock(intensity, 1, PiShockOperations.Shock);
        REPOShock.Logger.LogInfo($"[Damage Item Event] Played damaged {objName} for {valueLost} - Shocking for {intensity}%");
    }

    private static int MapValue(float value, float start1, float stop1, float start2, float stop2)
    {
        return (int)(start1 + (value - start1) * (stop2 - start2) / (stop1 - start1));
    }
}
