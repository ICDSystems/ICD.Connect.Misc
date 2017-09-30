using System;
using ICD.Common.Properties;
using ICD.Connect.Devices;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.GlobalCache
{
    public sealed class GlobalCacheFlexDeviceSettings : AbstractDeviceSettings
	{
		private const string FACTORY_NAME = "GlobalCacheFlex";

		public override string FactoryName { get { return FACTORY_NAME; } }

		public override Type OriginatorType { get { return typeof(GlobalCacheFlexDevice); } }

		/// <summary>
		/// Loads the settings from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI, XmlFactoryMethod(FACTORY_NAME)]
		public static GlobalCacheFlexDeviceSettings FromXml(string xml)
		{
			GlobalCacheFlexDeviceSettings output = new GlobalCacheFlexDeviceSettings();
			ParseXml(output, xml);
			return output;
		}
	}
}
