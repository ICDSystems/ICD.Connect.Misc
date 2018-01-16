using System;
using ICD.Common.Properties;
using ICD.Common.Utils.Xml;
using ICD.Connect.Misc.CrestronPro.Devices;
using ICD.Connect.Protocol.Ports.IrPort;
using ICD.Connect.Settings.Attributes;
using ICD.Connect.Settings.Attributes.SettingsProperties;

namespace ICD.Connect.Misc.CrestronPro.Ports.IrPort
{
	/// <summary>
	/// Settings for the IrPortAdapter.
	/// </summary>
	public sealed class IrPortAdapterSettings : AbstractIrPortSettings
	{
		private const string FACTORY_NAME = "IrPort";

		private const ushort DEFAULT_PULSE_TIME = 100;
		private const ushort DEFAULT_BETWEEN_TIME = 750;

		private const string PARENT_DEVICE_ELEMENT = "Device";
		private const string ADDRESS_ELEMENT = "Address";
		private const string DRIVER_ELEMENT = "Driver";
		private const string PULSETIME_ELEMENT = "PulseTime";
		private const string BETWEENTIME_ELEMENT = "BetweenTime";

		private int m_Address = 1;
		private ushort m_PulseTime = DEFAULT_PULSE_TIME;
		private ushort m_BetweenTime = DEFAULT_BETWEEN_TIME;

		#region Properties

		[OriginatorIdSettingsProperty(typeof(IPortParent))]
		public int? Device { get; set; }

		public int Address { get { return m_Address; } set { m_Address = value; } }

		[PathSettingsProperty("IRDrivers", ".ir")]
		public string Driver { get; set; }

		public ushort PulseTime { get { return m_PulseTime; } set { m_PulseTime = value; } }

		public ushort BetweenTime { get { return m_BetweenTime; } set { m_BetweenTime = value; } }

		/// <summary>
		/// Gets the originator factory name.
		/// </summary>
		public override string FactoryName { get { return FACTORY_NAME; } }

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(IrPortAdapter); } }

		#endregion

		#region Method

		/// <summary>
		/// Writes property elements to xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteElements(IcdXmlTextWriter writer)
		{
			base.WriteElements(writer);

			writer.WriteElementString(PARENT_DEVICE_ELEMENT, IcdXmlConvert.ToString(Device));
			writer.WriteElementString(ADDRESS_ELEMENT, IcdXmlConvert.ToString(Address));
			writer.WriteElementString(DRIVER_ELEMENT, Driver);
			writer.WriteElementString(PULSETIME_ELEMENT, IcdXmlConvert.ToString(PulseTime));
			writer.WriteElementString(BETWEENTIME_ELEMENT, IcdXmlConvert.ToString(BetweenTime));
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
		public override int DependencyCount { get { return Device != null ? 1 : 0; } }

		/// <summary>
		/// Loads the settings from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI, XmlFactoryMethod(FACTORY_NAME)]
		public static IrPortAdapterSettings FromXml(string xml)
		{
			int device = XmlUtils.TryReadChildElementContentAsInt(xml, PARENT_DEVICE_ELEMENT) ?? 0;
			int address = XmlUtils.TryReadChildElementContentAsInt(xml, ADDRESS_ELEMENT) ?? 0;
			string driver = XmlUtils.TryReadChildElementContentAsString(xml, DRIVER_ELEMENT);
			ushort pulseTime = (ushort?)XmlUtils.TryReadChildElementContentAsInt(xml, PULSETIME_ELEMENT) ?? 0;
			ushort betweenTime = (ushort?)XmlUtils.TryReadChildElementContentAsInt(xml, BETWEENTIME_ELEMENT) ?? 0;

			IrPortAdapterSettings output = new IrPortAdapterSettings
			{
				Device = device,
				Address = address,
				Driver = driver,
				PulseTime = pulseTime,
				BetweenTime = betweenTime
			};

			ParseXml(output, xml);
			return output;
		}

		#endregion
	}
}
