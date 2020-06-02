using System;
using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Timers;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.Protocol.Extensions;
using ICD.Connect.Protocol.Ports.RelayPort;
using ICD.Connect.Routing;
using ICD.Connect.Routing.Connections;
using ICD.Connect.Routing.Controls;
using ICD.Connect.Routing.Devices;
using ICD.Connect.Routing.EventArguments;
using ICD.Connect.Routing.Utils;
using ICD.Connect.Settings;

namespace ICD.Connect.Misc.Yepkit.Devices.YkupSwitcher
{
	/// <summary>
	/// YkupSwitcherDevice is a 1 input to 2 output USB switcher that is switched by two relays:
	///		Power Relay - Open powers off, closed powers on
	///		Switch Relay - Close followed by Open toggles the output while powered
	/// 
	/// In order to consistently land on input 1:
	///		1 - Close the switch relay, wait
	///		2 - Open the power relay, wait
	///		3 - Close the power relay, wait
	///		4 - Open the switch relay, wait
	/// 
	/// Then to switch to input 2:
	///		1 - Close the switch relay, wait
	///		2 - Open the switch relay, wait
	/// 
	/// Power is cycled when switching so we can be sure we are always synchronized with the device.
	/// </summary>
	public sealed class YkupSwitcherDevice : AbstractRouteSwitcherDevice<YkupSwitcherDeviceSettings>
	{
		private enum eState
		{
			None,

			// Power Cycle (switch to output 1)
			PrePowerOff,
			PowerOff,
			PowerOn,
			PostPowerOff,

			// Toggle output
			PreSwitch,
			Switch
		}

		private static readonly Dictionary<eState, long> s_StateToDuration =
			new Dictionary<eState, long>
			{
				{eState.PrePowerOff, 100},
				{eState.PowerOff, 500},
				{eState.PowerOn, 100},
				{eState.PostPowerOff, 100},

				{eState.PreSwitch, 100},
				{eState.Switch, 100}
			};

		private static readonly Dictionary<eState, eState> s_StateTransitions =
			new Dictionary<eState, eState>
			{
				{eState.PrePowerOff, eState.PowerOff},
				{eState.PowerOff, eState.PowerOn},
				{eState.PowerOn, eState.PostPowerOff},
				{eState.PostPowerOff, eState.None},

				{eState.PreSwitch, eState.Switch},
				{eState.Switch, eState.None}
			};

		private static readonly Dictionary<eState, Action<YkupSwitcherDevice>> s_StateActions =
			new Dictionary<eState, Action<YkupSwitcherDevice>>
			{
				{eState.PrePowerOff, s => s.SwitchPort.Close()},
				{eState.PowerOff, s => s.PowerPort.Open()},
				{eState.PowerOn, s => s.PowerPort.Close()},
				{eState.PostPowerOff, s => s.SwitchPort.Open()},

				{eState.PreSwitch, s => s.SwitchPort.Close()},
				{eState.Switch, s => s.SwitchPort.Open()}
			};

		private const int INPUT_ADDRESS = 1;
		private const int OUTPUT_1_ADDRESS = 1;
		private const int OUTPUT_2_ADDRESS = 2;

		private const eConnectionType CONNECTION_TYPE =
			eConnectionType.Audio |
			eConnectionType.Video |
			eConnectionType.Usb;

		/// <summary>
		/// Raised when an input source status changes.
		/// </summary>
		public override event EventHandler<SourceDetectionStateChangeEventArgs> OnSourceDetectionStateChange;

		/// <summary>
		/// Raised when the device starts/stops actively using an input, e.g. unroutes an input.
		/// </summary>
		public override event EventHandler<ActiveInputStateChangeEventArgs> OnActiveInputsChanged;

		/// <summary>
		/// Called when a route changes.
		/// </summary>
		public override event EventHandler<RouteChangeEventArgs> OnRouteChange;

