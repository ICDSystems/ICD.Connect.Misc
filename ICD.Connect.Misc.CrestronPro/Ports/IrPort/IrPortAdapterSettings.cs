using ICD.Common.Utils.Xml;
using ICD.Connect.Misc.CrestronPro.Devices;
using ICD.Connect.Protocol.Ports.IrPort;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Misc.CrestronPro.Ports.IrPort
{
	/// <summary>
	/// Settings for the IrPortAdapter.
	/// </summary>
	[KrangSettings("IrPort", typeof(IrPortAdapter))]
	public sealed class IrPortAdapterSettings : AbstractIrPortSettings
	{
		private const ushort DEFAULT_PULSE_TIME = 100;
		private const ushort DEFAULT_BETWEEN_TIME = 750;

		private const string PARENT_DEVICE_ELEMENT = "Device";
		private const string ADDRESS_ELEMENT = "Address";
		private const string DRIVER_ELEMENT = "Driver";
		private const string PULSETIME_ELEMENT = "PulseTime";
		private const string BETWEENTIME_ELEMENT = "BetweenTime";

		#region Properties

		[ControlPortParentSettingsProperty]
		[OriginatorIdSettingsProperty(typeof(IPortParent))]
		public int? Device { get; set; }

		public int Address { get; set; }

		[PathSettingsProperty("IRDrivers", ".ir")]
		public string Driver { get; set; }

		public ushort PulseTime { get; set; }

		public ushort BetweenTime { get; set; }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public IrPortAdapterSettings()
		{
			BetweenTime = DEFAULT_BETWEEN_TIME;
			PulseTime = DEFAULT_PULSE_TIME;
			Address = 1;
		}

		#region Method

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(PARENT_DEVICE_ELEMENT, IcdXmlConvert.ToString(Device));
			writer.WriteElementString(ADDRESS_ELEMENT, IcdXmlConvert.ToString(Address));
			writer.WriteElementString(DRIVER_ELEMENT, Driver);
			writer.WriteElementString(PULSETIME_ELEMENT, IcdXmlConvert.ToString(PulseTime));
			writer.WriteElementString(BETWEENTIME_ELEMENT, IcdXmlConvert.ToString(BetweenTime));
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			Device = XmlUtils.TryReadChildElementContentAsInt(xml, PARENT_DEVICE_ELEMENT);
			Address = XmlUtils.TryReadChildElementContentAsInt(xml, ADDRESS_ELEMENT) ?? 1;
			Driver = XmlUtils.TryReadChildElementContentAsString(xml, DRIVER_ELEMENT);
			PulseTime = (ushort?)XmlUtils.TryReadChildElementContentAsInt(xml, PULSETIME_ELEMENT) ?? 0;
			BetweenTime = (ushort?)XmlUtils.TryReadChildElementContentAsInt(xml, BETWEENTIME_ELEMENT) ?? 0;
		}

		#endregion
	}
}
