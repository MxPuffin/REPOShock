using HarmonyLib;
using PiShockLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace REPOShock.src.Patches;

[HarmonyPatch(typeof(PlayerHealth))]
static class PlayerHealthPatch
{

    [HarmonyPrefix, HarmonyPatch(nameof(PlayerHealth.Hurt))]
    private static void TakeDamage(ref int damage, PlayerHealth __instance)
    {
        if (SemiFunc.PlayerGetSteamID(__instance.playerAvatar) != ModGlobals.steamID)
            return;

        if (ModGlobals.IsSafeLevel)
            return;
        if (!ModGlobals.IsAlive)
            return;
        if (damage == 0)
            return;

        int shockIntensity = (int)Math.Ceiling(damage / ConfigHandler.ConfigDamageInteravl.Value);
        shockIntensity = PiShockController.ClampShock(shockIntensity);

        REPOShock.PiShockController?.OperatePiShock(shockIntensity, 1, PiShockOperations.Shock);
        REPOShock.Logger.LogInfo($"[Health Event] Took {damage} damage, shocking for {shockIntensity}%");
    }
}
