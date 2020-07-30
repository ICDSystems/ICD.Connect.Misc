using ICD.Common.Utils.Xml;
using ICD.Connect.Misc.CrestronPro.Cresnet;
using ICD.Connect.Misc.CrestronPro.Devices.CresnetBridge;
using ICD.Connect.Misc.CrestronPro.Utils;
using ICD.Connect.Partitioning.Devices;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Misc.CrestronPro.Devices.Partitioning
{
	[KrangSettings("GlsPartCn", typeof(GlsPartCnAdapter))]
	public sealed class GlsPartCnAdapterSettings : AbstractPartitionDeviceSettings, ICresnetDeviceSettings
	{
		private const string SENSITIVITY_ELEMENT = "Sensitivity";

		[CrestronByteSettingsProperty]
		public byte? CresnetId { get; set; }

		public int? BranchId { get; set; }

		[OriginatorIdSettingsProperty(typeof(ICresnetBridgeAdapter))]
		public int? ParentId { get; set; }

		public ushort? Sensitivity { get; set; }

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			CresnetSettingsUtils.WritePropertiesToXml(this, writer);

			writer.WriteElementString(SENSITIVITY_ELEMENT, IcdXmlConvert.ToString(Sensitivity));
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			CresnetSettingsUtils.ReadPropertiesFromXml(this, xml);

			Sensitivity = XmlUtils.TryReadChildElementContentAsUShort(xml, SENSITIVITY_ELEMENT);
		}
	}
}
