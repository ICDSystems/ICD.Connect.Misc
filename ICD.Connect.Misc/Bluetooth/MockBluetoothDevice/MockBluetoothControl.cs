using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Devices.Controls;

namespace ICD.Connect.Misc.Bluetooth.MockBluetoothDevice
{
	public sealed class MockBluetoothControl : AbstractDeviceControl<MockBluetoothDevice>, IBluetoothConnectedControl, IBluetoothDiscoverableControl
	{
		#region Fields

		private bool m_BluetoothConnectedStatus;
		private string m_BluetoothConnectedDeviceName;
		private bool m_BluetoothDiscoverableStatus;
		private string m_BluetoothDiscoverableName;

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="id"></param>
		public MockBluetoothControl([NotNull] MockBluetoothDevice parent, int id) : base(parent, id)
		{

			BluetoothDiscoverableFeatures = EnumUtils.GetFlagsAllValue<eBluetoothDiscoverableFeatures>();
			BluetoothConnectedFeatures = EnumUtils.GetFlagsAllValue<eBluetoothConnectedFeatures>();
			m_BluetoothDiscoverableName = Parent.Name;
		}

		#region Events

		public event EventHandler<BoolEventArgs> OnBluetoothConnectedStatusChanged;
		public event EventHandler<StringEventArgs> OnBluetootConnectedDeviceNameChanged;
		public event EventHandler<BoolEventArgs> OnBluetoothDiscoverableStatusChanged;

		public event EventHandler<StringEventArgs> OnBluetoothDiscoverableNameChanged;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the parent device for this control.
		/// </summary>
		IBluetoothDevice IDeviceControl<IBluetoothDevice>.Parent { get { return Parent; } }

		public eBluetoothConnectedFeatures BluetoothConnectedFeatures { get; private set; }

		public bool BluetoothConnectedStatus
		{
			get { return m_BluetoothConnectedStatus; }
			private set
			{
				if (m_BluetoothConnectedStatus == value)
					return;

				m_BluetoothConnectedStatus = value;

				OnBluetoothConnectedStatusChanged.Raise(this, m_BluetoothConnectedStatus);
			}
		}

		public string BluetoothConnectedDeviceName
		{
			get { return m_BluetoothConnectedDeviceName; }
			private set
			{
				if (m_BluetoothConnectedDeviceName == value)
					return;

				m_BluetoothConnectedDeviceName = value;

				OnBluetootConnectedDeviceNameChanged.Raise(this, m_BluetoothConnectedDeviceName);
			}
		}

		public eBluetoothDiscoverableFeatures BluetoothDiscoverableFeatures { get; private set; }

		public bool BluetoothDiscoverableStatus
		{
			get { return m_BluetoothDiscoverableStatus; }
			private set
			{
				if (m_BluetoothDiscoverableStatus == value)
					return;

				m_BluetoothDiscoverableStatus = value;

				OnBluetoothDiscoverableStatusChanged.Raise(this, m_BluetoothDiscoverableStatus);
			}
		}

		public string BluetoothDiscoverableName
		{
			get { return m_BluetoothDiscoverableName; }
			private set
			{
				if (m_BluetoothDiscoverableName == value)
					return;

				m_BluetoothDiscoverableName = value;

				OnBluetoothDiscoverableNameChanged.Raise(this, m_BluetoothDiscoverableName);
			}
		}

		#endregion

		#region Methods

		public void BluetoothDiscoverableStart(bool start)
		{
			BluetoothDiscoverableStatus = start;
		}

		public void BluetoothDisconnect()
		{
			BluetoothConnectedStatus = false;
			BluetoothConnectedDeviceName = null;
		}

		private void ConnectDevice()
		{
			ConnectDevice(null);
		}

		private void ConnectDevice(string deviceName)
		{
			BluetoothConnectedDeviceName = deviceName;
			BluetoothConnectedStatus = true;
		}

		private void DisconnectDevice()
		{
			BluetoothConnectedDeviceName = null;
			BluetoothConnectedStatus = false;
		}

		#endregion

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			yield return
				new GenericConsoleCommand<string>("ConnectName", "Simulate a device connecting, passing in a name",
				                                  s => ConnectDevice(s));
			yield return new ConsoleCommand("Connect", "Simulate a device connecting", () => ConnectDevice());
			yield return new ConsoleCommand("Disconnect", "Simulate a device disconnecting", () => DisconnectDevice());
			yield return
				new ConsoleCommand("StartPairing", "Simualtes starting paring", () => BluetoothDiscoverableStatus = true);
			yield return
				new ConsoleCommand("StopPairing", "Simualtes stopping paring", () => BluetoothDiscoverableStatus = false);

		}

		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			addRow("Connected Status", BluetoothConnectedStatus);
			addRow("ConnectedDeviceName", BluetoothConnectedDeviceName);
			addRow("DiscoverableStatus", BluetoothDiscoverableStatus);
			addRow("DiscoverableName", BluetoothDiscoverableName);
		}
	}
}