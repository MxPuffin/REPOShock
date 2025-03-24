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
		REPOShock.PiShockController.OperatePiShock(25, 3, PiShockOperations.Shock);
		REPOShock.Logger.LogInfo("Played died");
	}

	[HarmonyPostfix, HarmonyPatch(nameof(PlayerAvatar.PlayerDeathDone))]
	private static void PlayerRevive()
	{
		ModGlobals.IsAlive = true;
	}
}
