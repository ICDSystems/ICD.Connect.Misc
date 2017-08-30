using System;
using ICD.Common.Properties;
using ICD.Connect.Settings.Attributes.Factories;

namespace ICD.Connect.Misc.CrestronPro.Devices.Cards
{
	public sealed class C3Ir8AdapterSettings : AbstractC3CardAdapterSettings
	{
		private const string FACTORY_NAME = "C3Ir8";

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(C3Ir8Adapter); } }

		/// <summary>
		/// Loads the settings from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI, XmlDeviceSettingsFactoryMethod(FACTORY_NAME)]
		public static C3Ir8AdapterSettings FromXml(string xml)
		{
			C3Ir8AdapterSettings output = new C3Ir8AdapterSettings();
			ParseXml(output, xml);
			return output;
		}
	}
}
