using ICD.Common.Utils.Xml;
using ICD.Connect.Misc.ControlSystems;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Misc.Windows.Devices.ControlSystems
{
	[KrangSettings("WindowsControlSystem", typeof(WindowsControlSystem))]
	public sealed class WindowsControlSystemSettings : AbstractControlSystemDeviceSettings
	{
		private const string PERIPHERAL_WHITELIST_ELEMENT = "PeripheralWhitelistConfig";

		private const string PERIPHERAL_WHITELIST_DEFAULT = "PeripheralWhitelist.xml";

		public const string PERIPHERALS_WHITELIST_FOLDER = "Peripherals";

		private string m_PeripheralWhitelist;

		[PathSettingsProperty(PERIPHERALS_WHITELIST_FOLDER, ".xml")]
		public string PeripheralWhitelist
		{
			get
			{
				if (string.IsNullOrEmpty(m_PeripheralWhitelist))
					return PERIPHERAL_WHITELIST_DEFAULT;
				return m_PeripheralWhitelist;
			}
			set { m_PeripheralWhitelist = value; }
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			PeripheralWhitelist =
				XmlUtils.TryReadChildElementContentAsString(xml, PERIPHERAL_WHITELIST_ELEMENT);
		}

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(PERIPHERAL_WHITELIST_ELEMENT, PeripheralWhitelist);
		}
	}
}
