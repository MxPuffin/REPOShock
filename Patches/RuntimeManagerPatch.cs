using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace REPOShock.Patches;


[HarmonyPatch(typeof(RunManager))]
static class RunManagerPatch
{
	[HarmonyPostfix, HarmonyPatch(nameof(RunManager.ChangeLevel))]
	public static void ChangeLevel(RunManager __instance)
	{
		if (__instance == null) return;

		var levelField = typeof(RunManager).GetField("levelCurrent");

		if (levelField != null)
		{
			var levelCurrent = levelField.GetValue(__instance);

			if (levelCurrent != null)
			{
				var nameField = levelCurrent.GetType().GetProperty("name");

				if (nameField != null)
				{
					string levelName = ((ScriptableObject)levelCurrent).name;
					levelName = levelName.Replace("Level - ", "");
					ModGlobals.CurrentLevel = levelName.Trim();
					ModGlobals.EvaluateIsSafeLevel();
				}
			}
		}
	}
}
