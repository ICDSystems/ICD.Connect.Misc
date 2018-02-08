using System;
using ICD.Common.Properties;
using ICD.Connect.Devices;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.Occupancy
{
	public sealed class MockOccupancySensorDeviceSettings : AbstractDeviceSettings
	{

		private const string FACTORY_NAME = "MockOccupancySensorDevice";


		public override string FactoryName { get { return FACTORY_NAME; } }


		public override Type OriginatorType { get { return typeof(MockOccupancySensorDevice); } }

		/// <summary>
		/// Loads the settings from XML.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		[PublicAPI, XmlFactoryMethod(FACTORY_NAME)]
		public static MockOccupancySensorDeviceSettings FromXml(string xml)
		{
			MockOccupancySensorDeviceSettings output = new MockOccupancySensorDeviceSettings();
			
			ParseXml(output, xml);
			return output;
		}
	}
}
