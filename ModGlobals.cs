using HarmonyLib;

namespace REPOShock;

internal static class ModGlobals
{
	public static bool IsAlive { get; set; } = true;
	public static string CurrentLevel { get; set; } = "";
	public static bool IsSafeLevel { get; set; } = true;

	public static void EvaluateIsSafeLevel()
	{
		bool arena = false;
		if (!SemiFunc.IsMultiplayer())
		{
			arena = CurrentLevel == "Arena";
		}
		IsSafeLevel = CurrentLevel == "Shop" || CurrentLevel == "Lobby" || CurrentLevel == "Lobby Menu" || CurrentLevel == "" || arena;
	}
}








