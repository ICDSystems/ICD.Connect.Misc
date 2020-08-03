using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Xml;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Misc.CrestronPro.Cresnet
{
	public sealed class CresnetSettings
	{
		private const string CRESNET_ELEMENT = "Cresnet";
		private const string CRESNET_ID_ELEMENT = "CresnetID";
		private const string BRANCH_ID_ELEMENT = "BranchID";
		private const string PARENT_ID_ELEMENT = "ParentID";

		[CrestronByteSettingsProperty]
		public byte? CresnetId { get; set; }
		public int? ParentId { get; set; }
		public int? BranchId { get; set; }

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		public void WriteElements(IcdXmlTextWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			writer.WriteStartElement(CRESNET_ELEMENT);
			{
				writer.WriteElementString(CRESNET_ID_ELEMENT, CresnetId == null ? null : StringUtils.ToIpIdString(CresnetId.Value));
				writer.WriteElementString(BRANCH_ID_ELEMENT, BranchId == null ? null : BranchId.Value.ToString());
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
			// Try to read nested xml first, support non-nested settings for backwards compatibility second.
			string innerXml;
			ParseInnerXml(XmlUtils.TryGetChildElementAsString(xml, CRESNET_ELEMENT, out innerXml) ? innerXml : xml);
		}

		private void ParseInnerXml(string xml)
		{
			CresnetId = XmlUtils.TryReadChildElementContentAsByte(xml, CRESNET_ID_ELEMENT);
			BranchId = XmlUtils.TryReadChildElementContentAsInt(xml, BRANCH_ID_ELEMENT);
			ParentId = XmlUtils.TryReadChildElementContentAsInt(xml, PARENT_ID_ELEMENT);
		}
	}
}