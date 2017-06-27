using System;
using ICD.Common.Properties;
using ICD.Connect.Settings;
using ICD.Connect.Settings.Attributes.Factories;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Misc.CrestronPro.Devices.Cards
{
	public sealed class C3Ry8AdapterSettings : AbstractC3RyAdapterSettings
	{
		private const string FACTORY_NAME = "C3Ry8";

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(C3Ry8Adapter); } }

		/// <summary>
		/// Loads the settings from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI, XmlDeviceSettingsFactoryMethod(FACTORY_NAME)]
		public static C3Ry8AdapterSettings FromXml(string xml)
		{
			C3Ry8AdapterSettings output = new C3Ry8AdapterSettings();
			ParseXml(output, xml);
			return output;
		}
	}
}
