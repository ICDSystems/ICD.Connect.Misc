using ICD.Connect.Devices.Mock;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.Occupancy
{
	[KrangSettings("MockOccupancySensorDevice", typeof(MockOccupancySensorDevice))]
	public sealed class MockOccupancySensorDeviceSettings : AbstractMockDeviceSettings
	{
	}
}
