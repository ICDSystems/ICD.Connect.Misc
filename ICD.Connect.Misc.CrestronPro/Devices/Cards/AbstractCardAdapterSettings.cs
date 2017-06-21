using ICD.Common.Attributes.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Xml;
using ICD.Connect.Settings;

namespace ICD.Connect.Misc.CrestronPro.Devices.Cards
{
	public abstract class AbstractCardAdapterSettings : AbstractDeviceSettings
	{
		private const string IPID_ELEMENT = "IPID";
		private const string CARD_FRAME_ELEMENT = "CardFrame";

		[SettingsProperty(SettingsProperty.ePropertyType.Ipid)]
		public byte? Ipid { get; set; }

		[SettingsProperty(SettingsProperty.ePropertyType.DeviceId)]
		public int? CardFrame { get; set; }

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			if (Ipid != null)
				writer.WriteElementString(IPID_ELEMENT, StringUtils.ToIpIdString((byte)Ipid));

			if (CardFrame != null)
				writer.WriteElementString(CARD_FRAME_ELEMENT, IcdXmlConvert.ToString((int)CardFrame));
		}

		protected static void ParseXml(AbstractCardAdapterSettings instance, string xml)
		{
			instance.Ipid = XmlUtils.TryReadChildElementContentAsByte(xml, IPID_ELEMENT);
			instance.CardFrame = XmlUtils.TryReadChildElementContentAsInt(xml, CARD_FRAME_ELEMENT);

			AbstractDeviceSettings.ParseXml(instance, xml);
		}
	}
}
