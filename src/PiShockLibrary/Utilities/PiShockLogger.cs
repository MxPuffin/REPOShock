using System;

namespace REPOShock.src.PiShockLibrary.Utilities;

public static class PiShockLogger
{
    public static Action<string>? LogInfoAction { get; set; }
    public static Action<string>? LogErrorAction { get; set; }

    public static void Log(string message)
    {
        LogInfoAction?.Invoke($"[PiShock] {message}");
    }

    public static void LogError(string message)
    {
        LogErrorAction?.Invoke($"[PiShock] {message}");
    }

}
