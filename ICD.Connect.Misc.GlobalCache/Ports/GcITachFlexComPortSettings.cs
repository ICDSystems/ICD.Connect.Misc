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
		private const string FACTORY_NAME = "GcITachComPort";

		private const string PARENT_DEVICE_ELEMENT = "Device";

		#region Properties

		[SettingsProperty(SettingsProperty.ePropertyType.DeviceId)]
		public int? Device { get; set; }

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
			int? device = XmlUtils.TryReadChildElementContentAsInt(xml, PARENT_DEVICE_ELEMENT);

			GcITachFlexComPortSettings output = new GcITachFlexComPortSettings
			{
				Device = device
			};

			ParseXml(output, xml);
			return output;
		}

		#endregion
	}
}