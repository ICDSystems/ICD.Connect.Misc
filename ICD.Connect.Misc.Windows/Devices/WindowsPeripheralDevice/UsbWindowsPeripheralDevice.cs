using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Timers;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using Microsoft.Management.Infrastructure;

namespace ICD.Connect.Misc.Windows.Devices.WindowsPeripheralDevice
{
	/// <summary>
	/// Represents a USB peripheral device
	/// </summary>
	public sealed class UsbWindowsPeripheralDevice : AbstractWindowsPeripheralDevice<UsbWindowsPeripheralDeviceSettings>
	{
		private const long UPDATE_TIMER_INTERVAL = 250;
		private const long UPDATE_TIMER_INTERVAL_PNP_DRIVER = 2 * 1000;
		private const int PNP_DRIVER_UPDATE_MAX_TRIES = 120;

		/// <summary>
		/// Timer to update the CIM Instances
		/// Allows WMI to catch up, particularly with the PNP Driver instances,
		/// which can take a long time to update
		/// </summary>
		private readonly SafeTimer m_UpdateTimer;

		private CimSession m_Session;
		private CimInstance m_PnpSignedDriverInstance;
		private CimInstance m_HubDriverInstance;

		#region Properties

		/// <summary>
		/// Counter for the number of retires in getting the PNP driver instance
		/// when we already have the Hub instance
		/// </summary>
		private int PnpDriverUpdateTry { get; set; }

		/// <summary>
		/// Lazy-loads the CimSession.
		/// </summary>
		private CimSession Session
		{
			get { return m_Session; }
			set
			{
				if (value == m_Session)
					return;

				m_Session?.Dispose();
				m_Session = value;
			}
		}

		/// <summary>
		/// The CIM instance of the PNP drier
		/// This contains most of the info we actually want for telemetry
		/// </summary>
		private CimInstance PnpSignedDriverInstance
		{
			get { return m_PnpSignedDriverInstance; }
			set
			{
				if (value == m_PnpSignedDriverInstance)
					return;

				m_PnpSignedDriverInstance?.Dispose();
				m_PnpSignedDriverInstance = value;
			}
		}

