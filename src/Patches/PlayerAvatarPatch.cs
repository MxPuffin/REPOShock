using HarmonyLib;
using PiShockLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace REPOShock.src.Patches;

[HarmonyPatch(typeof(PlayerAvatar))]
static class PlayerAvatarPatch
{
    [HarmonyPostfix, HarmonyPatch(nameof(PlayerAvatar.PlayerDeathRPC))]
    private static void PlayerDeathRPC(PlayerAvatar __instance)
    {

        if (SemiFunc.PlayerGetSteamID(__instance) != ModGlobals.steamID)
            return;
        PlayerDeath();
    }

    private static void PlayerDeath()
    {
        if (ModGlobals.IsSafeLevel)
            return;

        ModGlobals.IsAlive = false;
        int intensity = ConfigHandler.ConfigDeathIntensity.Value;
        int duration = ConfigHandler.ConfigDeathDuration.Value;
        REPOShock.PiShockController?.OperatePiShock(intensity, duration, PiShockOperations.Shock);
        REPOShock.Logger.LogInfo($"Player died, shocking with {intensity}% for {duration}s");
    }

    [HarmonyPostfix, HarmonyPatch(nameof(PlayerAvatar.Revive))]
    private static void PlayerRevive()
    {
        ModGlobals.Revive();
    }
    [HarmonyPostfix, HarmonyPatch(nameof(PlayerAvatar.ReviveRPC))]
    private static void ReviveRPC()
    {
        ModGlobals.Revive();
    }

}
