using System;
using ICD.Common.Utils.EventArguments;

namespace ICD.Connect.Misc.Bluetooth
{

	[Flags]
	public enum eBluetoothDiscoverableFeatures
	{
		None = 0,
		StartStopDiscvoery = 1, // Supports starting/stopping discoverable status
		GetName = 2 // Supports getting this device's Bluetooth name
	}

	public interface IBluetoothDiscoverableControl : IBluetoothControl
	{
		event EventHandler<BoolEventArgs> OnBluetoothDiscoverableStatusChanged;

		event EventHandler<StringEventArgs> OnBluetoothDiscoverableNameChanged;
		
		eBluetoothDiscoverableFeatures BluetoothDiscoverableFeatures { get; }

		bool BluetoothDiscoverableStatus { get; }

		string BluetoothDiscoverableName { get; }

		void BluetoothDiscoverableStart(bool start);
	}
}