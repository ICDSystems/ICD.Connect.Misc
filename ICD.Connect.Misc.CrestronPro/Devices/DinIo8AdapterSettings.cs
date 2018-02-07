using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Xml;
using ICD.Connect.Devices;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Misc.CrestronPro.Devices
{
	public sealed class DinIo8AdapterSettings : AbstractDeviceSettings
	{
		private const string FACTORY_NAME = "DinIo8";

		private const string CRESNET_ID_ELEMENT = "CresnetID";

		[IpIdSettingsProperty]
		public byte CresnetId { get; set; }

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(DinIo8Adapter); } }

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
		public static DinIo8AdapterSettings FromXml(string xml)
		{
			DinIo8AdapterSettings output = new DinIo8AdapterSettings
			{
				CresnetId = XmlUtils.TryReadChildElementContentAsByte(xml, CRESNET_ID_ELEMENT) ?? 0
			};

			output.ParseXml(xml);
			return output;
		}
	}
}
