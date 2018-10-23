using ICD.Common.Utils.Xml;
using ICD.Connect.Misc.GlobalCache.Devices;
using ICD.Connect.Protocol.Ports.ComPort;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Misc.GlobalCache.Ports
{
	[KrangSettings("iTachFlexComPort", typeof(GcITachFlexComPort))]
	public sealed class GcITachFlexComPortSettings : AbstractComPortSettings
	{
		private const string PARENT_DEVICE_ELEMENT = "Device";
		private const string PARENT_MODULE_ELEMENT = "Module";
		private const string PARENT_ADDRESS_ELEMENT = "Address";

		#region Properties

		[ControlPortParentSettingsProperty]
		[OriginatorIdSettingsProperty(typeof(GcITachFlexDevice))]
		public int? Device { get; set; }

		public int Module { get; set; }

		public int Address { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(PARENT_DEVICE_ELEMENT, IcdXmlConvert.ToString(Device));
			writer.WriteElementString(PARENT_MODULE_ELEMENT, IcdXmlConvert.ToString(Module));
			writer.WriteElementString(PARENT_ADDRESS_ELEMENT, IcdXmlConvert.ToString(Address));
		}

		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			Device = XmlUtils.TryReadChildElementContentAsInt(xml, PARENT_DEVICE_ELEMENT);
			Module = XmlUtils.TryReadChildElementContentAsInt(xml, PARENT_MODULE_ELEMENT) ?? 1;
			Address = XmlUtils.TryReadChildElementContentAsInt(xml, PARENT_ADDRESS_ELEMENT) ?? 1;
		}

		public override int DependencyCount { get { return Device == null ? 0 : 1; } }

		public override bool HasDeviceDependency(int id)
		{
			return base.HasDeviceDependency(id) || id == Device;
		}

		#endregion
	}
}
