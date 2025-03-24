using HarmonyLib;
using PiShockLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace REPOShock.Patches;

[HarmonyPatch(typeof(PlayerHealth))]
static class PlayerHealthPatch
{
	[HarmonyPostfix, HarmonyPatch(nameof(PlayerHealth.Hurt))]
	private static void TakeDamage(ref int damage)
	{
		if (ModGlobals.IsSafeLevel)
			return;
		if (!ModGlobals.IsAlive)
			return;
		if (damage == 0)
			return;
		int shockIntensity = (int)Math.Ceiling(damage / REPOShock.ConfigDamageInteravl.Value);
		if (shockIntensity > REPOShock.ConfigMaxIntensity.Value)
			shockIntensity = REPOShock.ConfigMaxIntensity.Value;
		else if (shockIntensity < 1)
			shockIntensity = 1;
		REPOShock.PiShockController.OperatePiShock(shockIntensity, 1, PiShockOperations.Shock);
		REPOShock.Logger.LogInfo($"Took {damage} damage, shocking for {shockIntensity}%");
	}
}
