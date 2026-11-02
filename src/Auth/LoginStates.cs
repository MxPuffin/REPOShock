using System;

namespace REPOShock.src.Auth;

internal enum LoginStatus { NoKey, LoggingIn, LoggedIn, Failed }

internal static class LoginState
{
    public static LoginStatus Status { get; private set; } = LoginStatus.NoKey;
    public static string? Username { get; private set; }
    public static event Action? Changed;

    public static void Set(LoginStatus status, string? username = null)
    {
        Status = status;
        Username = status == LoginStatus.LoggedIn ? username : null;
        Changed?.Invoke();
    }
}