using HarmonyLib;
using Photon.Voice;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;

namespace REPOShock.src.Patches
{
	// Something borked here, but idk lol, moved setting steam ID to mod init
//    [HarmonyPatch(typeof(SteamManager))]
//    static class SteamManagerPatch
//    {
//
//        [HarmonyPostfix, HarmonyPatch(nameof(SteamManager.Start))]
//        private static void SteamManagerStart()
//        {
//            ModGlobals.SteamID = SteamClient.SteamId.ToString();
//            REPOShock.Logger.LogInfo($"Setting steam ID for the session: {ModGlobals.SteamID}");
//        }
//    }
}
