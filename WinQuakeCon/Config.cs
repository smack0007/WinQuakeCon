using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace WinQuakeCon
{
	public class Config
	{
		public int HotKeyCode
		{
			get;
			set;
		}

		public bool HotKeyAlt
		{
			get;
			set;
		}

		public bool HotKeyCtrl
		{
			get;
			set;
		}

		public bool HotKeyShift
		{
			get;
			set;
		}

		public bool HotKeyWin
		{
			get;
			set;
		}

		public string Console
		{
			get;
			set;
		}

		public int ConsoleWidth
		{
			get;
			set;
		}

		public int ConsoleHeight
		{
			get;
			set;
		}

		public int ConsoleHiddenX
		{
			get;
			set;
		}

		public int ConsoleHiddenY
		{
			get;
			set;
		}

		public int ConsoleVisibleX
		{
			get;
			set;
		}

		public int ConsoleVisibleY
		{
			get;
			set;
		}

		public string WorkingDirectory
		{
			get;
			set;
		}

		public bool Animate
		{
			get;
			set;
		}

		public int AnimateSpeedX
		{
			get;
			set;
		}

		public int AnimateSpeedY
		{
			get;
			set;
		}

		public Config()
		{
		}

		public static Config Load(string fileName)
		{
			Config config = new Config();

			// TODO: Validate config file.

			XDocument document = XDocument.Load(fileName);
			config.HotKeyCode = int.Parse(document.Root.Element("HotKeyCode").Value);
			config.HotKeyAlt = document.Root.Element("HotKeyAlt").Value.ToUpper() == "TRUE" ? true : false;
			config.HotKeyCtrl = document.Root.Element("HotKeyCtrl").Value.ToUpper() == "TRUE" ? true : false;
			config.HotKeyShift = document.Root.Element("HotKeyShift").Value.ToUpper() == "TRUE" ? true : false;
			config.HotKeyWin = document.Root.Element("HotKeyWin").Value.ToUpper() == "TRUE" ? true : false;
			config.Console = document.Root.Element("Console").Value;
			config.ConsoleWidth = int.Parse(document.Root.Element("ConsoleWidth").Value);
			config.ConsoleHeight = int.Parse(document.Root.Element("ConsoleHeight").Value);
			config.ConsoleHiddenX = int.Parse(document.Root.Element("ConsoleHiddenX").Value);
			config.ConsoleHiddenY = int.Parse(document.Root.Element("ConsoleHiddenY").Value);
			config.ConsoleVisibleX = int.Parse(document.Root.Element("ConsoleVisibleX").Value);
			config.ConsoleVisibleY = int.Parse(document.Root.Element("ConsoleVisibleY").Value);
			config.WorkingDirectory = document.Root.Element("WorkingDirectory").Value;
			config.Animate = document.Root.Element("Animate").Value.ToUpper() == "TRUE" ? true : false;
			config.AnimateSpeedX = int.Parse(document.Root.Element("AnimateSpeedX").Value);
			config.AnimateSpeedY = int.Parse(document.Root.Element("AnimateSpeedY").Value);

			if (config.AnimateSpeedX <= 0)
				config.AnimateSpeedX = 1;

			if (config.AnimateSpeedY <= 0)
				config.AnimateSpeedY = 1;

			return config;
		}
	}
}
