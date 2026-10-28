using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System.Net.Http;
using BepInEx.Configuration;
using Steamworks;
using ExitGames.Client.Photon.StructWrapping;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using REPOShock.src.PiShockLibrary.Services;
using REPOShock.src.PiShockLibrary.Models;
using REPOShock.src.PiShockLibrary.Utilities;
using Steamworks.Data;

namespace REPOShock.src;

[BepInPlugin("mxpuffin.REPOShock", "REPOShock", "1.0")]
public class REPOShock : BaseUnityPlugin
{
    internal static REPOShock Instance { get; private set; } = null!;
    internal static ConfigFile CFG { get; private set; } = null!;
    internal new static ManualLogSource Logger => Instance._logger;
    private ManualLogSource _logger => base.Logger;
    internal Harmony? Harmony { get; set; }

    private HttpClient Client { get; set; } = null!;
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

        Client = new HttpClient();

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

    private async Task LoadConfigAndLogin()
    {
        ConfigHandler.InitConfig();
        var _configAPIKey = CFG.Bind("Auth_PiShock", "APIKey", "", "Your PiShock API Key");

        try
        {
            if (string.IsNullOrWhiteSpace(_configAPIKey.Value))
            {
                throw new ArgumentException("Username and APIKey cannot be empty! Please add your Username and APIKey to the config!");
            }

            PiShockAPI = new PiShockAPI(Client, _configAPIKey.Value.Trim());
            var userInfo = await PiShockAPI.GetUserInfo();
            if (userInfo == null) { return; }
            Logger.LogInfo($"Successfully logged in as {userInfo.Username} ({userInfo.UserId})");
            
            userInfo = await PiShockAPI.GetUserShockers(userInfo!);
            if (userInfo == null) { return; }
            
            var controller = new PiShockController(PiShockAPI, userInfo!);

            PiShockUserInfo = userInfo;
            PiShockController = controller;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.ToString());
        }


    }
}
