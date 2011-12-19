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
			config.Console = document.Root.Element("Console").Value;
			config.WorkingDirectory = document.Root.Element("WorkingDirectory").Value;
			config.Animate = document.Root.Element("Animate").Value.ToUpper() == "TRUE" ? true : false;

			return config;
		}
	}
}
