using ICD.Common.Utils.Xml;
using ICD.Connect.Misc.CrestronPro.Devices;
using ICD.Connect.Protocol.Ports.ComPort;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Misc.CrestronPro.Ports.ComPort
{
	/// <summary>
	/// Settings for the ComPortAdapter.
	/// </summary>
	[KrangSettings("ComPort", typeof(ComPortAdapter))]
	public sealed class ComPortAdapterSettings : AbstractComPortSettings
	{
		private const string PARENT_DEVICE_ELEMENT = "Device";
		private const string ADDRESS_ELEMENT = "Address";

		#region Properties

		[ControlPortParentSettingsProperty]
		[OriginatorIdSettingsProperty(typeof(IPortParent))]
		public int? Device { get; set; }

		public int Address { get; set; }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public ComPortAdapterSettings()
		{
			Address = 1;
		}

		#region Methods

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(PARENT_DEVICE_ELEMENT, IcdXmlConvert.ToString(Device));
			writer.WriteElementString(ADDRESS_ELEMENT, IcdXmlConvert.ToString(Address));
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
