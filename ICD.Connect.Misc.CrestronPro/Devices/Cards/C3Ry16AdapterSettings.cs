using System;
using ICD.Common.Properties;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.CrestronPro.Devices.Cards
{
	public sealed class C3Ry16AdapterSettings : AbstractC3RyAdapterSettings
	{
		private const string FACTORY_NAME = "C3Ry16";

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(C3Ry16Adapter); } }

		/// <summary>
		/// Loads the settings from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI, XmlFactoryMethod(FACTORY_NAME)]
		public static C3Ry16AdapterSettings FromXml(string xml)
		{
			C3Ry16AdapterSettings output = new C3Ry16AdapterSettings();
			output.ParseXml(xml);
			return output;
		}
	}
}
