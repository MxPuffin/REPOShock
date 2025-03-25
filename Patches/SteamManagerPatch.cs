using HarmonyLib;
using Photon.Voice;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;

namespace REPOShock.Patches
{

	[HarmonyPatch(typeof(SteamManager))]
	static class SteamManagerPatch
	{

		[HarmonyPostfix, HarmonyPatch(nameof(SteamManager.Start))]
		private static void SteamManagerStart()
		{
			ModGlobals.steamID = SteamClient.SteamId.ToString();
			REPOShock.Logger.LogInfo($"Setting steam ID for the session: {ModGlobals.steamID}");
		}
	}
}
