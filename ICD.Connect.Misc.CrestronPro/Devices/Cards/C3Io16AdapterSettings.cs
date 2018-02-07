﻿using System;
using ICD.Common.Properties;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.CrestronPro.Devices.Cards
{
	public sealed class C3Io16AdapterSettings : AbstractC3CardAdapterSettings
	{
		private const string FACTORY_NAME = "C3Io16";

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(C3Io16Adapter); } }

		/// <summary>
		/// Loads the settings from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI, XmlFactoryMethod(FACTORY_NAME)]
		public static C3Io16AdapterSettings FromXml(string xml)
		{
			C3Io16AdapterSettings output = new C3Io16AdapterSettings();
			output.ParseXml(xml);
			return output;
		}
	}
}