		/// <summary>
		/// Raised when the device starts/stops actively transmitting on an output.
		/// </summary>
		public override event EventHandler<TransmissionStateEventArgs> OnActiveTransmissionStateChanged;

		private readonly SwitcherCache m_Cache;
		private readonly SafeTimer m_StateTimer;

		private IRelayPort m_PowerPort;
		private IRelayPort m_SwitchPort;
		private int m_ExpectedOutput;
		private eState m_State;

		#region Properties

		/// <summary>
		/// Toggling cycles the device power and resets back to output 1
		/// </summary>
		public IRelayPort PowerPort
		{
			get { return m_PowerPort; }
			set
			{
				if (value == m_PowerPort)
					return;

				Unsubscribe(m_PowerPort);
				m_PowerPort = value;
				Subscribe(m_PowerPort);

				UpdateCachedOnlineStatus();
			}
		}

		/// <summary>
		/// Toggling switches the output back and forth
		/// </summary>
		public IRelayPort SwitchPort
		{
			get { return m_SwitchPort; }
			set
			{
				if (value == m_SwitchPort)
					return;

				Unsubscribe(m_SwitchPort);
				m_SwitchPort = value;
				Subscribe(m_SwitchPort);

				UpdateCachedOnlineStatus();
			}
		}

		/// <summary>
		/// Gets the UTC time that the switcher was last switched.
		/// </summary>
		public DateTime LastSwitchTime { get; private set; }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public YkupSwitcherDevice()
		{
			m_Cache = new SwitcherCache();
			m_Cache.OnActiveInputsChanged += CacheOnActiveInputsChanged;
			m_Cache.OnActiveTransmissionStateChanged += CacheOnActiveTransmissionStateChanged;
			m_Cache.OnRouteChange += CacheOnRouteChange;
			m_Cache.OnSourceDetectionStateChange += CacheOnSourceDetectionStateChange;

			m_StateTimer = SafeTimer.Stopped(AdvanceToNextState);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			m_StateTimer.Dispose();

			base.DisposeFinal(disposing);
		}

		#region Methods

		/// <summary>
		/// Returns true if a signal is detected at the given input.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public override bool GetSignalDetectedState(int input, eConnectionType type)
		{
			if (!ContainsInput(input))
				throw new ArgumentOutOfRangeException("input");

			if (type == eConnectionType.None)
				throw new ArgumentOutOfRangeException("type");

			return true;
		}

		/// <summary>
		/// Gets the input at the given address.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public override ConnectorInfo GetInput(int input)
		{
			if (!ContainsInput(input))
				throw new ArgumentOutOfRangeException("input");

			return new ConnectorInfo(input, CONNECTION_TYPE);
		}

		/// <summary>
		/// Returns true if the destination contains an input at the given address.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public override bool ContainsInput(int input)
		{
			return input == INPUT_ADDRESS;
		}

		/// <summary>
		/// Returns the inputs.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<ConnectorInfo> GetInputs()
		{
			yield return GetInput(INPUT_ADDRESS);
		}

		/// <summary>
		/// Gets the output at the given address.
		/// </summary>
		/// <param name="output"></param>
		/// <returns></returns>
		public override ConnectorInfo GetOutput(int output)
		{
			if (!ContainsOutput(output))
				throw new ArgumentOutOfRangeException("output");

			return new ConnectorInfo(output, CONNECTION_TYPE);
		}

		/// <summary>
		/// Returns true if the source contains an output at the given address.
		/// </summary>
		/// <param name="output"></param>
		/// <returns></returns>
		public override bool ContainsOutput(int output)
		{
			return output == OUTPUT_1_ADDRESS || output == OUTPUT_2_ADDRESS;
		}

		/// <summary>
		/// Returns the outputs.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<ConnectorInfo> GetOutputs()
		{
			yield return GetOutput(OUTPUT_1_ADDRESS);
			yield return GetOutput(OUTPUT_2_ADDRESS);
		}

