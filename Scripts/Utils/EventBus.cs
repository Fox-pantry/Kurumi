using System;

public static class EventBus
{
    public static event Action<float, float> OnVoiceDetected;
    public static event Action<string, float> OnKurumiSpeak;
    public static event Action<string, float> OnCommand;
    public static event Action<string> OnAppLaunchResult;
    public static event Action<string> OnTypeResult;
    public static event Action<string> OnInternetResult;

    public static void Trigger(string eventName, params object[] args)
    {
        try
        {
            switch (eventName)
            {
                case "OnVoiceDetected":
                    if (OnVoiceDetected != null && args.Length >= 2)
                        OnVoiceDetected((float)args[0], (float)args[1]);
                    break;
                case "OnKurumiSpeak":
                    if (OnKurumiSpeak != null && args.Length >= 2)
                        OnKurumiSpeak((string)args[0], (float)args[1]);
                    break;
                case "OnCommand":
                    if (OnCommand != null && args.Length >= 2)
                        OnCommand((string)args[0], (float)args[1]);
                    break;
                case "OnAppLaunchResult":
                    if (OnAppLaunchResult != null && args.Length >= 1)
                        OnAppLaunchResult((string)args[0]);
                    break;
                case "OnTypeResult":
                    if (OnTypeResult != null && args.Length >= 1)
                        OnTypeResult((string)args[0]);
                    break;
                case "OnInternetResult":
                    if (OnInternetResult != null && args.Length >= 1)
                        OnInternetResult((string)args[0]);
                    break;
            }
        }
        catch (Exception e) { UnityEngine.Debug.LogError($"EventBus: {e.Message}"); }
    }
}