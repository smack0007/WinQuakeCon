using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace WinQuakeCon
{
	public class Config
	{
		public string Console
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

			XDocument document = XDocument.Load(fileName);
			config.Console = document.Root.Element("Console").Value;

			return config;
		}
	}
}