		/// <summary>
		/// Gets the input routed to the given output matching the given type.
		/// </summary>
		/// <param name="output"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException">Type has multiple flags.</exception>
		public override ConnectorInfo? GetInput(int output, eConnectionType type)
		{
			if (!ContainsOutput(output))
				throw new ArgumentOutOfRangeException("output");

			if (type == eConnectionType.None)
				throw new ArgumentOutOfRangeException("type");

			return m_Cache.GetInputConnectorInfoForOutput(output, type);
		}

		/// <summary>
		/// Gets the outputs for the given input.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public override IEnumerable<ConnectorInfo> GetOutputs(int input, eConnectionType type)
		{
			if (!ContainsOutput(input))
				throw new ArgumentOutOfRangeException("input");

			if (type == eConnectionType.None)
				throw new ArgumentOutOfRangeException("type");

			return m_Cache.GetOutputsForInput(input, type);
		}

		/// <summary>
		/// Performs the given route operation.
		/// </summary>
		/// <param name="info"></param>
		/// <returns></returns>
		public override bool Route(RouteOperation info)
		{
			if (info == null)
				throw new ArgumentNullException("info");

			int input = info.LocalInput;
			if (!ContainsInput(input))
				throw new ArgumentException("No input with address " + input);

			int output = info.LocalOutput;
			return Route(output);
		}

		/// <summary>
		/// Routes the input to the given output.
		/// </summary>
		/// <param name="output"></param>
		/// <returns></returns>
		public bool Route(int output)
		{
			if (!ContainsOutput(output))
				throw new ArgumentOutOfRangeException("output");

			if (PowerPort == null || SwitchPort == null)
				throw new InvalidOperationException("Missing ports for switching");

			// No change
			if (GetInput(output, eConnectionType.Video) != null)
				return false;

			LastSwitchTime = IcdEnvironment.GetUtcTime();
			m_ExpectedOutput = output;

			// Start the state machine
			SetState(eState.PrePowerOff);

			return true;
		}

		/// <summary>
		/// Stops routing to the given output.
		/// </summary>
		/// <param name="output"></param>
		/// <param name="type"></param>
		/// <returns>True if successfully cleared.</returns>
		public override bool ClearOutput(int output, eConnectionType type)
		{
			// Clearing output doesn't really make sense
			return false;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return PowerPort != null &&
			       PowerPort.IsOnline &&
			       SwitchPort != null &&
			       SwitchPort.IsOnline;
		}

		/// <summary>
		/// Sets the current state.
		/// Performs the action for the state and resets the timer for the interval.
		/// </summary>
		/// <param name="state"></param>
		private void SetState(eState state)
		{
			if (state == m_State)
				return;

			m_State = state;

			// Execute the action for this state
			Action<YkupSwitcherDevice> stateAction = s_StateActions.GetDefault(m_State, s => { });
			stateAction(this);

			// Set up the timer
			long duration;
			if (s_StateToDuration.TryGetValue(m_State, out duration))
				m_StateTimer.Reset(duration);
			else
				m_StateTimer.Stop();
		}

		/// <summary>
		/// Called after a state has completed.
		/// Advances to the next state.
		/// </summary>
		private void AdvanceToNextState()
		{
            eState nextState = s_StateTransitions.GetDefault(m_State);
			bool updateCache = false;

			// Edge cases
			switch (m_State)
			{
				case eState.PostPowerOff:
					switch (m_ExpectedOutput)
					{
						case OUTPUT_1_ADDRESS:
							// Done switching
							updateCache = true;
							break;

						case OUTPUT_2_ADDRESS:
							// Switch to the second output
							nextState = eState.PreSwitch;
							break;
					}
					break;

				case eState.Switch:
					updateCache = true;
					break;
			}

			// Update the cache to reflect the current routing state
			if (updateCache)
			{
				m_Cache.SetInputForOutput(OUTPUT_1_ADDRESS,
				                          m_ExpectedOutput == OUTPUT_1_ADDRESS ? INPUT_ADDRESS : (int?)null,
				                          CONNECTION_TYPE);
				m_Cache.SetInputForOutput(OUTPUT_2_ADDRESS,
				                          m_ExpectedOutput == OUTPUT_2_ADDRESS ? INPUT_ADDRESS : (int?)null,
				                          CONNECTION_TYPE);
			}

			SetState(nextState);
		}

