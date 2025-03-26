using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace REPOShock.src;

internal static class ModGlobals
{
    internal static string steamID { get; set; } = string.Empty;
    internal static bool IsAlive { get; set; } = true;
    internal static string CurrentLevel { get; set; } = "";
    internal static bool IsSafeLevel { get; set; } = true;
    internal static float LastHeldItemPickupTime { get; set; } = Time.time;
    private static float _lastLevelMessage = Time.time;


    internal static GameObject? LastOffensiveObject = null;
    internal static float LastOffensiveGracePeriodTime = 0;

    internal static Dictionary<GameObject, float> RecentlyHeldObjects = new Dictionary<GameObject, float>();


    internal static void UpdateLevel(string level)
    {
        CurrentLevel = level;
        EvaluateIsSafeLevel();

        if (_lastLevelMessage < 3)
        {
            return;
        }

        REPOShock.Logger.LogInfo($"Updating Level to {level} - Resetting Mod Global Variables");
        _lastLevelMessage = Time.time;
    }
    private static void EvaluateIsSafeLevel()
    {
        bool arena = false;
        // Prevent the forced arena death from injuring player in single player
        if (!SemiFunc.IsMultiplayer())
        {
            arena = CurrentLevel == "Arena";
        }
        IsSafeLevel = CurrentLevel == "Shop" || CurrentLevel == "Lobby" || CurrentLevel == "Lobby Menu" || CurrentLevel == "" || arena;
    }
    internal static void Revive()
    {
        IsAlive = true;
        REPOShock.Logger.LogInfo("Player has been revived.");
    }
}








