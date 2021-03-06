﻿using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace FlashDevelop
{
    class Win32
    {
        public const Int32 SW_RESTORE = 9;
        public const UInt32 SWP_SHOWWINDOW = 64;

        [DllImport("user32.dll")]
        public static extern Boolean IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern UInt32 SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);

        [DllImport("user32.dll")]
        public static extern Boolean SystemParametersInfo(UInt32 uiAction, UInt32 uiParam, UInt32 pvParam, UInt32 fWinIni);

        [DllImport("user32.dll")]
        public static extern void SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, Int32 x, Int32 y, Int32 width, Int32 height, UInt32 flags);

        /// <summary>
        /// Sets the window specified by handle to fullscreen
        /// </summary>
        public static void SetWinFullScreen(IntPtr hwnd)
        {
            Screen screen = Screen.FromHandle(hwnd);
            Int32 screenTop = screen.WorkingArea.Top;
            Int32 screenLeft = screen.WorkingArea.Left;
            Int32 screenWidth = screen.WorkingArea.Width;
            Int32 screenHeight = screen.WorkingArea.Height;
            Win32.SetWindowPos(hwnd, IntPtr.Zero, screenLeft, screenTop, screenWidth, screenHeight, Win32.SWP_SHOWWINDOW);
        }

        /// <summary>
        /// Activates the window with Win32
        /// </summary>
        public static void ActivateWindow(IntPtr handle)
        {
            Win32.SystemParametersInfo((UInt32)0x2001, 0, 0, 0x0002 | 0x0001);
            if (Win32.IsIconic(handle)) Win32.ShowWindow(handle, Win32.SW_RESTORE);
            else Win32.SetForegroundWindow(handle); // Bring window to front...
            Win32.SystemParametersInfo((UInt32)0x2001, 200000, 200000, 0x0002 | 0x0001);
        }

    }

}
