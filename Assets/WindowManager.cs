using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    [DllImport("user32.dll")] private static extern IntPtr GetActiveWindow();
    [DllImport("user32.dll")] private static extern int SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);
    [DllImport("user32.dll")] private static extern bool SetWindowDisplayAffinity(IntPtr hwnd, uint dwAffinity);
    [DllImport("Dwmapi.dll")] private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);
    [DllImport("user32.dll")] static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    private struct MARGINS { public int cxLeftWidth; public int cxRightWidth; public int cxTopHeight; public int cxBottomHeight; }

    private const int GWL_EXSTYLE = -20;
    private const uint WS_EX_LAYERED = 0x00080000;
    private const uint WS_EX_TRANSPARENT = 0x00000020;
    private const uint WS_EX_TOOLWINDOW = 0x00000080;
    private const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;
    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

    void Start()
    {
#if !UNITY_EDITOR
        IntPtr unityWindowHandle = GetActiveWindow();
        MARGINS margins = new MARGINS { cxLeftWidth = -1 };
        DwmExtendFrameIntoClientArea(unityWindowHandle, ref margins);
        SetWindowLong(unityWindowHandle, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW);
        SetWindowDisplayAffinity(unityWindowHandle, WDA_EXCLUDEFROMCAPTURE);
        SetWindowPos(unityWindowHandle, HWND_TOPMOST, 0, 0, 0, 0, 0);
#endif
    }
}