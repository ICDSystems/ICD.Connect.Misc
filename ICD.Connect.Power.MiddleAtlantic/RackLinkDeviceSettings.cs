using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Xml;
using ICD.Connect.Devices;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Power.MiddleAtlantic
{
	public sealed class RackLinkDeviceSettings : AbstractDeviceSettings
	{
		private const string FACTORY_NAME = "MiddleAtlanticRackLink";

		private const string PORT_ELEMENT = "Port";

		[SettingsProperty(SettingsProperty.ePropertyType.PortId)]
		public int? Port { get; set; }

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(RackLinkDevice); } }

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(PORT_ELEMENT, IcdXmlConvert.ToString(Port));
		}

		/// <summary>
		/// Loads the settings from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI, XmlFactoryMethod(FACTORY_NAME)]
		public static RackLinkDeviceSettings FromXml(string xml)
		{
			RackLinkDeviceSettings output = new RackLinkDeviceSettings
			{
				Port = XmlUtils.TryReadChildElementContentAsInt(xml, PORT_ELEMENT)
			};

			ParseXml(output, xml);
			return output;
		}
	}
}
