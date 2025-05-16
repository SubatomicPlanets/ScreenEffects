using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class ScreenCapture : MonoBehaviour
{
    [DllImport("user32.dll")] private static extern IntPtr GetDesktopWindow();
    [DllImport("user32.dll")] private static extern IntPtr GetDC(IntPtr hwnd);
    [DllImport("user32.dll")] private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);
    [DllImport("gdi32.dll")] private static extern IntPtr CreateCompatibleDC(IntPtr hdc);
    [DllImport("gdi32.dll")] private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int width, int height);
    [DllImport("gdi32.dll")] private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
    [DllImport("gdi32.dll")] private static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int width, int height, IntPtr hdcSrc, int xSrc, int ySrc, uint dwRop);
    [DllImport("gdi32.dll")] private static extern bool DeleteDC(IntPtr hdc);
    [DllImport("gdi32.dll")] private static extern bool DeleteObject(IntPtr hObject);
    [DllImport("user32.dll")] private static extern int GetSystemMetrics(int nIndex);
    [DllImport("gdi32.dll")] private static extern int GetDIBits(IntPtr hdc, IntPtr hbmp, uint uStartScan, uint cScanLines, byte[] lpvBits, ref BITMAPINFO lpbi, uint uUsage);

    [StructLayout(LayoutKind.Sequential)] public struct BITMAPINFOHEADER { public uint biSize; public int biWidth; public int biHeight; public short biPlanes; public short biBitCount; public uint biCompression; public uint biSizeImage; public int biXPelsPerMeter; public int biYPelsPerMeter; public uint biClrUsed; public uint biClrImportant; }
    [StructLayout(LayoutKind.Sequential)] public struct BITMAPINFO { public BITMAPINFOHEADER bmiHeader; }

    private const uint SRCCOPY = 0x00CC0020;
    private const int SM_CXSCREEN = 0;
    private const int SM_CYSCREEN = 1;

    private Texture2D screenTexture;
    private int screenWidth;
    private int screenHeight;
    private BITMAPINFO bmi;
    private byte[] pixels;
    private IntPtr desktopDC;
    private IntPtr memoryDC;
    private IntPtr hBitmap;
    private IntPtr oldBitmap;

    [SerializeField] private Material mat;

    void Start()
    {
        screenWidth = GetSystemMetrics(SM_CXSCREEN);
        screenHeight = GetSystemMetrics(SM_CYSCREEN);

        screenTexture = new Texture2D(screenWidth, screenHeight, TextureFormat.RGB24, false);
        mat.mainTexture = screenTexture;

        bmi = new BITMAPINFO();
        bmi.bmiHeader.biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER));
        bmi.bmiHeader.biWidth = screenWidth;
        bmi.bmiHeader.biHeight = screenHeight;
        bmi.bmiHeader.biPlanes = 1;
        bmi.bmiHeader.biBitCount = 24;
        bmi.bmiHeader.biCompression = 0;
        bmi.bmiHeader.biSizeImage = (uint)(screenWidth * screenHeight * 3);

        pixels = new byte[bmi.bmiHeader.biSizeImage];

        desktopDC = GetDC(GetDesktopWindow());
        memoryDC = CreateCompatibleDC(desktopDC);
        hBitmap = CreateCompatibleBitmap(desktopDC, screenWidth, screenHeight);
        oldBitmap = SelectObject(memoryDC, hBitmap);
    }

    void LateUpdate()
    {
        CaptureScreen();
    }

    void CaptureScreen()
    {
        BitBlt(memoryDC, 0, 0, screenWidth, screenHeight, desktopDC, 0, 0, SRCCOPY);
        GetDIBits(memoryDC, hBitmap, 0, (uint)screenHeight, pixels, ref bmi, 0);

        screenTexture.LoadRawTextureData(pixels);
        screenTexture.Apply();
    }

    void OnDestroy()
    {
        SelectObject(memoryDC, oldBitmap);
        DeleteDC(memoryDC);
        DeleteObject(hBitmap);
        ReleaseDC(GetDesktopWindow(), desktopDC);
    }
}