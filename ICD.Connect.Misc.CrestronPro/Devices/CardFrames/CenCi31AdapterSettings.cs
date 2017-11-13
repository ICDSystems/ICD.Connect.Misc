using System;
using ICD.Common.Properties;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.CrestronPro.Devices.CardFrames
{
	public sealed class CenCi31AdapterSettings : AbstractCardFrameDeviceSettings
	{
		private const string FACTORY_NAME = "CenCi31";

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(CenCi31Adapter); } }

		/// <summary>
		/// Loads the settings from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI, XmlFactoryMethod(FACTORY_NAME)]
		public static CenCi31AdapterSettings FromXml(string xml)
		{
			CenCi31AdapterSettings output = new CenCi31AdapterSettings();
			ParseXml(output, xml);
			return output;
		}
	}
}
