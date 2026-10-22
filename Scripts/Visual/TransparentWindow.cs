using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class TransparentWindow : MonoBehaviour
{
    [DllImport("user32.dll")]
    static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll")]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    private const int GWL_EXSTYLE = -20;
    private const uint WS_EX_LAYERED = 0x80000;
    private const uint WS_EX_TRANSPARENT = 0x20;
    private const uint WS_EX_TOOLWINDOW = 0x80;
    private const int SW_HIDE = 0;
    private const int SW_SHOW = 5;

    private IntPtr hwnd;

    void Start()
    {
#if !UNITY_EDITOR
        hwnd = GetActiveWindow();
        SetWindowLong(hwnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW);
        SetWindowPos(hwnd, new IntPtr(-1), 0, 0, 0, 0, 0x0002 | 0x0001);
        ShowWindow(hwnd, SW_HIDE);
#endif

        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.backgroundColor = new Color(0, 0, 0, 0);
    }

    void OnApplicationQuit()
    {
#if !UNITY_EDITOR
        ShowWindow(hwnd, SW_SHOW);
#endif
    }
}