using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using PiShockClassLibrary;
using PiShockClassLibrary.Services;
using System.Net.Http;
using PiShockLibrary.Utilities;
using BepInEx.Configuration;
using System.IO;
using Steamworks;
using PiShockClassLibrary.Models;
using ExitGames.Client.Photon.StructWrapping;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace REPOShock;

[BepInPlugin("mxpuffin.REPOShock", "REPOShock", "1.0")]
public class REPOShock : BaseUnityPlugin
{
	internal static REPOShock Instance { get; private set; } = null!;
	internal new static ManualLogSource Logger => Instance._logger;
	private ManualLogSource _logger => base.Logger;
	internal Harmony? Harmony { get; set; }

	private HttpClient? httpClient { get; set; }
	private PiShockAPI? PiShockAPI { get; set; }
	private PiShockUserInfo? PiShockUserInfo { get; set; }
	internal static PiShockController? PiShockController { get; set; }

	private ConfigEntry<string> configUserName { get; set; }
	private ConfigEntry<string> configAPIKey { get; set; }
	private ConfigEntry<string> configShareCodes { get; set; }
	public static ConfigEntry<int> ConfigMaxIntensity { get; private set; }
	public static ConfigEntry<int> ConfigDeathIntensity { get; private set; }
	public static ConfigEntry<int> ConfigDeathDuration { get; private set; }
	public static ConfigEntry<float> ConfigDamageInteravl {  get; private set; }


	private void Awake()
	{
		Instance = this;

		// Prevent the plugin from being deleted
		this.gameObject.transform.parent = null;
		this.gameObject.hideFlags = HideFlags.HideAndDontSave;


		httpClient = new HttpClient();
		PiShockAPI = new PiShockAPI(httpClient);
		PiShockLogger.LogInfoAction = Logger.LogInfo;
		PiShockLogger.LogErrorAction = Logger.LogError;
		InitConfig();

		Patch();

		Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
	}

	internal void Patch()
	{
		Harmony ??= new Harmony(Info.Metadata.GUID);
		Harmony.PatchAll();
	}

	internal void Unpatch()
	{
		Harmony?.UnpatchSelf();
	}

	private void Update()
	{
		// Code that runs every frame goes here
	}

	private async Task InitConfig()
	{
		// API Config
		configUserName = Config.Bind("PiShock Auth",
			"UserName",
			"",
			"Your PiShock username");
		configAPIKey = Config.Bind("PiShock Auth",
			"APIKey",
			"",
			"Your PiShock API Key");
		configShareCodes = Config.Bind("PiShock Auth",
			"ShareCodes",
			"",
			"Comma separated list of share codes");

		// Settings Config
		ConfigMaxIntensity = Config.Bind("Settings",
			"Max Intensity",
			100,
			"The maximum amount of an intensity a shock will ever be. (1-100)");
		if (ConfigMaxIntensity.Value < 1 || ConfigMaxIntensity.Value > 100)
			throw new ArgumentOutOfRangeException("Max intensity cannot be less than 1 or greater than 100");
		ConfigDamageInteravl = Config.Bind("Settings",
			"Damage Interval",
			1.0f,
			"The intensity for each damage dealt to you. Shock will always be round up (0.1 - 100)");
		if (ConfigDamageInteravl.Value < 0.1 || ConfigDamageInteravl.Value > 100)
			throw new ArgumentOutOfRangeException("Damage Interval cannot be less than 0.1 or greater than 100");
		ConfigDeathIntensity = Config.Bind("Settings",
			"Death Intensity",
			50,
			"The intensity when you die. (1-100)");
		if (ConfigDeathIntensity.Value < 1 || ConfigDeathIntensity.Value > 100)
			throw new ArgumentOutOfRangeException("Death Intensity cannot be less than 1 or greater than 100");
		ConfigDeathDuration = Config.Bind("Settings",
			"Max Duration",
			3,
			"The duration when you die. (1-15)");
		if (ConfigDeathDuration.Value < 1 || ConfigDeathDuration.Value > 15)
			throw new ArgumentOutOfRangeException("Death Duration cannot be less than 1 or greater than 15");

		try
		{
			if (string.IsNullOrEmpty(configUserName.Value) || string.IsNullOrEmpty(configAPIKey.Value))
			{
				throw new ArgumentException("Username and APIKey cannot be empty! Please add your Username and APIKey to the config!");
			}
			if (string.IsNullOrEmpty(configShareCodes.Value))
			{
				throw new ArgumentException("You have to enter atleast 1 Share Code! If you have multiple, you can separate them with commas like so \"abc123,def456\".");
			}

			var sharecodes = configShareCodes.Value.Split(',');
			var devices = new List<PiShockDeviceInfo>();
			for (int i = 0; i < sharecodes.Length; i++)
			{
				devices.Add(new PiShockDeviceInfo($"Shocker {i}", sharecodes[i]));
			}
			
			PiShockUserInfo = await PiShockAPI.GetUserInfoFromAPI(configUserName.Value, configAPIKey.Value);
			PiShockUserInfo = PiShockUserInfo.WithDevices(devices);

			Logger.LogInfo($"Logged into PiShock as {PiShockUserInfo.Username}");

			PiShockController = new PiShockController(PiShockAPI, PiShockUserInfo);
		}
		catch (Exception ex)
		{ 
			Logger.LogError(ex.Message);
		}

	}
}