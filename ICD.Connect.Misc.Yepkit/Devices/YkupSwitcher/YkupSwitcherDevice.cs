using System;
using System.Collections.Generic;
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
	/// YkupSwitcherDevice is a 1 input to 2 USB switcher that is switched by two relays:
	/// Power Relay - Toggling cycles the device power and resets back to output 1
	/// Switch Relay - Toggling switches the output back and forth
	/// 
	/// Power is cycled so we can be sure we are synchronized with the device.
	/// </summary>
	public sealed class YkupSwitcherDevice : AbstractRouteSwitcherDevice<YkupSwitcherDeviceSettings>
	{
		/// <summary>
		/// How long to wait, in milliseconds, after power cycling the device
		/// </summary>
		private const long POWER_TIME = 1000;

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
		private readonly SafeTimer m_PowerTimer;

		private IRelayPort m_PowerPort;
		private IRelayPort m_SwitchPort;
		private int m_ExpectedOutput;

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

			m_PowerTimer = SafeTimer.Stopped(PowerTimerCallback);
			
			Controls.Add(new RouteSwitcherControl(this, 0));
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
				return true;

			m_ExpectedOutput = output;

			// Cycle power
			m_PowerPort.Open();
			m_PowerPort.Close();

			// Wait for warmup before selecting output
			m_PowerTimer.Reset(POWER_TIME);

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
		/// Called after the device powers on.
		/// </summary>
		private void PowerTimerCallback()
		{
			switch (m_ExpectedOutput)
			{
				case OUTPUT_1_ADDRESS:
					// We should be on output 1 already
					break;

				case OUTPUT_2_ADDRESS:
					// Toggle the button to switch to the second output
					SwitchPort.Close();
					SwitchPort.Open();
					break;

				default:
					throw new InvalidOperationException(string.Format("Unexpected output {0}", m_ExpectedOutput));
			}

			m_Cache.SetInputForOutput(OUTPUT_1_ADDRESS, m_ExpectedOutput == OUTPUT_1_ADDRESS ? INPUT_ADDRESS : (int?)null, CONNECTION_TYPE);
			m_Cache.SetInputForOutput(OUTPUT_2_ADDRESS, m_ExpectedOutput == OUTPUT_2_ADDRESS ? INPUT_ADDRESS : (int?)null, CONNECTION_TYPE);
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
					Log(eSeverity.Error, "No relay port with id {0}", settings.PowerPort);
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
					Log(eSeverity.Error, "No relay port with id {0}", settings.SwitchPort);
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

		#endregion
	}
}
