using ICD.Common.Utils.Xml;
using ICD.Connect.Misc.CrestronPro.Devices;
using ICD.Connect.Protocol.Ports.IrPort;
using ICD.Connect.Protocol.Settings;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Misc.CrestronPro.Ports.IrPort
{
	/// <summary>
	/// Settings for the IrPortAdapter.
	/// </summary>
	[KrangSettings("IrPort", typeof(IrPortAdapter))]
	public sealed class IrPortAdapterSettings : AbstractIrPortSettings, IIrDriverSettings
	{
		private const string PARENT_DEVICE_ELEMENT = "Device";
		private const string ADDRESS_ELEMENT = "Address";

		private int m_Address = 1;
		private ushort m_PulseTime = IrDriverSettingsParsing.DEFAULT_PULSE_TIME;
		private ushort m_BetweenTime = IrDriverSettingsParsing.DEFAULT_BETWEEN_TIME;

		#region Properties

		[OriginatorIdSettingsProperty(typeof(IPortParent))]
		public int? Device { get; set; }

		public int Address { get { return m_Address; } set { m_Address = value; } }

		/// <summary>
		/// Gets/sets the configurable path to the IR driver.
		/// </summary>
		[PathSettingsProperty("IRDrivers", ".ir")]
		public string IrDriverPath { get; set; }

		/// <summary>
		/// Gets/sets the configurable pulse time for the IR driver.
		/// </summary>
		public ushort IrPulseTime { get { return m_PulseTime; } set { m_PulseTime = value; } }

		/// <summary>
		/// Gets/sets the configurable between time for the IR driver.
		/// </summary>
		public ushort IrBetweenTime { get { return m_BetweenTime; } set { m_BetweenTime = value; } }

		#endregion

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
			
			IrDriverSettingsParsing.WriteElements(writer, this);
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

			IrDriverSettingsParsing.ParseXml(xml, this);
		}

		/// <summary>
		/// Returns true if the settings depend on a device with the given ID.
		/// For example, to instantiate an IR Port from settings, the device the physical port
		/// belongs to will need to be instantiated first.
		/// </summary>
		/// <returns></returns>
		public override bool HasDeviceDependency(int id)
		{
			return Device != null && Device == id;
		}

		/// <summary>
		/// Returns the count from the collection of ids that the settings depends on.
		/// </summary>
		public override int DependencyCount { get { return Device != null ? 1 : 0; } }

		#endregion
	}
}
