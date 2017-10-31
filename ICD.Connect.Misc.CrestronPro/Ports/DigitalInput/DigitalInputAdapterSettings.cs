﻿using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils.Xml;
using ICD.Connect.Protocol.Ports.DigitalInput;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.CrestronPro.Ports.DigitalInput
{
    public sealed class DigitalInputAdapterSettings : AbstractDigitalInputPortSettings
	{
		private const string FACTORY_NAME = "DigitalInputPort";

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

		/// <summary>
		/// Gets the type of the originator for this settings instance.
		/// </summary>
		public override Type OriginatorType { get { return typeof(DigitalInputAdapter); } }

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
		public static DigitalInputAdapterSettings FromXml(string xml)
		{
			int device = XmlUtils.TryReadChildElementContentAsInt(xml, PARENT_DEVICE_ELEMENT) ?? 0;
			int address = XmlUtils.TryReadChildElementContentAsInt(xml, ADDRESS_ELEMENT) ?? 0;

            DigitalInputAdapterSettings output = new DigitalInputAdapterSettings
			{
				Device = device,
				Address = address
			};

			ParseXml(output, xml);
			return output;
		}

		#endregion
	}
}
