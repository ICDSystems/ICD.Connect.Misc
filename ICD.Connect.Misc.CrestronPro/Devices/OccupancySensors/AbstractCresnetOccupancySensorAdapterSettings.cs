using ICD.Common.Utils;
using ICD.Common.Utils.Xml;
using ICD.Connect.Devices;
using ICD.Connect.Misc.CrestronPro.Devices.CresnetBridge;
using ICD.Connect.Misc.CrestronPro.Utils;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Misc.CrestronPro.Devices.OccupancySensors
{
	public abstract class AbstractCresnetOccupancySensorAdapterSettings : AbstractDeviceSettings, ICresnetDeviceSettings
	{
		[CrestronByteSettingsProperty]
		public byte? CresnetId { get; set; }

		public int? BranchId { get; set; }

		[OriginatorIdSettingsProperty(typeof(ICresnetBridgeAdapter))]
		public int? ParentId { get; set; }

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(CresnetSettingsUtils.CRESNET_ID_ELEMENT, CresnetId == null ? null : StringUtils.ToIpIdString(CresnetId.Value));
			writer.WriteElementString(CresnetSettingsUtils.BRANCH_ID_ELEMENT, BranchId == null ? null : BranchId.Value.ToString());
			writer.WriteElementString(CresnetSettingsUtils.PARENT_ID_ELEMENT, ParentId == null ? null : ParentId.Value.ToString());
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			CresnetId = XmlUtils.TryReadChildElementContentAsByte(xml, CresnetSettingsUtils.CRESNET_ID_ELEMENT);
			BranchId = XmlUtils.TryReadChildElementContentAsInt(xml, CresnetSettingsUtils.BRANCH_ID_ELEMENT);
			ParentId = XmlUtils.TryReadChildElementContentAsInt(xml, CresnetSettingsUtils.PARENT_ID_ELEMENT);
		}
	}
}