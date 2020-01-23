using ICD.Common.Utils.Xml;
using ICD.Connect.Devices;
using ICD.Connect.Protocol.Ports.RelayPort;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Misc.Yepkit.Devices.YkupSwitcher
{
	[KrangSettings("YkupSwitcher", typeof(YkupSwitcherDevice))]
	public sealed class YkupSwitcherDeviceSettings : AbstractDeviceSettings
	{
		public const string POWER_PORT_ELEMENT = "PowerPort";
		public const string SWITCH_PORT_ELEMENT = "SwitchPort";

		/// <summary>
		/// The power port id.
		/// </summary>
		[OriginatorIdSettingsProperty(typeof(IRelayPort))]
		public int? PowerPort { get; set; }

		/// <summary>
		/// The switch port id.
		/// </summary>
		[OriginatorIdSettingsProperty(typeof(IRelayPort))]
		public int? SwitchPort { get; set; }

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(POWER_PORT_ELEMENT, PowerPort == null ? null : IcdXmlConvert.ToString((int)PowerPort));
			writer.WriteElementString(SWITCH_PORT_ELEMENT, SwitchPort == null ? null : IcdXmlConvert.ToString((int)SwitchPort));
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			PowerPort = XmlUtils.TryReadChildElementContentAsInt(xml, POWER_PORT_ELEMENT);
			SwitchPort = XmlUtils.TryReadChildElementContentAsInt(xml, SWITCH_PORT_ELEMENT);
		}
	}
}
