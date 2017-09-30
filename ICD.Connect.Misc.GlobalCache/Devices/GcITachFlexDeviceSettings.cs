using System;
using ICD.Common.Properties;
using ICD.Connect.Devices;
using ICD.Connect.Settings.Attributes;
using ICD.Common.Utils.Xml;

namespace ICD.Connect.Misc.GlobalCache.Devices
{
    public sealed class GcITachFlexDeviceSettings : AbstractDeviceSettings
	{
		private const string FACTORY_NAME = "iTachFlex";

		private const string ADDRESS_ELEMENT = "Address";

		public override string FactoryName { get { return FACTORY_NAME; } }

		public override Type OriginatorType { get { return typeof(GcITachFlexDevice); } }

		/// <summary>
		/// The network address of the iTach device.
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(ADDRESS_ELEMENT, Address);
		}

		/// <summary>
		/// Loads the settings from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI, XmlFactoryMethod(FACTORY_NAME)]
		public static GcITachFlexDeviceSettings FromXml(string xml)
		{
			GcITachFlexDeviceSettings output = new GcITachFlexDeviceSettings
			{
				Address = XmlUtils.TryReadChildElementContentAsString(xml, ADDRESS_ELEMENT)
			};

			ParseXml(output, xml);
			return output;
		}
	}
}
