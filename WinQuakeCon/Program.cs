using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace WinQuakeCon
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Config config = Config.Load("Config.xml");

			if (config == null)
			{
				MessageBox.Show("Failed to load Config.xml", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			ConsoleController mainForm = new ConsoleController(config);
			mainForm.CreateControl();
			mainForm.Initialize();
			Application.Run();
			mainForm.Shutdown();
		}
	}
}
