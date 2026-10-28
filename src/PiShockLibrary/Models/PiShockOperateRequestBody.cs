using Newtonsoft.Json;
using REPOShock.src.PiShockLibrary.Enums;

namespace REPOShock.src.PiShockLibrary.Models;

public class PiShockOperateRequestBody
{
    [JsonProperty("AgentName")] public string AgentName { get; set; }
    [JsonProperty("Operation")] public int Operation { get; set; }
    [JsonProperty("Duration")] public int Duration { get; set; }
    [JsonProperty("Intensity")] public int Intensity { get; set; }
    [JsonProperty("MinimumDuration")] public int? MinimumDuration { get; set; } = null;
    [JsonProperty("MinimumIntensity")] public int? MinimumIntensity { get; set; } = null;
    [JsonProperty("IntensityAsPercentage")] public bool? IntensityAsPercentage { get; set; } = null;

    public PiShockOperateRequestBody(PiShockOperation op, int intesnity, int duration, string agentName = "PiShock App")
    {
        Operation = (int)op;
        Intensity = intesnity;
        Duration = duration < 16 ? duration * 1000 : duration;
        AgentName = agentName;
    }
}