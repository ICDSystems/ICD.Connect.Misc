using System.Collections.Generic;
using ICD.Common.Attributes.Properties;
using ICD.Common.Properties;
using ICD.Common.Utils.Xml;
using ICD.Connect.Settings;
using ICD.Connect.Settings.Attributes.Factories;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Misc.CrestronPro.Ports.ComPort
{
	/// <summary>
	/// Settings for the ComPortAdapter.
	/// </summary>
	public sealed class ComPortAdapterSettings : AbstractPortSettings
	{
		private const string FACTORY_NAME = "ComPort";

		private const string PARENT_DEVICE_ELEMENT = "Device";
		private const string ADDRESS_ELEMENT = "Address";

		private int m_Address = 1;

		#region Properties

		[SettingsProperty(SettingsProperty.ePropertyType.DeviceId)]
		public int? Device { get; set; }

		public int Address { get { return m_Address; } set { m_Address = value; } }

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
		}

		/// <summary>
		/// Creates a new originator instance from the settings.
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		public override IOriginator ToOriginator(IDeviceFactory factory)
		{
			ComPortAdapter output = new ComPortAdapter();
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
		public static ComPortAdapterSettings FromXml(string xml)
		{
			int? device = XmlUtils.TryReadChildElementContentAsInt(xml, PARENT_DEVICE_ELEMENT);
			int address = XmlUtils.ReadChildElementContentAsInt(xml, ADDRESS_ELEMENT);

			ComPortAdapterSettings output = new ComPortAdapterSettings
			{
				Device = device,
				Address = address,
			};

			ParseXml(output, xml);
			return output;
		}

		#endregion
	}
}
