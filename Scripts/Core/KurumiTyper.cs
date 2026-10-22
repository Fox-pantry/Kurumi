using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections;

public class KurumiTyper : MonoBehaviour
{
    [DllImport("user32.dll")]
    static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, System.UIntPtr dwExtraInfo);

    private const byte VK_RETURN = 0x0D;
    private const byte VK_SPACE = 0x20;
    private const byte VK_BACK = 0x08;
    private const byte VK_CONTROL = 0x11;
    private const byte VK_V = 0x56;
    private const byte VK_A = 0x41;
    private const byte VK_C = 0x43;

    private const uint KEYEVENTF_KEYDOWN = 0x0000;
    private const uint KEYEVENTF_KEYUP = 0x0002;

    public void TypeText(string text)
    {
        StartCoroutine(TypeCoroutine(text));
    }

    IEnumerator TypeCoroutine(string text)
    {
        yield return new WaitForSeconds(0.3f);

        foreach (char c in text)
        {
            TypeCharacter(c);
            yield return new WaitForSeconds(0.03f);
        }

        EventBus.Trigger("OnTypeResult", "Готово!");
    }

    void TypeCharacter(char c)
    {
        if (c == '\n') PressEnter();
        else if (c == ' ') PressSpace();
        else
        {
            bool needShift = char.IsUpper(c) || "!@#$%^&*()_+{}|:\"<>?~".Contains(c);
            char lowerChar = char.ToLower(c);
            byte vk = GetVirtualKeyCode(lowerChar);

            if (vk != 0)
            {
                if (needShift) keybd_event(0x10, 0, KEYEVENTF_KEYDOWN, System.UIntPtr.Zero);
                keybd_event(vk, 0, KEYEVENTF_KEYDOWN, System.UIntPtr.Zero);
                keybd_event(vk, 0, KEYEVENTF_KEYUP, System.UIntPtr.Zero);
                if (needShift) keybd_event(0x10, 0, KEYEVENTF_KEYUP, System.UIntPtr.Zero);
            }
        }
    }

    byte GetVirtualKeyCode(char c)
    {
        if (c >= 'a' && c <= 'z') return (byte)(0x41 + (c - 'a'));
        if (c >= '0' && c <= '9') return (byte)(0x30 + (c - '0'));
        switch (c)
        {
            case '-': return 0xBD;
            case '.': return 0xBE;
            case ',': return 0xBC;
            case ';': return 0xBA;
            case '\'': return 0xDE;
            case '[': return 0xDB;
            case ']': return 0xDD;
            default: return 0;
        }
    }

    public void PressEnter()
    {
        keybd_event(VK_RETURN, 0, KEYEVENTF_KEYDOWN, System.UIntPtr.Zero);
        keybd_event(VK_RETURN, 0, KEYEVENTF_KEYUP, System.UIntPtr.Zero);
    }

    public void PressSpace()
    {
        keybd_event(VK_SPACE, 0, KEYEVENTF_KEYDOWN, System.UIntPtr.Zero);
        keybd_event(VK_SPACE, 0, KEYEVENTF_KEYUP, System.UIntPtr.Zero);
    }

    public void Copy()
    {
        keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYDOWN, System.UIntPtr.Zero);
        keybd_event(VK_C, 0, KEYEVENTF_KEYDOWN, System.UIntPtr.Zero);
        keybd_event(VK_C, 0, KEYEVENTF_KEYUP, System.UIntPtr.Zero);
        keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, System.UIntPtr.Zero);
    }

    public void Paste()
    {
        keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYDOWN, System.UIntPtr.Zero);
        keybd_event(VK_V, 0, KEYEVENTF_KEYDOWN, System.UIntPtr.Zero);
        keybd_event(VK_V, 0, KEYEVENTF_KEYUP, System.UIntPtr.Zero);
        keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, System.UIntPtr.Zero);
    }

    public void SelectAll()
    {
        keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYDOWN, System.UIntPtr.Zero);
        keybd_event(VK_A, 0, KEYEVENTF_KEYDOWN, System.UIntPtr.Zero);
        keybd_event(VK_A, 0, KEYEVENTF_KEYUP, System.UIntPtr.Zero);
        keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, System.UIntPtr.Zero);
    }
}