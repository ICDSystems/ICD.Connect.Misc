using ICD.Common.Utils.Xml;
using ICD.Connect.Misc.CrestronPro.Cresnet;
using ICD.Connect.Partitioning.Devices;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.CrestronPro.Devices.Partitioning
{
	[KrangSettings("GlsPartCn", typeof(GlsPartCnAdapter))]
	public sealed class GlsPartCnAdapterSettings : AbstractPartitionDeviceSettings, ICresnetDeviceSettings
	{
		private const string SENSITIVITY_ELEMENT = "Sensitivity";

		private readonly CresnetSettings m_CresnetSettings;

		public CresnetSettings CresnetSettings { get { return m_CresnetSettings; } }

		public ushort? Sensitivity { get; set; }

		public GlsPartCnAdapterSettings()
		{
			m_CresnetSettings = new CresnetSettings();
		}

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			m_CresnetSettings.WriteElements(writer);

			writer.WriteElementString(SENSITIVITY_ELEMENT, IcdXmlConvert.ToString(Sensitivity));
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			m_CresnetSettings.ParseXml(xml);

			Sensitivity = XmlUtils.TryReadChildElementContentAsUShort(xml, SENSITIVITY_ELEMENT);
		}
	}
}
