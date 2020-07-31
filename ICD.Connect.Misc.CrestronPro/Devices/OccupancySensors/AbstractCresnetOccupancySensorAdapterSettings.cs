using ICD.Common.Utils.Xml;
using ICD.Connect.Devices;
using ICD.Connect.Misc.CrestronPro.Cresnet;

namespace ICD.Connect.Misc.CrestronPro.Devices.OccupancySensors
{
	public abstract class AbstractCresnetOccupancySensorAdapterSettings : AbstractDeviceSettings, ICresnetDeviceSettings
	{
		private readonly CresnetDeviceSettings m_CresnetDeviceSettings;

		public CresnetDeviceSettings CresnetDeviceSettings { get { return m_CresnetDeviceSettings; } }

		protected AbstractCresnetOccupancySensorAdapterSettings()
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