		/// <summary>
		/// The CIM instance of the USB Hub device
		/// This is used to track connected/disconnect state more accurately, since PNP driver object updates are slooowwww
		/// </summary>
		private CimInstance HubDriverInstance
		{
			get { return m_HubDriverInstance; }
			set
			{
				if (value == m_HubDriverInstance)
					return;

				m_HubDriverInstance?.Dispose();
				m_HubDriverInstance = value;
			}
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public UsbWindowsPeripheralDevice()
		{
			m_UpdateTimer = SafeTimer.Stopped(UpdateTimerCallback);

			IcdEnvironment.OnSystemDeviceAddedRemoved += IcdEnvironmentOnSystemDeviceAddedRemoved;
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			base.DisposeFinal(disposing);

			IcdEnvironment.OnSystemDeviceAddedRemoved -= IcdEnvironmentOnSystemDeviceAddedRemoved;

			Session = null;
			PnpSignedDriverInstance = null;
			HubDriverInstance = null;
		}

		#region Methods

		/// <summary>
		/// Updates the CIM instance.
		/// </summary>
		/// <remarks>Runs on a new thread to avoid blocking.</remarks>
		public void UpdateCimInstance()
		{
			m_UpdateTimer.Trigger();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return HubDriverInstance != null;
		}

		/// <summary>
		/// This pulls new CIM objects from windows management, and updates properties and telemetry as necessary
		/// </summary>
		private void UpdateTimerCallback()
		{
			if (string.IsNullOrEmpty(DeviceId))
			{
				HubDriverInstance= null;
				PnpSignedDriverInstance = null;
				PnpDriverUpdateTry = 0;

				UpdateCachedOnlineStatus();
				UpdateTelemetry();

				return;
			}

			string deviceId = string.IsNullOrEmpty(DeviceId) ? null : DeviceId.Replace(@"\", @"\\");
			string pnpQuery = @"Select * From Win32_PnPSignedDriver where DeviceID=""" + deviceId + @"""";
			string hubQuery = @"Select * From Win32_USBHub where DeviceID=""" + deviceId + @"""";

			Session = Session ?? CimSession.Create(null);

			HubDriverInstance = Session.QueryInstances(@"root\cimv2", "WQL", hubQuery).FirstOrDefault();
			
			if (HubDriverInstance == null)
			{
				// Since the PnpDriverInstance takes a while to update, go ahead and clear it if the hub instance is null
				PnpSignedDriverInstance = null;
				PnpDriverUpdateTry = 0;
			}
			else
			{
				CimInstance pnpDriver = Session.QueryInstances(@"root\cimv2", "WQL", pnpQuery).FirstOrDefault();
				
				// If we didn't successfully get the PNP Driver CMI Instance, wait a few seconds and try again, unless we've hit the retry limit
				if (pnpDriver == null)
				{
					if (PnpDriverUpdateTry < PNP_DRIVER_UPDATE_MAX_TRIES)
					{
						PnpDriverUpdateTry++;
						m_UpdateTimer.Reset(UPDATE_TIMER_INTERVAL_PNP_DRIVER);
					}
					else
					{
						Logger.Log(eSeverity.Error, "Couldn't get CIM Instance for PNP Driver for device {0}", DeviceId);
						PnpDriverUpdateTry = 0;
					}
				}
				else
				{
					PnpDriverUpdateTry = 0;
				}

				PnpSignedDriverInstance = pnpDriver;
			}

			UpdateCachedOnlineStatus();
			UpdateTelemetry();
		}

		/// <summary>
		/// Called when a device is added or removed from the system
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void IcdEnvironmentOnSystemDeviceAddedRemoved(object sender, BoolEventArgs args)
		{
			// Wait briefly to update so WMI can catch up
			m_UpdateTimer.Reset(UPDATE_TIMER_INTERVAL);
		}

		#endregion

		#region Telemetry

		/// <summary>
		/// Updates the telemetry for the device
		/// </summary>
		private void UpdateTelemetry()
		{
			MonitoredDeviceInfo.FirmwareVersion = PnpSignedDriverInstance?.CimInstanceProperties["DriverVersion"]?.Value?.ToString();
			MonitoredDeviceInfo.FirmwareDate = PnpSignedDriverInstance?.CimInstanceProperties["DriverDate"]?.Value as DateTime?;
			MonitoredDeviceInfo.Make = PnpSignedDriverInstance?.CimInstanceProperties["Manufacturer"]?.Value?.ToString();
			MonitoredDeviceInfo.Model = PnpSignedDriverInstance?.CimInstanceProperties["DeviceName"]?.Value?.ToString();
		}

		#endregion

		#region Console

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			yield return new ConsoleCommand("UpdateInstance", "Updates CIM Instances", () => UpdateCimInstance());
			yield return new ConsoleCommand("ListDriverProperties", "Lists the properties for the CIM instance",
			                                () => ListDriverProperties());
			yield return new ConsoleCommand("ListHubProperties", "Lists the properties for the CIM instance",
			                                () => ListHubProperties());
		}

		/// <summary>
		/// Prints out all the properties of the PNP Driver Instance CIM object
		/// </summary>
		/// <returns></returns>
		private string ListDriverProperties()
		{
			if (PnpSignedDriverInstance == null)
				return "CIM Instance is null";

			TableBuilder table = new TableBuilder("Name", "Value");
			foreach (CimProperty property in PnpSignedDriverInstance.CimInstanceProperties)
				table.AddRow(property.Name, property.Value?.ToString());

			return table.ToString();
		}

		/// <summary>
		/// Prints out all the properties of the Hub Driver Instance CIM object
		/// </summary>
		/// <returns></returns>
		private string ListHubProperties()
		{
			if (HubDriverInstance == null)
				return "CIM Instance is null";

			TableBuilder table = new TableBuilder("Name", "Value");
			foreach (CimProperty property in HubDriverInstance.CimInstanceProperties)
				table.AddRow(property.Name, property.Value?.ToString());

			return table.ToString();
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
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

			addRow("DeviceID", DeviceId);
		}

		#endregion

		#region Settings

		/// <summary>
		/// Override to add actions on StartSettings
		/// This should be used to start communications with devices and perform initial actions
		/// </summary>
		protected override void StartSettingsFinal()
		{
			base.StartSettingsFinal();

			UpdateCimInstance();
		}

		#endregion
	}
}
