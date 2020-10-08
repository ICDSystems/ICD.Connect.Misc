using System;
using ICD.Connect.Devices.Controls;
using ICD.Connect.Devices.Mock;
using ICD.Connect.Settings;

namespace ICD.Connect.Misc.Bluetooth.MockBluetoothDevice
{
	public sealed class MockBluetoothDevice : AbstractMockDevice<MockBluetoothDeviceSettings>, IBluetoothDevice
	{
		/// <summary>
		/// Override to add controls to the device.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		/// <param name="addControl"></param>
		protected override void AddControls(MockBluetoothDeviceSettings settings, IDeviceFactory factory, Action<IDeviceControl> addControl)
		{
			base.AddControls(settings, factory, addControl);

			addControl(new MockBluetoothControl(this, 1));
		}
	}
}