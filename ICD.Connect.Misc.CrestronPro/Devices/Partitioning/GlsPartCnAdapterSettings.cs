using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Xml;
using ICD.Connect.Misc.CrestronPro.Utils;
using ICD.Connect.Partitioning.Devices;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Misc.CrestronPro.Devices.Partitioning
{
	[KrangSettings(FACTORY_NAME)]
	public sealed class GlsPartCnAdapterSettings : AbstractPartitionDeviceSettings, ICresnetDeviceSettings
	{
		private const string FACTORY_NAME = "GlsPartCn";
		
		private const string SENSITIVITY_ELEMENT = "Sensitivity";

		[CrestronByteSettingsProperty]
		public byte? CresnetId { get; set; }
		public int? BranchId { get; set; }
		public int? ParentId { get; set; }

		public ushort Sensitivity { get; set; }

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(GlsPartCnAdapter); } }

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
			writer.WriteElementString(SENSITIVITY_ELEMENT, IcdXmlConvert.ToString(Sensitivity));
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
			Sensitivity = XmlUtils.TryReadChildElementContentAsUShort(xml, SENSITIVITY_ELEMENT) ?? 1;
		}
	}
}
