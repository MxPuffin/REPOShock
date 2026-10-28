using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using REPOShock.src.PiShockLibrary.Models;

namespace REPOShock.src.PiShockLibrary.Models;

public class PiShockUserInfo
{
    public int UserId { get; }
    // public int ClientID { get; } = clientId;
    public string Username { get; }
    public string ApiKey { get; }
    public IReadOnlyList<PiShockDeviceInfo> Devices { get; } 

    public PiShockUserInfo(int userId, string username, string apiKey, List<PiShockDeviceInfo>? devices = null)
    {
        UserId = userId;
        Username = username ?? throw new ArgumentNullException(nameof(username));
        ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        Devices = devices?.AsReadOnly() ?? new List<PiShockDeviceInfo>().AsReadOnly();
    }

    public PiShockUserInfo WithDevices(List<PiShockDeviceInfo> devices)
    {
        return new PiShockUserInfo(UserId, Username, ApiKey, devices);
    }
}

