using System.Collections.Generic;
using ICD.Common.Attributes.Properties;
using ICD.Common.Properties;
using ICD.Common.Utils.Xml;
using ICD.Connect.Protocol.Ports;
using ICD.Connect.Settings;
using ICD.Connect.Settings.Attributes.Factories;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Misc.CrestronPro.Ports.IoPort
{
	public sealed class IoPortAdapterSettings : AbstractPortSettings
	{
		private const string FACTORY_NAME = "IoPort";

		private const string PARENT_DEVICE_ELEMENT = "Device";
		private const string ADDRESS_ELEMENT = "Address";
		private const string CONFIGURATION_ELEMENT = "Configuration";

		private int m_Address = 1;

		#region Properties

		[SettingsProperty(SettingsProperty.ePropertyType.DeviceId)]
		public int? Device { get; set; }

		public int Address { get { return m_Address; } set { m_Address = value; } }

		[SettingsProperty(SettingsProperty.ePropertyType.Enum)]
		public eIoPortConfiguration Configuration { get; set; }

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		#endregion

		#region Methods

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			if (Device != null)
				writer.WriteElementString(PARENT_DEVICE_ELEMENT, IcdXmlConvert.ToString((int)Device));
			writer.WriteElementString(ADDRESS_ELEMENT, IcdXmlConvert.ToString(Address));
			writer.WriteElementString(CONFIGURATION_ELEMENT, Configuration.ToString());
		}

		/// <summary>
		/// Creates a new originator instance from the settings.
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		public override IOriginator ToOriginator(IDeviceFactory factory)
		{
			IoPortAdapter output = new IoPortAdapter();
			output.ApplySettings(this, factory);
			return output;
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
		[PublicAPI, XmlPortSettingsFactoryMethod(FACTORY_NAME)]
		public static IoPortAdapterSettings FromXml(string xml)
		{
			int? device = XmlUtils.TryReadChildElementContentAsInt(xml, PARENT_DEVICE_ELEMENT);
			int address = XmlUtils.ReadChildElementContentAsInt(xml, ADDRESS_ELEMENT);
			eIoPortConfiguration configuration =
				XmlUtils.ReadChildElementContentAsEnum<eIoPortConfiguration>(xml, CONFIGURATION_ELEMENT, true);

			IoPortAdapterSettings output = new IoPortAdapterSettings
			{
				Device = device,
				Address = address,
				Configuration = configuration
			};

			ParseXml(output, xml);
			return output;
		}

		#endregion
	}
}
