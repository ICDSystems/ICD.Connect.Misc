using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Xml;
using ICD.Connect.Partitioning.Devices;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.CrestronPro.Devices.Partitioning
{
	public sealed class GlsPartCnAdapterSettings : AbstractPartitionDeviceSettings
	{
		private const string FACTORY_NAME = "GlsPartCn";

		private const string CRESNET_ID_ELEMENT = "CresnetID";

		[SettingsProperty(SettingsProperty.ePropertyType.Ipid)]
		public byte CresnetId { get; set; }

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

			writer.WriteElementString(CRESNET_ID_ELEMENT, StringUtils.ToIpIdString(CresnetId));
		}

		/// <summary>
		/// Loads the settings from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI, XmlFactoryMethod(FACTORY_NAME)]
		public static GlsPartCnAdapterSettings FromXml(string xml)
		{
			GlsPartCnAdapterSettings output = new GlsPartCnAdapterSettings
			{
				CresnetId = XmlUtils.TryReadChildElementContentAsByte(xml, CRESNET_ID_ELEMENT) ?? 0
			};

			ParseXml(output, xml);
			return output;
		}
	}
}
