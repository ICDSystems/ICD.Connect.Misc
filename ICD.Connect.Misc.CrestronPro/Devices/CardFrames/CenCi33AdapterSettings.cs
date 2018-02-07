using System;
using ICD.Common.Properties;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.CrestronPro.Devices.CardFrames
{
	public sealed class CenCi33AdapterSettings : AbstractCardFrameDeviceSettings
	{
		private const string FACTORY_NAME = "CenCi33";

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(CenCi33Adapter); } }

		/// <summary>
		/// Loads the settings from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI, XmlFactoryMethod(FACTORY_NAME)]
		public static CenCi33AdapterSettings FromXml(string xml)
		{
			CenCi33AdapterSettings output = new CenCi33AdapterSettings();
			output.ParseXml(xml);
			return output;
		}
	}
}
