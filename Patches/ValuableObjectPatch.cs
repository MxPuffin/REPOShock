using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace REPOShock.Patches;

[HarmonyPatch(typeof(ValuableObject))]
static class ValuableObjectPatch
{

	[HarmonyPostfix, HarmonyPatch(nameof(ValuableObject.DollarValueSetLogic))]
	private static void ItemTakeDamagePostfix(ValuableObject __instance)
	{
		//REPOShock.Logger.LogInfo($"{__instance.name} Item spawned worth ${__instance.dollarValueCurrent}.");
	}
}