		#endregion

		#region Port Callbacks

		/// <summary>
		/// Subscribe to the port events.
		/// </summary>
		/// <param name="port"></param>
		private void Subscribe(IRelayPort port)
		{
			if (port == null)
				return;

			port.OnIsOnlineStateChanged += PortOnIsOnlineStateChanged;
		}

		/// <summary>
		/// Unsubscribe from the port events.
		/// </summary>
		/// <param name="port"></param>
		private void Unsubscribe(IRelayPort port)
		{
			if (port == null)
				return;

			port.OnIsOnlineStateChanged -= PortOnIsOnlineStateChanged;
		}

		/// <summary>
		/// Called when the port online state changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void PortOnIsOnlineStateChanged(object sender, DeviceBaseOnlineStateApiEventArgs eventArgs)
		{
			UpdateCachedOnlineStatus();
		}

		#endregion

		#region Cache Callbacks

		private void CacheOnActiveInputsChanged(object sender, ActiveInputStateChangeEventArgs eventArgs)
		{
			OnActiveInputsChanged.Raise(this, new ActiveInputStateChangeEventArgs(eventArgs));
		}

		private void CacheOnActiveTransmissionStateChanged(object sender, TransmissionStateEventArgs eventArgs)
		{
			OnActiveTransmissionStateChanged.Raise(this, new TransmissionStateEventArgs(eventArgs));
		}

		private void CacheOnRouteChange(object sender, RouteChangeEventArgs eventArgs)
		{
			OnRouteChange.Raise(this, new RouteChangeEventArgs(eventArgs));
		}

		private void CacheOnSourceDetectionStateChange(object sender, SourceDetectionStateChangeEventArgs eventArgs)
		{
			OnSourceDetectionStateChange.Raise(this, new SourceDetectionStateChangeEventArgs(eventArgs));
		}

		#endregion

		#region Settings

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(YkupSwitcherDeviceSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			IRelayPort powerPort = null;
			IRelayPort switchPort = null;

			if (settings.PowerPort != null)
			{
				try
				{
					powerPort = factory.GetPortById((int)settings.PowerPort) as IRelayPort;
				}
				catch (KeyNotFoundException)
				{
					Logger.Log(eSeverity.Error, "No relay port with id {0}", settings.PowerPort);
				}
			}

			if (settings.SwitchPort != null)
			{
				try
				{
					switchPort = factory.GetPortById((int)settings.SwitchPort) as IRelayPort;
				}
				catch (KeyNotFoundException)
				{
					Logger.Log(eSeverity.Error, "No relay port with id {0}", settings.SwitchPort);
				}
			}

			PowerPort = powerPort;
			SwitchPort = switchPort;
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			PowerPort = null;
			SwitchPort = null;
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(YkupSwitcherDeviceSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.PowerPort = PowerPort == null ? (int?)null : PowerPort.Id;
			settings.SwitchPort = SwitchPort == null ? (int?)null : SwitchPort.Id;
		}

		/// <summary>
		/// Override to add controls to the device.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		/// <param name="addControl"></param>
		protected override void AddControls(YkupSwitcherDeviceSettings settings, IDeviceFactory factory, Action<ICD.Connect.Devices.Controls.IDeviceControl> addControl)
		{
			base.AddControls(settings, factory, addControl);

			addControl(new RouteSwitcherControl(this, 0));
		}

		#endregion
	}
}
