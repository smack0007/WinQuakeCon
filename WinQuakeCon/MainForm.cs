using System;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

using Timer = System.Threading.Timer;

namespace WinQuakeCon
{
	public class MainForm : Form
	{
		Config config;

		NotifyIcon trayIcon;

		Process consoleProcess;
		
		IntPtr oldForeground;

		public MainForm(Config config)
		{
			if (config == null)
				throw new ArgumentNullException("config");

			this.config = config;

			this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
			this.ShowInTaskbar = false;
			this.WindowState = FormWindowState.Minimized;
			this.Visible = false;

			this.trayIcon = new NotifyIcon();
			this.trayIcon.Icon = WinQuakeCon.Properties.Resources.Icon;
			this.trayIcon.Visible = true;

			ContextMenu menu = new ContextMenu();
			menu.MenuItems.Add("Toggle Console", this.TrayIcon_ToggleConsole);
			menu.MenuItems.Add("Reload Config", this.TrayIcon_ReloadConfig);
			menu.MenuItems.Add("Exit", this.TrayIcon_Exit);

			this.trayIcon.ContextMenu = menu;
			this.trayIcon.DoubleClick += this.TrayIcon_ToggleConsole;
		}

		public void Initialize()
		{
			uint fsModifiers = 0;

			if (this.config.HotKeyAlt)
				fsModifiers |= Win32.MOD_ALT;
			
			if (this.config.HotKeyCtrl)
				fsModifiers |= Win32.MOD_CONTROL;

			if (this.config.HotKeyShift)
				fsModifiers |= Win32.MOD_SHIFT;

			if (this.config.HotKeyWin)
				fsModifiers |= Win32.MOD_WIN;

			Win32.RegisterHotKey(this.Handle, 0, fsModifiers, (uint)this.config.HotKeyCode);
		}
		
		public void Shutdown()
		{
			Win32.UnregisterHotKey(this.Handle, 0);

			if(this.consoleProcess != null && !this.consoleProcess.HasExited)
				this.consoleProcess.Kill();
		}

		private void TrayIcon_ToggleConsole(object sender, EventArgs e)
		{
			this.ToggleConsole();
		}

