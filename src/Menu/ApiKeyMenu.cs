using System;
using System.Threading.Tasks;
using MenuLib;
using MenuLib.MonoBehaviors;
using REPOShock.src.Auth;
using UnityEngine;

namespace REPOShock.src.Menu;

internal static class ApiKeyMenu
{
    private const string FileName = "REPOShock.es3";
    private const string KeyName = "pishockApiKey";

    private static bool LoggingIn = false;

    public static void Init()
    {
        MenuAPI.AddElementToMainMenu(x =>
        {
            MenuAPI.CreateREPOButton(
                "REPOShock",
                OpenPopup,
                x,
                new Vector2(550, 300)
            );
        });
    }

    private static void OpenPopup()
    {
        var popup = MenuAPI.CreateREPOPopupPage(
            "REPOShock Configuration",
            REPOPopupPage.PresetSide.Left,
            shouldCachePage: false,
            pageDimmerVisibility: true,
            spacing: 0f
        );

        string pendingKey = string.Empty;
        REPOLabel? statusLabel = null;
        Action? handler = null;

        popup.AddElement(x =>
        {
            statusLabel = MenuAPI.CreateREPOLabel(StatusText(), x, new Vector2(100f, 150f));
        });

        handler = () =>
        {
            if (statusLabel == null)
            {
                LoginState.Changed -= handler;
                return;
            }
            statusLabel.labelTMP.text = StatusText();
        };
        LoginState.Changed += handler;

        popup.AddElement(x =>
        {
            var y = MenuAPI.CreateREPOInputField(
                "New API Key",
                value => pendingKey = value,
                x,
                new Vector2(100f, 100f),
                false,
                "Paste new key here",
                ""
            );
        });

        popup.AddElement(x =>
        {
            MenuAPI.CreateREPOButton("Save", async () =>
            {
                if (LoggingIn || string.IsNullOrWhiteSpace(pendingKey)) { return; }
                LoggingIn = true;
                try
                {
                    var ok = await REPOShock.Instance.SetApiKey(pendingKey.Trim());
                    REPOShock.Logger.LogInfo(ok ? "PiShock login success" : "PiShock login failed");
                    await Task.Delay(1_000);
                    popup.ClosePage(true);
                }
                finally
                {
                    LoggingIn = false;
                }
            }, x, new Vector2(100f, 50f));
        });

        popup.OpenPage(false);
    }

    private static string Mask(string input) =>
        string.IsNullOrWhiteSpace(input) ? "none" : new string('•', Math.Max(0, input.Length - 4)) + input.Substring(Math.Max(0, input.Length - 4));

    private static string StatusText() => LoginState.Status switch
    {
        LoginStatus.LoggedIn => $"<color=green>Connected as {LoginState.Username}</color>",
        LoginStatus.LoggingIn => "<color=yellow>Logging in...</color>",
        LoginStatus.Failed => "<color=red>Login failed - check your API key</color>",
        _ => "<color=grey>No API key set</color>",
    };
}