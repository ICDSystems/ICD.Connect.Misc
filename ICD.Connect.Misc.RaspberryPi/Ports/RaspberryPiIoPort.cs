using System;
using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Protocol.Ports.IoPort;
using ICD.Connect.Settings.Core;
using RaspberrySharp.IO.GeneralPurpose;

namespace ICD.Connect.Misc.RaspberryPi.Ports
{
	public sealed class RaspberryPiIoPort : AbstractIoPort<RaspberryPiIoPortSettings>
	{
		private GpioConnection m_Connection;
		private int m_Pin;

		#region Methods

		/// <summary>
		/// Sets the configuration mode.
		/// </summary>
		public override void SetConfiguration(eIoPortConfiguration configuration)
		{
			switch (configuration)
			{
				case eIoPortConfiguration.DigitalIn:
				case eIoPortConfiguration.DigitalOut:
					break;

				default:
					throw new NotSupportedException("configuration");
			}

			SetPin(m_Pin, configuration);
		}

		/// <summary>
		/// Sets the pin address.
		/// </summary>
		/// <param name="pin"></param>
		public void SetPin(int pin)
		{
			if (!EnumUtils.IsDefined(typeof(ProcessorPin), pin))
				throw new ArgumentOutOfRangeException("pin");

			SetPin(pin, Configuration);
		}

		/// <summary>
		/// Sets the pin address and configuration.
		/// </summary>
		/// <param name="pin"></param>
		/// <param name="configuration"></param>
		private void SetPin(int pin, eIoPortConfiguration configuration)
		{
			if (pin == m_Pin && configuration == Configuration)
				return;

			m_Pin = pin;
			Configuration = configuration;

			RebuildConnection();
		}

		/// <summary>
		/// Sets the digital output state.
		/// </summary>
		/// <param name="digitalOut"></param>
		public override void SetDigitalOut(bool digitalOut)
		{
			if (digitalOut)
				m_Connection.Open();
			else
				m_Connection.Close();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return m_Connection != null;
		}

		/// <summary>
		/// Destroys the old connection and makes a new connection based on the
		/// pin and configuration info.
		/// </summary>
		private void RebuildConnection()
		{
			Unscubscribe(m_Connection);

			IDisposable disposable = m_Connection;
			if (disposable != null)
				disposable.Dispose();

			m_Connection = null;

			if (m_Pin == 0 || Configuration == eIoPortConfiguration.None)
				return;

			ProcessorPin pin = (ProcessorPin)m_Pin;
			PinConfiguration pinConfiguration = BuildPinConfiguration(Configuration, pin);

			m_Connection = new GpioConnection(pinConfiguration);
			Subscribe(m_Connection);
		}

		/// <summary>
		/// Instantiates a pin configuration for the given configuration and pin address.
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="pin"></param>
		/// <returns></returns>
		private static PinConfiguration BuildPinConfiguration(eIoPortConfiguration configuration, ProcessorPin pin)
		{
			switch (configuration)
			{
				case eIoPortConfiguration.DigitalIn:
					InputPinConfiguration input = pin.Input();
					input.Resistor = PinResistor.PullUp;
					return input;

				case eIoPortConfiguration.DigitalOut:
					OutputPinConfiguration output = pin.Output().Enable();
					return output;

				default:
					throw new ArgumentOutOfRangeException(nameof(configuration), configuration, null);
			}
		}

		#endregion

		#region Settings

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			SetPin(0);
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(RaspberryPiIoPortSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Pin = m_Pin;
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(RaspberryPiIoPortSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			SetPin(settings.Pin);
		}

		#endregion

		#region Connection Callbacks

		/// <summary>
		/// Subscribe to the connection events.
		/// </summary>
		/// <param name="connection"></param>
		private void Subscribe(GpioConnection connection)
		{
			if (connection == null)
				return;

			connection.PinStatusChanged += ConnectionOnPinStatusChanged;
		}

		/// <summary>
		/// Unsubscribe from the connection events.
		/// </summary>
		/// <param name="connection"></param>
		private void Unscubscribe(GpioConnection connection)
		{
			if (connection == null)
				return;

			connection.PinStatusChanged -= ConnectionOnPinStatusChanged;
		}

		/// <summary>
		/// Called when a connection pin status changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="pinStatusEventArgs"></param>
		private void ConnectionOnPinStatusChanged(object sender, PinStatusEventArgs pinStatusEventArgs)
		{
			switch (Configuration)
			{
				case eIoPortConfiguration.DigitalIn:
					DigitalIn = !pinStatusEventArgs.Enabled;
					break;
				case eIoPortConfiguration.DigitalOut:
					DigitalOut = pinStatusEventArgs.Enabled;
					break;
			}
		}

		#endregion

		#region Console

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			addRow("Pin", m_Pin);
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			yield return new GenericConsoleCommand<int>("SetPin", "SetPin <NUMBER>", i => SetPin(i));
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		#endregion
	}
}
