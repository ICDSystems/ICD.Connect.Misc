using System;
using System.Linq;
using ICD.Common.Utils;
using ICD.Connect.API.Nodes;
using ICD.Connect.Protocol.Ports.IoPort;
using ICD.Connect.Settings.Core;
using RaspberrySharp.IO.GeneralPurpose;

namespace ICD.Connect.Misc.RaspberryPi.Ports
{
	public sealed class RaspberryPiIoPort : AbstractIoPort<RaspberryPiIoPortSettings>
	{
		private readonly GpioConnection m_Connection;

		private int m_Pin;

		/// <summary>
		/// Gets the configured processor pin enum.
		/// </summary>
		private ProcessorPin ProcessorPin { get { return (ProcessorPin)m_Pin; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		public RaspberryPiIoPort()
		{
			m_Connection = new GpioConnection();
			m_Connection.PinStatusChanged += ConnectionOnPinStatusChanged;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			base.DisposeFinal(disposing);

			m_Connection.PinStatusChanged -= ConnectionOnPinStatusChanged;

			(m_Connection as IDisposable).Dispose();
		}

		#region Methods

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return m_Connection.Pins.Any();
		}

		/// <summary>
		/// Sets the configuration mode.
		/// </summary>
		public override void SetConfiguration(eIoPortConfiguration configuration)
		{
			// Clear the existing pins
			foreach (ConnectedPin pin in m_Connection.Pins)
				m_Connection.Remove(pin.Configuration);

			switch (configuration)
			{
				case eIoPortConfiguration.DigitalIn:
					InputPinConfiguration inputPin = new InputPinConfiguration(ProcessorPin);
					m_Connection.Add(inputPin);
					break;

				case eIoPortConfiguration.DigitalOut:
					OutputPinConfiguration outputPin = new OutputPinConfiguration(ProcessorPin);
					m_Connection.Add(outputPin);
					break;

				case eIoPortConfiguration.AnalogIn:
					throw new NotImplementedException();
					break;

				default:
					throw new ArgumentOutOfRangeException("configuration");
			}
		}

		/// <summary>
		/// Sets the digital output state.
		/// </summary>
		/// <param name="digitalOut"></param>
		public override void SetDigitalOut(bool digitalOut)
		{
			if (digitalOut)
				m_Connection.Close();
			else
				m_Connection.Open();
		}

		#endregion

		#region Settings

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			m_Pin = 0;
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

			m_Pin = settings.Pin;
		}

		#endregion

		#region Connection Callbacks

		private void ConnectionOnPinStatusChanged(object sender, PinStatusEventArgs pinStatusEventArgs)
		{
			IcdConsole.PrintLine(eConsoleColor.Magenta, "{0}, {1}", pinStatusEventArgs.Configuration,
								 pinStatusEventArgs.Enabled);
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

		#endregion
	}
}
