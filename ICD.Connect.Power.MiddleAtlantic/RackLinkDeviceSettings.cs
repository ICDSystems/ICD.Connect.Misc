using ICD.Common.Attributes.Properties;
using ICD.Common.Properties;
using ICD.Common.Utils.Xml;
using ICD.Connect.Settings;
using ICD.Connect.Settings.Attributes.Factories;
using ICD.Connect.Settings.Core;

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
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(PORT_ELEMENT, IcdXmlConvert.ToString(Port));
		}

		/// <summary>
		/// Creates a new originator instance from the settings.
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		public override IOriginator ToOriginator(IDeviceFactory factory)
		{
			RackLinkDevice output = new RackLinkDevice();
			output.ApplySettings(this, factory);
			return output;
		}

		/// <summary>
		/// Loads the settings from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI, XmlDeviceSettingsFactoryMethod(FACTORY_NAME)]
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
