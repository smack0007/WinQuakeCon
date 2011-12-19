using System;
using System.Runtime.InteropServices;

namespace WinQuakeCon
{
	internal static class Win32
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		public const int GWL_STYLE = -16;
		public const int GWL_EXSTYLE = -20;

		public static IntPtr HWND_TOPMOST = (IntPtr)(-1);
		public static IntPtr HWND_BOTTOM = (IntPtr)1;

		// Window Styles 
		public const uint WS_OVERLAPPED = 0;
		public const uint WS_POPUP = 0x80000000;
		public const uint WS_CHILD = 0x40000000;
		public const uint WS_MINIMIZE = 0x20000000;
		public const uint WS_VISIBLE = 0x10000000;
		public const uint WS_DISABLED = 0x8000000;
		public const uint WS_CLIPSIBLINGS = 0x4000000;
		public const uint WS_CLIPCHILDREN = 0x2000000;
		public const uint WS_MAXIMIZE = 0x1000000;
		public const uint WS_CAPTION = 0xC00000;      // WS_BORDER or WS_DLGFRAME  
		public const uint WS_BORDER = 0x800000;
		public const uint WS_DLGFRAME = 0x400000;
		public const uint WS_VSCROLL = 0x200000;
		public const uint WS_HSCROLL = 0x100000;
		public const uint WS_SYSMENU = 0x80000;
		public const uint WS_THICKFRAME = 0x40000;
		public const uint WS_GROUP = 0x20000;
		public const uint WS_TABSTOP = 0x10000;
		public const uint WS_MINIMIZEBOX = 0x20000;
		public const uint WS_MAXIMIZEBOX = 0x10000;
		public const uint WS_TILED = WS_OVERLAPPED;
		public const uint WS_ICONIC = WS_MINIMIZE;
		public const uint WS_SIZEBOX = WS_THICKFRAME;

		// Extended Window Styles 
		public const uint WS_EX_DLGMODALFRAME = 0x0001;
		public const uint WS_EX_NOPARENTNOTIFY = 0x0004;
		public const uint WS_EX_TOPMOST = 0x0008;
		public const uint WS_EX_ACCEPTFILES = 0x0010;
		public const uint WS_EX_TRANSPARENT = 0x0020;
		public const uint WS_EX_MDICHILD = 0x0040;
		public const uint WS_EX_TOOLWINDOW = 0x0080;
		public const uint WS_EX_WINDOWEDGE = 0x0100;
		public const uint WS_EX_CLIENTEDGE = 0x0200;
		public const uint WS_EX_CONTEXTHELP = 0x0400;
		public const uint WS_EX_RIGHT = 0x1000;
		public const uint WS_EX_LEFT = 0x0000;
		public const uint WS_EX_RTLREADING = 0x2000;
		public const uint WS_EX_LTRREADING = 0x0000;
		public const uint WS_EX_LEFTSCROLLBAR = 0x4000;
		public const uint WS_EX_RIGHTSCROLLBAR = 0x0000;
		public const uint WS_EX_CONTROLPARENT = 0x10000;
		public const uint WS_EX_STATICEDGE = 0x20000;
		public const uint WS_EX_APPWINDOW = 0x40000;
		public const uint WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE);
		public const uint WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST);
		public const uint WS_EX_LAYERED = 0x00080000;
		public const uint WS_EX_NOINHERITLAYOUT = 0x00100000; // Disable inheritence of mirroring by children
		public const uint WS_EX_LAYOUTRTL = 0x00400000; // Right to left mirroring
		public const uint WS_EX_COMPOSITED = 0x02000000;
		public const uint WS_EX_NOACTIVATE = 0x08000000;

		public const uint SWP_FRAMECHANGED = 0x0020;
		public const uint SWP_NOMOVE = 0x0002;
		public const uint SWP_NOSIZE = 0x0001;
		public const uint SWP_SHOWWINDOW = 0x0040;

		public const int SW_HIDE = 0;
		public const int SW_SHOW = 5;

		public const int MOD_ALT = 0x1;
		public const int MOD_CONTROL = 0x2;
		public const int MOD_SHIFT = 0x4;
		public const int MOD_WIN = 0x8;
		
		public const int WM_HOTKEY = 0x312;
						
		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();
			
		[DllImport("user32.dll")]
		public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

		[DllImport("user32.dll")]
		public static extern IntPtr SetForegroundWindow(IntPtr hwnd);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
				
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
	}
}
