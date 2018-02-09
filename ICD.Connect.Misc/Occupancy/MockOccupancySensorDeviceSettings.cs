using System;
using ICD.Connect.Devices;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.Occupancy
{
	[KrangSettings(FACTORY_NAME)]
	public sealed class MockOccupancySensorDeviceSettings : AbstractDeviceSettings
	{
		private const string FACTORY_NAME = "MockOccupancySensorDevice";

		public override string FactoryName { get { return FACTORY_NAME; } }

		public override Type OriginatorType { get { return typeof(MockOccupancySensorDevice); } }
	}
}
