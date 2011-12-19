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
			config.WorkingDirectory = document.Root.Element("WorkingDirectory").Value;
			config.Animate = document.Root.Element("Animate").Value.ToUpper() == "TRUE" ? true : false;

			return config;
		}
	}
}
