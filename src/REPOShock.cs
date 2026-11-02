using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System.Net.Http;
using BepInEx.Configuration;
using Steamworks;
using System;
using System.Threading.Tasks;
using REPOShock.src.PiShockLibrary.Services;
using REPOShock.src.PiShockLibrary.Models;
using REPOShock.src.Auth;
using REPOShock.src.Menu;
using SingularityGroup.HotReload;
using Unity.Collections;
using System.Linq;
using REPOShock.src.PiShockLibrary.Utilities;

namespace REPOShock.src;

[BepInPlugin("mxpuffin.REPOShock", "REPOShock", "1.1.0")]
public class REPOShock : BaseUnityPlugin
{
    internal static REPOShock Instance { get; private set; } = null!;
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

        gameObject.transform.parent = null;
        gameObject.hideFlags = HideFlags.HideAndDontSave;

        Client = new HttpClient();

        PiShockLogger.LogInfoAction = Logger.LogInfo;
        PiShockLogger.LogErrorAction = Logger.LogError;

        ConfigHandler.InitConfig(Config);
        ApiKeyMenu.Init();
        Patch();

        await SetSteamIdWhenReady();
        await Login(ApiKeyStore.LoadKey());

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

    internal async Task<bool> SetApiKey(string newKey)
    {
        ApiKeyStore.SaveKey(newKey);
        return await Login(newKey);
    }

    internal async Task<bool> Login(string apiKey)
    {
        PiShockController = null;
        PiShockAPI = null;
        PiShockUserInfo = null;

        try
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                Logger.LogWarning("No PiShock APIKey set. Enter one in the REPO Settings");
                LoginState.Set(LoginStatus.NoKey);
                return false;
            }

            LoginState.Set(LoginStatus.LoggingIn);
            var api = new PiShockAPI(Client, apiKey.Trim());

            var userInfo = await api.GetUserInfo();
            if (userInfo is null)
            {
                LoginState.Set(LoginStatus.Failed);
                Logger.LogError("Failed to fetch user info, this is likely due to an invalid ApiKey.");
                return false;
            }
            Logger.LogInfo($"Successfully logged in as {userInfo.Username} ({userInfo.UserId})");

            userInfo = await api.GetUserShockers(userInfo);
            if (userInfo is null)
            {
                Logger.LogError("Failed to fetch any sharecodes from your account, make sure you own atleast one sharecode for your device.");
                return false;
            }

            PiShockAPI = api;
            PiShockUserInfo = userInfo;
            PiShockController = new PiShockController(api, userInfo);
            LoginState.Set(LoginStatus.LoggedIn, userInfo.Username);
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.ToString());
            LoginState.Set(LoginStatus.Failed);
            return false;
        }
    }

    private async Task SetSteamIdWhenReady()
    {
        for (int i = 0; i < 300 && !SteamClient.IsValid; i++)
        {
            await Task.Delay(100);
        }

        if (!SteamClient.IsValid)
        {
            Logger.LogWarning("Steam client not available, SteamID not set.");
            return;
        }

        ModGlobals.SteamID = SteamClient.SteamId.ToString();
        Logger.LogInfo($"Setting steam ID for the session: {ModGlobals.SteamID}");
    }
}
