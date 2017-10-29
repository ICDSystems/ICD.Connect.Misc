using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils.Xml;
using ICD.Connect.Protocol.Ports.ComPort;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.GlobalCache.Ports
{
	public sealed class GcITachFlexComPortSettings : AbstractComPortSettings
	{
		private const string FACTORY_NAME = "iTachFlexComPort";

		private const string PARENT_DEVICE_ELEMENT = "Device";
		private const string PARENT_MODULE_ELEMENT = "Module";
		private const string PARENT_ADDRESS_ELEMENT = "Address";

		#region Properties

		[SettingsProperty(SettingsProperty.ePropertyType.DeviceId)]
		public int? Device { get; set; }

		public int Module { get; set; }

		public int Address { get; set; }

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(GcITachFlexComPort); } }

		#endregion

		#region Methods

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(PARENT_DEVICE_ELEMENT, IcdXmlConvert.ToString(Device));
			writer.WriteElementString(PARENT_MODULE_ELEMENT, IcdXmlConvert.ToString(Module));
			writer.WriteElementString(PARENT_ADDRESS_ELEMENT, IcdXmlConvert.ToString(Address));
		}

		/// <summary>
		/// Returns the collection of ids that the settings will depend on.
		/// For example, to instantiate an IR Port from settings, the device the physical port
		/// belongs to will need to be instantiated first.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<int> GetDeviceDependencies()
		{
			if (Device != null)
				yield return (int)Device;
		}

		/// <summary>
		/// Loads the settings from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI, XmlFactoryMethod(FACTORY_NAME)]
		public static GcITachFlexComPortSettings FromXml(string xml)
		{
			GcITachFlexComPortSettings output = new GcITachFlexComPortSettings
			{
				Device = XmlUtils.TryReadChildElementContentAsInt(xml, PARENT_DEVICE_ELEMENT),
				Module = XmlUtils.TryReadChildElementContentAsInt(xml, PARENT_MODULE_ELEMENT) ?? 1,
				Address = XmlUtils.TryReadChildElementContentAsInt(xml, PARENT_ADDRESS_ELEMENT) ?? 1,
			};

			ParseXml(output, xml);
			return output;
		}

		#endregion
	}
}