using ICD.Common.Utils.Xml;
using ICD.Connect.Devices;

namespace ICD.Connect.Misc.Windows.Devices.WindowsPeripheralDevice
{
	public abstract class AbstractWindowsPeripheralDeviceSettings : AbstractDeviceSettings, IWindowsPeripheralDeviceSettings
	{
		private const string DEVICE_ID_ELEMENT = "DeviceId";
		private const string CONTROL_SYSTEM_ELEMENT = "ControlSystem";

		/// <summary>
		/// Unique identifier for the peripheral device
		/// </summary>
		public string DeviceId { get; set; }

		/// <summary>
		/// The windows control system this device is attached to
		/// </summary>
		public int ControlSystem { get; set; }

		#region XML

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			DeviceId = XmlUtils.TryReadChildElementContentAsString(xml, DEVICE_ID_ELEMENT);
			ControlSystem = XmlUtils.ReadChildElementContentAsInt(xml, CONTROL_SYSTEM_ELEMENT);
		}

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(DEVICE_ID_ELEMENT, DeviceId);
			writer.WriteElementString(CONTROL_SYSTEM_ELEMENT, IcdXmlConvert.ToString(ControlSystem));
		}

		#endregion
	}
}
