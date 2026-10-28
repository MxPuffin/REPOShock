using HarmonyLib;
using REPOShock.src.PiShockLibrary.Enums;
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
        if (SemiFunc.PlayerGetSteamID(__instance) != ModGlobals.SteamID) { return; }
        PlayerDeath();
    }

    private static void PlayerDeath()
    {
        if (ModGlobals.IsSafeLevel) { return; }

        ModGlobals.IsAlive = false;
        int intensity = ConfigHandler.DeathIntensity.Value;
        int duration = ConfigHandler.DeathDuration.Value;
        REPOShock.PiShockController?.OperatePiShock(intensity, duration, PiShockOperation.Shock);
        REPOShock.Logger.LogInfo($"Player died, shocking with {intensity}% for {(duration > 15 ? duration / 15 : duration)}s");
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
