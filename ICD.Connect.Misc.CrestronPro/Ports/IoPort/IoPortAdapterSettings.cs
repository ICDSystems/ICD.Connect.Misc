using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Xml;
using ICD.Connect.Misc.CrestronPro.Devices;
using ICD.Connect.Protocol.Ports.IoPort;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.CrestronPro.Ports.IoPort
{
	public sealed class IoPortAdapterSettings : AbstractIoPortSettings
	{
		private const string FACTORY_NAME = "IoPort";

		private const string PARENT_DEVICE_ELEMENT = "Device";
		private const string ADDRESS_ELEMENT = "Address";
		private const string CONFIGURATION_ELEMENT = "Configuration";

		private int m_Address = 1;

		#region Properties

		[SettingsProperty(SettingsProperty.ePropertyType.Id, typeof(IPortParent))]
		public int? Device { get; set; }

		public int Address { get { return m_Address; } set { m_Address = value; } }

		public eIoPortConfiguration Configuration { get; set; }

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(IoPortAdapter); } }

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
        /// Returns true if the settings depend on a device with the given ID.
        /// For example, to instantiate an IR Port from settings, the device the physical port
        /// belongs to will need to be instantiated first.
        /// </summary>
        /// <returns></returns>
        public override bool HasDeviceDependency(int id)
        {
            return Device != null && Device == id;
        }

        /// <summary>
        /// Returns the count from the collection of ids that the settings depends on.
        /// </summary>
        public override int DependencyCount
        {
            get { return Device != null ? 1 : 0; }
        } 

		/// <summary>
		/// Loads the settings from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI, XmlFactoryMethod(FACTORY_NAME)]
		public static IoPortAdapterSettings FromXml(string xml)
		{
			int device = XmlUtils.TryReadChildElementContentAsInt(xml, PARENT_DEVICE_ELEMENT) ?? 0;
			int address = XmlUtils.TryReadChildElementContentAsInt(xml, ADDRESS_ELEMENT) ?? 0;

			eIoPortConfiguration configuration;
			XmlUtils.TryReadChildElementContentAsEnum(xml, CONFIGURATION_ELEMENT, true, out configuration);

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
