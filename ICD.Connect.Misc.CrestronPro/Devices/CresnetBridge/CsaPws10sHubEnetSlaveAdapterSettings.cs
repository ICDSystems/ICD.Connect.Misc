using ICD.Common.Utils.Xml;
using ICD.Connect.Devices;
using ICD.Connect.Misc.CrestronPro.Utils;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Misc.CrestronPro.Devices.CresnetBridge
{
	[KrangSettings("CsaPws10sHubEnetSlave", typeof(CsaPws10sHubEnetSlaveAdapter))]
	public sealed class CsaPws10sHubEnetSlaveAdapterSettings : AbstractDeviceSettings, ICsaPws10sHubEnetSettings, ICresnetDeviceSettings
	{
		[CrestronByteSettingsProperty]
		public byte? CresnetId { get; set; }

		public int? ParentId { get; set; }

		public int? BranchId { get; set; }

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			CresnetSettingsUtils.WritePropertiesToXml(this, writer);
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			CresnetSettingsUtils.ReadPropertiesFromXml(this, xml);
		}
	}
}