		private void TrayIcon_ReloadConfig(object sender, EventArgs e)
		{
			Config config = Config.Load("Config.xml");

			if (config == null)
			{
				MessageBox.Show(this, "Failed to load Config.xml.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			this.config = config;
		}

		private void TrayIcon_Exit(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void StartConsoleProcess()
		{
			ProcessStartInfo startInfo = new ProcessStartInfo()
			{
				FileName = this.config.Console,
				WorkingDirectory = this.config.WorkingDirectory			
			};

			this.consoleProcess = Process.Start(startInfo);

			while(this.consoleProcess.MainWindowHandle == IntPtr.Zero)
				Thread.Sleep(100);
						
			uint style = (uint)Win32.GetWindowLong(this.consoleProcess.MainWindowHandle, Win32.GWL_STYLE);
			style &= ~(Win32.WS_VISIBLE);
			
			uint exStyle = (uint)Win32.GetWindowLong(this.consoleProcess.MainWindowHandle, Win32.GWL_EXSTYLE);
			exStyle &= ~(Win32.WS_EX_APPWINDOW);
			exStyle |= Win32.WS_EX_TOOLWINDOW;

			if (config.ConsoleRemoveBorder)
			{
				style &= ~(Win32.WS_CAPTION | Win32.WS_THICKFRAME | Win32.WS_MINIMIZE | Win32.WS_MAXIMIZE | Win32.WS_SYSMENU);
				exStyle &= ~(Win32.WS_EX_DLGMODALFRAME | Win32.WS_EX_CLIENTEDGE | Win32.WS_EX_STATICEDGE);
			}

			Win32.ShowWindow(this.consoleProcess.MainWindowHandle, Win32.SW_HIDE);
			Win32.SetWindowLong(this.consoleProcess.MainWindowHandle, Win32.GWL_STYLE, (int)style);
			Win32.SetWindowLong(this.consoleProcess.MainWindowHandle, Win32.GWL_EXSTYLE, (int)exStyle);
			
			Win32.SetWindowPos(this.consoleProcess.MainWindowHandle, IntPtr.Zero, this.config.ConsoleHiddenX, this.config.ConsoleHiddenY, this.config.ConsoleWidth, this.config.ConsoleHeight, Win32.SWP_HIDEWINDOW);
		}

		private Rectangle GetConsoleRectangle()
		{
			Win32.RECT rect = new Win32.RECT();
			Win32.GetWindowRect(this.consoleProcess.MainWindowHandle, out rect);
			return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Top, rect.Bottom - rect.Top);
		}

		private bool IsConsoleVisible()
		{
			return Win32.IsWindowVisible(this.consoleProcess.MainWindowHandle) && Win32.GetForegroundWindow() == this.consoleProcess.MainWindowHandle;
		}

		private void AnimateConsoleProc(object state)
		{
			int direction = (int)state;

			Rectangle rect = this.GetConsoleRectangle();

			if (direction > 0)
			{
				Win32.SetWindowPos(this.consoleProcess.MainWindowHandle, IntPtr.Zero, this.config.ConsoleHiddenX, this.config.ConsoleHiddenY, this.config.ConsoleWidth, this.config.ConsoleHeight, Win32.SWP_SHOWWINDOW);

				while (rect.X < this.config.ConsoleVisibleX || rect.Y < this.config.ConsoleVisibleY)
				{
					rect.X += this.config.AnimateSpeedX;
					rect.Y += this.config.AnimateSpeedY;

					if (rect.X > this.config.ConsoleVisibleX)
						rect.X = this.config.ConsoleVisibleX;

					if (rect.Y > this.config.ConsoleVisibleY)
						rect.Y = this.config.ConsoleVisibleY;

					Win32.SetWindowPos(this.consoleProcess.MainWindowHandle, IntPtr.Zero, rect.X, rect.Y, this.config.ConsoleWidth, this.config.ConsoleHeight, Win32.SWP_SHOWWINDOW);

					Thread.Sleep(10);
				}

				Win32.SetWindowPos(this.consoleProcess.MainWindowHandle, IntPtr.Zero, this.config.ConsoleVisibleX, this.config.ConsoleVisibleY, this.config.ConsoleWidth, this.config.ConsoleHeight, Win32.SWP_SHOWWINDOW);
				Win32.SetForegroundWindow(this.consoleProcess.MainWindowHandle);
			}
			else
			{
				while (rect.X > this.config.ConsoleHiddenX || rect.Y > this.config.ConsoleHiddenY)
				{
					rect.X -= this.config.AnimateSpeedX;
					rect.Y -= this.config.AnimateSpeedY;

					if (rect.X < this.config.ConsoleHiddenX)
						rect.X = this.config.ConsoleHiddenX;

					if (rect.Y < this.config.ConsoleHiddenY)
						rect.Y = this.config.ConsoleHiddenY;

					Win32.SetWindowPos(this.consoleProcess.MainWindowHandle, IntPtr.Zero, rect.X, rect.Y, this.config.ConsoleWidth, this.config.ConsoleHeight, Win32.SWP_SHOWWINDOW);

					Thread.Sleep(10);
				}

				Win32.ShowWindow(this.consoleProcess.MainWindowHandle, Win32.SW_HIDE);
				Win32.SetForegroundWindow(this.oldForeground);
			}
		}

		private void ToggleConsole()
		{
			if(this.consoleProcess == null || this.consoleProcess.HasExited)
				this.StartConsoleProcess();
												
			if(!this.IsConsoleVisible())
			{
				this.oldForeground = Win32.GetForegroundWindow();

				if (this.config.Animate)
					ThreadPool.QueueUserWorkItem(this.AnimateConsoleProc, 1);
				else
				{
					Win32.ShowWindow(this.consoleProcess.MainWindowHandle, Win32.SW_SHOW);
					Win32.SetWindowPos(this.consoleProcess.MainWindowHandle, IntPtr.Zero, this.config.ConsoleVisibleX, this.config.ConsoleVisibleY, this.config.ConsoleWidth, this.config.ConsoleHeight, Win32.SWP_SHOWWINDOW);
					Win32.SetForegroundWindow(this.consoleProcess.MainWindowHandle);
				}
			}
			else
			{
				if (this.config.Animate)
					ThreadPool.QueueUserWorkItem(this.AnimateConsoleProc, -1);
				else
				{
					Win32.ShowWindow(this.consoleProcess.MainWindowHandle, Win32.SW_HIDE);
					Win32.SetForegroundWindow(this.oldForeground);
				}
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
