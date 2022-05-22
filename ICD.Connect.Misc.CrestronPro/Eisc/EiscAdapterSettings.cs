using ICD.Common.Utils;
using ICD.Common.Utils.Xml;
using ICD.Connect.Devices;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.CrestronPro.Eisc
{
	[KrangSettings("EiscAdapter", typeof(EiscAdapter))]
	public sealed class EiscAdapterSettings : AbstractDeviceSettings
	{
		private const string ELEMENT_EISC_IPID = "IPID";
		private const string ELEMENT_EISC_ADDRESS = "Address";

		public byte EiscIpid { get; set; }

		public string EiscAddress { get; set; }

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(ELEMENT_EISC_IPID, StringUtils.ToIpIdString(EiscIpid));
			writer.WriteElementString(ELEMENT_EISC_ADDRESS, IcdXmlConvert.ToString(EiscAddress));
		}

		/// <summary>
		/// Updates the settings from xml.
		/// </summary>
		/// <param name="xml"></param>
		public override void ParseXml(string xml)
		{
			base.ParseXml(xml);

			EiscIpid = XmlUtils.ReadChildElementContentAsByte(xml, ELEMENT_EISC_IPID);
			EiscAddress = XmlUtils.ReadChildElementContentAsString(xml, ELEMENT_EISC_ADDRESS);


		}
	}
}