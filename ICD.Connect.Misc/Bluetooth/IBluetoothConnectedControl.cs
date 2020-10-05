using System;
using ICD.Common.Utils.EventArguments;

namespace ICD.Connect.Misc.Bluetooth
{

	[Flags]
	public enum eBluetoothConnectedFeatures
	{
		None = 0,
		Disconnect = 2, //Supprts disconnecting all devices
		ConnectedStatus = 4, //Supports getting global status of any device connected  
		ConnectedDeviceName = 8 //Supports getting a list of connected devices
	}

	public interface IBluetoothConnectedControl : IBluetoothControl
	{
		event EventHandler<BoolEventArgs> OnBluetoothConnectedStatusChanged;

		event EventHandler<StringEventArgs> OnBluetootConnectedDeviceNameChanged;

		eBluetoothConnectedFeatures BluetoothConnectedFeatures { get; }

		bool BluetoothConnectedStatus { get; }

		string BluetoothConnectedDeviceName { get; }
		
		void BluetoothDisconnect();
	}
}