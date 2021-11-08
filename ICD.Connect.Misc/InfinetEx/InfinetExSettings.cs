using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Xml;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Misc.InfinetEx
{
	public sealed class InfinetExSettings
	{
		private const string INFINET_ELEMENT = "InfinetEx";
		private const string RFID_ELEMENT = "RFID";
		private const string PARENT_ID_ELEMENT = "ParentID";

		[CrestronByteSettingsProperty]
		public byte? RfId { get; set; }


		public int? ParentId { get; set; }

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		public void WriteElements(IcdXmlTextWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			writer.WriteStartElement(INFINET_ELEMENT);
			{
				writer.WriteElementString(RFID_ELEMENT, RfId == null ? null : StringUtils.ToIpIdString(RfId.Value));
				writer.WriteElementString(PARENT_ID_ELEMENT, ParentId == null ? null : ParentId.Value.ToString());
			}
			writer.WriteEndElement();
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public void ParseXml(string xml)
		{
			string innerXml = XmlUtils.GetChildElementAsString(xml, INFINET_ELEMENT);
			ParseInnerXml(innerXml);
		}

		private void ParseInnerXml(string xml)
		{
			RfId = XmlUtils.TryReadChildElementContentAsByte(xml, RFID_ELEMENT);
			ParentId = XmlUtils.TryReadChildElementContentAsInt(xml, PARENT_ID_ELEMENT);
		}
	}
}