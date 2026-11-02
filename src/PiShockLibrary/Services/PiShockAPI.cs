
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using REPOShock.src.PiShockLibrary.Models.Response;
using REPOShock.src.PiShockLibrary.Models;
using Newtonsoft.Json;
using REPOShock.src.PiShockLibrary.Exceptions;
using ExitGames.Client.Photon.StructWrapping;
using BepInEx.Logging;
using UnityEngine.CrashReportHandler;


namespace REPOShock.src.PiShockLibrary.Services;

/// <summary>
/// An interface for PiShock API endpoint definitions
/// </summary>
public interface IPiShockAPI
{
    /// <summary>
    /// Fetches a users account info from the API if APIKey is valid
    /// </summary>
    /// <returns>An object containing user account info</returns>
    Task<PiShockUserInfo?> GetUserInfo();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userInfo"></param>
    /// <returns></returns>
    Task<PiShockUserInfo?> GetUserShockers(PiShockUserInfo userInfo);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="requestBody"></param>
    /// <param name="shokcerId"></param>
    /// <returns></returns>
    Task<bool> SendOperationRequest(PiShockOperateRequestBody requestBody, int shokcerId);
}


public class PiShockAPI : IPiShockAPI
{
    private readonly HttpClient Client;
    private readonly string BaseUrl = "https://api.pishock.com";
    private readonly JsonSerializerSettings JsonSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore
    };


    public PiShockAPI(HttpClient httpClient, string APIKey, string? url = null)
    {
        if (string.IsNullOrWhiteSpace(APIKey)) { throw new ArgumentNullException(nameof(APIKey), "APIKey provided is null"); }

        Client = httpClient;
        Client.BaseAddress = new(url ?? BaseUrl);
        Client.DefaultRequestHeaders.Clear();
        Client.DefaultRequestHeaders.Add("X-PiShock-Api-Key", APIKey);
    }

    /// <inheritdoc />
    public async Task<PiShockUserInfo?> GetUserInfo()
    {
        REPOShock.Logger.LogInfo("Validating ApiKey...");
        try
        {
            var res = await Client.GetAsync("/Account");
            res.EnsureSuccessStatusCode();
            var userInfo = JsonConvert.DeserializeObject<AccountResponse>(await res.Content.ReadAsStringAsync() ?? throw new PiShockAPIException("Failed to deserialize account information."));

            // REPOShock.Logger.LogInfo($"Login Successful!: {userInfo.Username}");
            return new PiShockUserInfo(userInfo.UserId, userInfo.Username, Client.DefaultRequestHeaders.First(x => x.Key == "X-PiShock-Api-Key").Value.ToString());
        }
        catch (HttpRequestException ex)
        {
            REPOShock.Logger.LogError($"Failed to validate your ApiKey, {ex.Message}");
        }
        catch (Exception ex)
        {
            REPOShock.Logger.LogError(ex.Message);
        }
        return null;
    }

    /// <inheritdoc />
    public async Task<PiShockUserInfo?> GetUserShockers(PiShockUserInfo userInfo)
    {
        REPOShock.Logger.LogInfo("Attempting to fetch users Shockers");

        try
        {
            var res = await Client.GetAsync("/Share/GetShared");
            res.EnsureSuccessStatusCode();
            
            var shockers = JsonConvert.DeserializeObject<ShockerResponse[]>(await res.Content.ReadAsStringAsync())?.Where(x => x.OwnerId == userInfo.UserId && !x.Paused).ToArray() ?? throw new PiShockAPIException("Failed to get response from Shockers endpoint");
            if (shockers.Length == 0) { throw new PiShockDeviceException("No devices were found on your account"); }
            if (shockers.Any(x => !x.IsV3)) { throw new PiShockDeviceException("One or more devices are not V3 compatible"); }

            return userInfo.WithDevices(shockers.Select(x => new PiShockDeviceInfo(x.Name, x.ShareCode, x.Id, !x.Paused)).ToList());
        }
        catch (HttpRequestException ex)
        {
            REPOShock.Logger.LogError($"Failed to fetch your devices, {ex.Message}");
        }
        catch (Exception ex)
        {
            REPOShock.Logger.LogError(ex.Message);
        }
        return null;
    }

    /// <inheritdoc />
    public async Task<bool> SendOperationRequest(PiShockOperateRequestBody req, int shockerId)
    {
        try
        {
            var res = await Client.PostAsync($"/Shockers/OperateById/{shockerId}", new StringContent(JsonConvert.SerializeObject(req, JsonSettings), Encoding.UTF8, "application/json"));
            res.EnsureSuccessStatusCode();

            return true;
        }
        catch (HttpRequestException ex)
        {
            REPOShock.Logger.LogError($"Failed to operate your device, {ex.Message}");
        }
        catch (Exception ex)
        {
            REPOShock.Logger.LogError($"Error in SendOperationRequest: {ex.Message}");
        }
        return false;
    }
}
