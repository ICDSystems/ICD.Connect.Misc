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
		public IrPortAdapterSettings()
		{
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

		#endregion
	}
}
