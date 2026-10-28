namespace REPOShock.src.PiShockLibrary.Models;

public class PiShockDeviceInfo
{
    public string ShockerName { get; private set; }
    public string ShareCode { get; private set; }
    public int ShockerId { get; private set; }
    public bool IsEnabled { get; private set; }

    public PiShockDeviceInfo(string name, string shareCode, int shockerId, bool enabled)
    {
        ShockerName = name;
        ShareCode = shareCode;
        ShockerId = shockerId;
        IsEnabled = enabled;
    }
}


