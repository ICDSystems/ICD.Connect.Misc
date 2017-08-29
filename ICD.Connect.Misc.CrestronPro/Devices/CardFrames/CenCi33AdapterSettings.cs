using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Xml;
using ICD.Connect.Devices;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Attributes.Factories;

namespace ICD.Connect.Misc.CrestronPro.Devices.CardFrames
{
	public sealed class CenCi33AdapterSettings : AbstractDeviceSettings
	{
		private const string FACTORY_NAME = "CenCi33";

		private const string IPID_ELEMENT = "IPID";

		[SettingsProperty(SettingsProperty.ePropertyType.Ipid)]
		public byte Ipid { get; set; }

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(CenCi33Adapter); } }

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(IPID_ELEMENT, StringUtils.ToIpIdString(Ipid));
		}

		/// <summary>
		/// Loads the settings from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI, XmlDeviceSettingsFactoryMethod(FACTORY_NAME)]
		public static CenCi33AdapterSettings FromXml(string xml)
		{
			byte ipid = XmlUtils.ReadChildElementContentAsByte(xml, IPID_ELEMENT);

			CenCi33AdapterSettings output = new CenCi33AdapterSettings
			{
				Ipid = ipid
			};

			ParseXml(output, xml);
			return output;
		}
	}
}
