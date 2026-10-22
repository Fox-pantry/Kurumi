using UnityEngine;

public static class GlobalState
{
    public static float LastVoiceTime = 0f;
    public static string LastCommand = "";
    public static int AppLaunchCount = 0;
    public static System.DateTime StartupTime = System.DateTime.Now;
}