namespace REPOShock.src.Auth;

internal static class ApiKeyStore
{
    private const string FileName = "REPOShock.es3";
    private const string KeyName = "pishockApiKey";

    public static void SaveKey(string apiKey) =>
        ES3.Save(KeyName, apiKey.Trim(), FileName);

    public static string LoadKey() =>
        ES3.KeyExists(KeyName, FileName) ? ES3.Load<string>(KeyName, FileName) : "";
}