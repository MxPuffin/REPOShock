namespace REPOShock.src.PiShockLibrary.Models.Response;

internal sealed class ShockerResponse
{
    public int OwnerId { get; set; }
    public int ClientId { get; set; }
    public string Name { get; set; }
    public int Type { get; set; }
    public int Id { get; set; }
    public bool Paused { get; set; }
    public bool CanPause { get; set; }
    public string OwnedBy { get; set; }
    public int MaxIntensity { get; set; }
    public int MaxDuration { get; set; }
    public bool SharePaused { get; set; }
    public bool CanHold { get; set; }
    public int ShareId { get; set; }
    public bool CanBeep { get; set; }
    public bool CanShock { get; set; }
    public bool CanVibrate { get; set; }
    public string ShareCode { get; set; }
    public bool CanLog { get; set; }
    public bool CanCreateLinks { get; set; }
    public bool IsV3 { get; set; }
    public bool Locked { get; set; }
}