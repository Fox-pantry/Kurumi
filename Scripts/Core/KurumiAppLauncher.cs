using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

public class KurumiAppLauncher : MonoBehaviour
{
    private Dictionary<string, string> appPaths = new Dictionary<string, string>();

    void Start()
    {
        appPaths["браузер"] = "msedge.exe";
        appPaths["хром"] = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
        appPaths["блокнот"] = "notepad.exe";
        appPaths["калькулятор"] = "calc.exe";
        appPaths["проводник"] = "explorer.exe";
        appPaths["paint"] = "mspaint.exe";
        appPaths["командная строка"] = "cmd.exe";
        appPaths["vs code"] = @"C:\Users\" + System.Environment.UserName + @"\AppData\Local\Programs\Microsoft VS Code\Code.exe";
        appPaths["steam"] = @"C:\Program Files (x86)\Steam\Steam.exe";
    }

    public void LaunchApp(string appName)
    {
        appName = appName.ToLower().Trim();

        if (appPaths.ContainsKey(appName))
        {
            ExecuteApp(appPaths[appName], appName);
            return;
        }

        foreach (var key in appPaths.Keys)
        {
            if (key.Contains(appName) || appName.Contains(key))
            {
                ExecuteApp(appPaths[key], key);
                return;
            }
        }

        EventBus.Trigger("OnAppLaunchResult", $"Не знаю, как открыть '{appName}'");
    }

    void ExecuteApp(string path, string name)
    {
        try
        {
            if (path.StartsWith("C:\\") && !File.Exists(path))
            {
                EventBus.Trigger("OnAppLaunchResult", $"Приложение '{name}' не найдено");
                return;
            }

            Process.Start(path);
            GlobalState.AppLaunchCount++;
            EventBus.Trigger("OnAppLaunchResult", $"Запускаю {name}!");
        }
        catch (System.Exception e)
        {
            EventBus.Trigger("OnAppLaunchResult", $"Ошибка: {e.Message}");
        }
    }
}