using HarmonyLib;
using PiShockLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace REPOShock.Patches;

[HarmonyPatch(typeof(PlayerAvatar))]
static class PlayerAvatarPatch
{
	[HarmonyPostfix, HarmonyPatch(nameof(PlayerAvatar.PlayerDeath))]
	private static void PlayerDeath()
	{
		if (ModGlobals.IsSafeLevel)
			return;

		ModGlobals.IsAlive = false;
		int intensity = REPOShock.ConfigDeathIntensity.Value;
		int duration = REPOShock.ConfigDeathDuration.Value;
		REPOShock.PiShockController.OperatePiShock(intensity, duration, PiShockOperations.Shock);
		REPOShock.Logger.LogInfo("Played died");
	}

	[HarmonyPostfix, HarmonyPatch(nameof(PlayerAvatar.PlayerDeathDone))]
	private static void PlayerRevive()
	{
		ModGlobals.IsAlive = true;
	}
}
