﻿using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.Xml;
using ICD.Connect.Devices;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Attributes.Factories;

namespace ICD.Connect.Misc.CrestronPro.Devices
{
	public sealed class DinIo8AdapterSettings : AbstractDeviceSettings
	{
		private const string FACTORY_NAME = "DinIo8";

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
		[PublicAPI, XmlDeviceSettingsFactoryMethod(FACTORY_NAME)]
		public static DinIo8AdapterSettings FromXml(string xml)
		{
			byte cresnetId = XmlUtils.ReadChildElementContentAsByte(xml, CRESNET_ID_ELEMENT);

			DinIo8AdapterSettings output = new DinIo8AdapterSettings
			{
				CresnetId = cresnetId
			};

			ParseXml(output, xml);
			return output;
		}
	}
}
