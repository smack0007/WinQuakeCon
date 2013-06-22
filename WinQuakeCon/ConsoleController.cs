using System;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

using Timer = System.Threading.Timer;

namespace WinQuakeCon
{
	/// <summary>
	/// Controls the console.
	/// </summary>
	public class ConsoleController : Form
	{
		Config config;

		NotifyIcon trayIcon;

		Process consoleProcess;
		
		IntPtr oldForeground;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="config"></param>
		public ConsoleController(Config config)
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

		/// <summary>
		/// Activates the global hotkey.
		/// </summary>
		public bool Initialize()
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

            if (!Win32.RegisterHotKey(this.Handle, 0, fsModifiers, (uint)this.config.HotKeyCode))
            {
                MessageBox.Show("Failed to register hotkey.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
		}
		
		/// <summary>
		/// Deactivates the global hotkey and kills the console process if it is still running.
		/// </summary>
		public void Shutdown()
		{
			Win32.UnregisterHotKey(this.Handle, 0);

			if(this.consoleProcess != null && !this.consoleProcess.HasExited)
				this.consoleProcess.Kill();
		}

		/// <summary>
		/// Called when the "Toggle Console" tray icon menu item is selected.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TrayIcon_ToggleConsole(object sender, EventArgs e)
		{
			this.ToggleConsole();
		}

		/// <summary>
		/// Called when the "Reload Config" tray icon menu item is selected.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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

		/// <summary>
		/// Called when the "Exit" tray icon menu item is selected.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TrayIcon_Exit(object sender, EventArgs e)
		{
			Application.Exit();
		}

		/// <summary>
		/// Gets the screen selected by the config option "ConsoleScreen".
		/// </summary>
		/// <returns></returns>
		private Screen GetScreen()
		{
			if (this.config.ConsoleScreen >= Screen.AllScreens.Length)
			{
				MessageBox.Show(this, "Invalid screen index in ConsoleScreen.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Application.Exit();
				return null;
			}

			return Screen.AllScreens[this.config.ConsoleScreen];
		}

		/// <summary>
		/// Sets the position of the console relative to the display screen.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="showConsole"></param>
		private void SetConsolePosition(int x, int y, bool showConsole)
		{
			Screen screen = this.GetScreen();
			Win32.SetWindowPos(this.consoleProcess.MainWindowHandle, IntPtr.Zero, screen.WorkingArea.X + x, screen.WorkingArea.Y + y, this.config.ConsoleWidth, this.config.ConsoleHeight, showConsole ? Win32.SWP_SHOWWINDOW : Win32.SWP_HIDEWINDOW);
		}

		/// <summary>
		/// Starts an instance of the console process.
		/// </summary>
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

			this.SetConsolePosition(this.config.ConsoleHiddenX, this.config.ConsoleHiddenY, false);
		}

		/// <summary>
		/// Gets the Rectangle of the console window.
		/// </summary>
		/// <returns></returns>
		private Rectangle GetConsoleRectangle()
		{
			Screen screen = this.GetScreen();

			Win32.RECT rect = new Win32.RECT();
			Win32.GetWindowRect(this.consoleProcess.MainWindowHandle, out rect);

			// Subtract screen position as SetConsolePosition is relative to the selected screen.
			return new Rectangle(rect.Left - screen.WorkingArea.X, rect.Top - screen.WorkingArea.Y, rect.Right - rect.Top, rect.Bottom - rect.Top);
		}

		/// <summary>
		/// Indicates whether or not the console window is in visible or should be treated as if it is not.
		/// </summary>
		/// <returns></returns>
		private bool IsConsoleVisible()
		{
			return Win32.IsWindowVisible(this.consoleProcess.MainWindowHandle) && Win32.GetForegroundWindow() == this.consoleProcess.MainWindowHandle;
		}

		/// <summary>
		/// Animation procedure.
		/// </summary>
		/// <param name="state"></param>
		private void AnimateConsoleProc(object state)
		{
			int direction = (int)state;
						
			if (direction > 0)
			{
				this.SetConsolePosition(this.config.ConsoleHiddenX, this.config.ConsoleHiddenY, true);
				Win32.SetForegroundWindow(this.consoleProcess.MainWindowHandle);
				
				Rectangle rect = this.GetConsoleRectangle();

				while (rect.X < this.config.ConsoleVisibleX || rect.Y < this.config.ConsoleVisibleY)
				{
					rect.X += this.config.AnimateSpeedX;
					rect.Y += this.config.AnimateSpeedY;

					if (rect.X > this.config.ConsoleVisibleX)
						rect.X = this.config.ConsoleVisibleX;

					if (rect.Y > this.config.ConsoleVisibleY)
						rect.Y = this.config.ConsoleVisibleY;

					this.SetConsolePosition(rect.X, rect.Y, true);

					Thread.Sleep(10);
				}

				this.SetConsolePosition(this.config.ConsoleVisibleX, this.config.ConsoleVisibleY, true);
			}
			else
			{
				Rectangle rect = this.GetConsoleRectangle();

				while (rect.X > this.config.ConsoleHiddenX || rect.Y > this.config.ConsoleHiddenY)
				{
					rect.X -= this.config.AnimateSpeedX;
					rect.Y -= this.config.AnimateSpeedY;

					if (rect.X < this.config.ConsoleHiddenX)
						rect.X = this.config.ConsoleHiddenX;

					if (rect.Y < this.config.ConsoleHiddenY)
						rect.Y = this.config.ConsoleHiddenY;

					this.SetConsolePosition(rect.X, rect.Y, true);

					Thread.Sleep(10);
				}

				Win32.ShowWindow(this.consoleProcess.MainWindowHandle, Win32.SW_HIDE);
				Win32.SetForegroundWindow(this.oldForeground);
			}
		}

		/// <summary>
		/// Toggles the console window in or out of focus.
		/// </summary>
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
					this.SetConsolePosition(this.config.ConsoleVisibleX, this.config.ConsoleVisibleY, true);
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

		/// <summary>
		/// Listens for the hotkey.
		/// </summary>
		/// <param name="m"></param>
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
