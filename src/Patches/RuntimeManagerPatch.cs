using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

namespace REPOShock.src.Patches;


[HarmonyPatch(typeof(RunManager))]
static class RunManagerPatch
{

    [HarmonyPostfix, HarmonyPatch(nameof(RunManager.ChangeLevel))]
    public static void ChangeLevel(RunManager __instance)
    {
        UpdateLevelInfoAndResetState(__instance.levelCurrent.name);
    }

    [HarmonyPostfix, HarmonyPatch(nameof(RunManager.UpdateLevel))]
    public static void UpdateLevel(RunManager __instance, ref string _levelName, ref bool _gameOver)
    {
        UpdateLevelInfoAndResetState(_levelName);
    }

    private static void UpdateLevelInfoAndResetState(string level)
    {
        var levelName = level.Replace("Level - ", "");
        ModGlobals.UpdateLevel(levelName.Trim());

        ModGlobals.RecentlyHeldObjects.Clear();
        if (!ModGlobals.IsAlive)
        {
            ModGlobals.Revive();
        }
    }
}
