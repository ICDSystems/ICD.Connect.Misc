using ICD.Common.Utils;
using ICD.Common.Utils.Xml;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Misc.CrestronPro.Devices.Keypads.C2nCbdBase
{
	public abstract class AbstractC2nCbdBaseAdapterSettings : AbstractInetCbdexAdapterSettings, IC2nCbdBaseAdapterSettings
	{
		private const string CRESNET_ID_ELEMENT = "CresnetId";

		[IpIdSettingsProperty]
		public byte? CresnetId { get; set; }

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(CRESNET_ID_ELEMENT, CresnetId == null ? null : StringUtils.ToIpIdString((byte)CresnetId));
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			CresnetId = XmlUtils.TryReadChildElementContentAsByte(xml, CRESNET_ID_ELEMENT);
		}
	}
}