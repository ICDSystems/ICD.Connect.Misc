using ICD.Common.Utils.Xml;
using ICD.Connect.Devices;
using ICD.Connect.Misc.CrestronPro.Cresnet;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.CrestronPro.Devices.CresnetBridge
{
	[KrangSettings("CsaPws10sHubEnetSlave", typeof(CsaPws10sHubEnetSlaveAdapter))]
	public sealed class CsaPws10sHubEnetSlaveAdapterSettings : AbstractDeviceSettings, ICsaPws10sHubEnetSettings, ICresnetDeviceSettings
	{
		private readonly CresnetDeviceSettings m_CresnetDeviceSettings;

		public CresnetDeviceSettings CresnetDeviceSettings { get { return m_CresnetDeviceSettings; } }

		public CsaPws10sHubEnetSlaveAdapterSettings()
		{
			m_CresnetDeviceSettings = new CresnetDeviceSettings();
		}

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			m_CresnetDeviceSettings.WriteElements(writer);
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			m_CresnetDeviceSettings.ParseXml(xml);
		}
	}
}