using ICD.Connect.Devices.Mock;
using ICD.Connect.Settings.Attributes;

namespace ICD.Connect.Misc.Bluetooth.MockBluetoothDevice
{
	[KrangSettings("MockBluetoothDevice", typeof(MockBluetoothDevice))]
	public sealed class MockBluetoothDeviceSettings : AbstractMockDeviceSettings
	{
	}
}