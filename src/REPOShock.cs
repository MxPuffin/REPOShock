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
using PiShockClassLibrary.Interfaces;

namespace REPOShock.src;

[BepInPlugin("mxpuffin.REPOShock", "REPOShock", "1.0")]
public class REPOShock : BaseUnityPlugin
{
    internal static REPOShock Instance { get; private set; } = null!;
    internal static ConfigFile CFG { get; private set; }
    internal new static ManualLogSource Logger => Instance._logger;
    private ManualLogSource _logger => base.Logger;
    internal Harmony? Harmony { get; set; }

    private HttpClient? httpClient { get; set; }
    private PiShockAPI? PiShockAPI { get; set; }
    private PiShockUserInfo? PiShockUserInfo { get; set; }
    internal static PiShockController? PiShockController { get; set; }



    private async void Awake()
    {
        Instance = this;

        // Prevent the plugin from being deleted
        gameObject.transform.parent = null;
        gameObject.hideFlags = HideFlags.HideAndDontSave;
        CFG = Config;


        httpClient = new HttpClient();
        PiShockAPI = new PiShockAPI(httpClient);

        PiShockLogger.LogInfoAction = Logger.LogInfo;
        PiShockLogger.LogErrorAction = Logger.LogError;

        await LoadConfigAndLogin();

		ModGlobals.SteamID = SteamClient.SteamId.ToString();
		Logger.LogInfo($"Setting steam ID for the session: {ModGlobals.SteamID}");


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

    private async Task LoadConfigAndLogin()
    {
        ConfigHandler.InitConfig();
        // API Config
        var _configUserName = CFG.Bind("Auth_PiShock",
            "UserName",
            "",
            "Your PiShock username");
        var _configAPIKey = CFG.Bind("Auth_PiShock",
            "APIKey",
            "",
            "Your PiShock API Key");
        var _configShareCodes = CFG.Bind("Auth_PiShock",
        "ShareCodes",
        "",
			"Enter your sharecodes here! If you have multiple, you can separate them with commas like so \"abc123,def456\"");

        try
        {
            if (string.IsNullOrEmpty(_configUserName.Value) || string.IsNullOrEmpty(_configAPIKey.Value))
                throw new ArgumentException("Username and APIKey cannot be empty! Please add your Username and APIKey to the config!");

            if (string.IsNullOrEmpty(_configShareCodes.Value))
                throw new ArgumentException("You have to enter atleast 1 Share Code! If you have multiple, you can separate them with commas like so \"abc123,def456\".");
            var sharecodes = _configShareCodes.Value.Split(',');
            var devices = new List<PiShockDeviceInfo>();
            for (int i = 0; i < sharecodes.Length; i++)
            {
                devices.Add(new PiShockDeviceInfo($"Shocker {i}", sharecodes[i]));
            }

            Logger.LogInfo(_configAPIKey.Value);

			var userInfo = await PiShockAPI.GetUserInfoFromAPI(_configUserName.Value, _configAPIKey.Value);
            userInfo = userInfo.WithDevices(devices);

            var controller = new PiShockController(PiShockAPI, userInfo);

            PiShockUserInfo = userInfo;
            PiShockController = controller;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.ToString());
        }


    }
}
