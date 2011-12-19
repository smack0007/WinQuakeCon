using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;

namespace WinQuakeCon
{
	public class MainForm : Form
	{
		Config config;

		NotifyIcon trayIcon;

		Process consoleProcess;
		int consoleWidth;
		int consoleHeight;

		public MainForm(Config config)
		{
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}

			this.config = config;

			this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
			this.ShowInTaskbar = false;
			this.WindowState = FormWindowState.Minimized;

			this.trayIcon = new NotifyIcon();
			this.trayIcon.Icon = WinQuakeCon.Properties.Resources.Icon;
			this.trayIcon.Visible = true;

			ContextMenu menu = new ContextMenu();
			menu.MenuItems.Add("Toggle Console", this.TrayIcon_ToggleConsole);
			menu.MenuItems.Add("Exit", this.TrayIcon_Exit);

			this.trayIcon.ContextMenu = menu;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			Win32.RegisterHotKey(this.Handle, 0, Win32.MOD_ALT, (uint)Keys.Space);
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			Win32.UnregisterHotKey(this.Handle, 0);

			if(this.consoleProcess != null && !this.consoleProcess.HasExited)
				this.consoleProcess.Kill();
		}

		private void TrayIcon_ToggleConsole(object sender, EventArgs e)
		{
			this.ToggleConsole();
		}

		private void TrayIcon_Exit(object sender, EventArgs e)
		{
			this.Close();
		}

		private void StartConsoleProcess()
		{
			ProcessStartInfo startInfo = new ProcessStartInfo()
			{
				FileName = this.config.Console,
			};

			this.consoleProcess = Process.Start(startInfo);
			this.consoleProcess.Exited += (s, e) => { this.consoleProcess = null; };

			while(this.consoleProcess.MainWindowHandle == IntPtr.Zero)
				Thread.Sleep(100);
						
			uint style = (uint)Win32.GetWindowLong(this.consoleProcess.MainWindowHandle, Win32.GWL_STYLE);
			style &= ~(Win32.WS_VISIBLE);
			
			uint exStyle = (uint)Win32.GetWindowLong(this.consoleProcess.MainWindowHandle, Win32.GWL_EXSTYLE);
			exStyle &= ~(Win32.WS_EX_APPWINDOW);
			exStyle |= Win32.WS_EX_TOOLWINDOW;

			Win32.ShowWindow(this.consoleProcess.MainWindowHandle, Win32.SW_HIDE);
			Win32.SetWindowLong(this.consoleProcess.MainWindowHandle, Win32.GWL_STYLE, (int)style);
			Win32.SetWindowLong(this.consoleProcess.MainWindowHandle, Win32.GWL_EXSTYLE, (int)exStyle);
			Win32.ShowWindow(this.consoleProcess.MainWindowHandle, Win32.SW_SHOW);

			this.consoleWidth = Screen.PrimaryScreen.WorkingArea.Width;
			this.consoleHeight = Screen.PrimaryScreen.WorkingArea.Height / 2;

			Win32.SetWindowPos(this.consoleProcess.MainWindowHandle, IntPtr.Zero, 0, -this.consoleHeight - 1, this.consoleWidth, this.consoleHeight, Win32.SWP_SHOWWINDOW);
		}

		private bool IsConsoleVisible()
		{
			Win32.RECT win32Rect = new Win32.RECT();
			Win32.GetWindowRect(this.consoleProcess.MainWindowHandle, out win32Rect);
			Rectangle windowRect = new Rectangle(win32Rect.Left, win32Rect.Top, win32Rect.Right - win32Rect.Top, win32Rect.Bottom - win32Rect.Top);

			return Screen.PrimaryScreen.WorkingArea.Contains(windowRect);
		}

		private void ToggleConsole()
		{
			if(this.consoleProcess == null || this.consoleProcess.HasExited)
				this.StartConsoleProcess();

			uint flags = Win32.SWP_SHOWWINDOW;

			if(!this.IsConsoleVisible())
			{
				Win32.SetWindowPos(this.consoleProcess.MainWindowHandle, Win32.HWND_TOPMOST, 0, 0, this.consoleWidth, this.consoleHeight, flags);
			}
			else
			{
				Win32.SetWindowPos(this.consoleProcess.MainWindowHandle, Win32.HWND_BOTTOM, 0, -this.consoleHeight - 1, this.consoleWidth, this.consoleHeight, flags);
			}
		}

		protected override void WndProc(ref Message m)
		{
			switch(m.Msg)
			{
				case Win32.WM_HOTKEY:
					this.ToggleConsole();
					break;

				default:
					base.WndProc(ref m);
					break;
			}
		}
	}
}
