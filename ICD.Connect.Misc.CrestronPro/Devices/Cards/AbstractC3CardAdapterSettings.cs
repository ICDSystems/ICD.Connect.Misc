using ICD.Common.Utils;
using ICD.Common.Utils.Xml;
using ICD.Connect.Devices;
using ICD.Connect.Misc.CrestronPro.Devices.CardFrames;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Misc.CrestronPro.Devices.Cards
{
	public abstract class AbstractC3CardAdapterSettings : AbstractDeviceSettings
	{
		private const string IPID_ELEMENT = "IPID";
		private const string CARD_FRAME_ELEMENT = "CardFrame";

		[IpIdSettingsProperty]
		public byte? Ipid { get; set; }

		[OriginatorIdSettingsProperty(typeof(ICardFrameDevice))]
		public int? CardFrame { get; set; }

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(IPID_ELEMENT, Ipid == null ? null : StringUtils.ToIpIdString(Ipid.Value));
			writer.WriteElementString(CARD_FRAME_ELEMENT, IcdXmlConvert.ToString(CardFrame));
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			Ipid = XmlUtils.TryReadChildElementContentAsByte(xml, IPID_ELEMENT) ?? 0;
			CardFrame = XmlUtils.TryReadChildElementContentAsInt(xml, CARD_FRAME_ELEMENT);
		}
	}
}
