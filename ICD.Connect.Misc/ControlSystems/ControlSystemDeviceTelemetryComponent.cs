using System;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Connect.Devices.Telemetry.DeviceInfo;

namespace ICD.Connect.Misc.ControlSystems
{
	internal sealed class ControlSystemDeviceTelemetryComponent
	{
		private readonly IControlSystemDevice m_ControlSystem;

		internal ControlSystemDeviceTelemetryComponent([NotNull] IControlSystemDevice controlSystem)
		{
			if (controlSystem == null)
				throw new ArgumentNullException("controlSystem");

			m_ControlSystem = controlSystem;

			IcdEnvironment.OnEthernetEvent += IcdEnvironmentOnEthernetEvent;

			UpdateEthernetInfo();
			UpdateDeviceInfo();
		}

		private void UpdateDeviceInfo()
		{
			m_ControlSystem.MonitoredDeviceInfo.Model = ProcessorUtils.ModelName;
			m_ControlSystem.MonitoredDeviceInfo.UptimeStart = ProcessorUtils.GetSystemStartTime();
			m_ControlSystem.MonitoredDeviceInfo.FirmwareVersion = ProcessorUtils.ModelVersion;
			m_ControlSystem.MonitoredDeviceInfo.FirmwareDate = ProcessorUtils.ModelVersionDate;
			m_ControlSystem.MonitoredDeviceInfo.SerialNumber = ProcessorUtils.ProcessorSerialNumber;
		}

		private void IcdEnvironmentOnEthernetEvent(IcdEnvironment.eEthernetAdapterType adapter, IcdEnvironment.eEthernetEventType type)
		{
			UpdateEthernetInfo();
		}

		private void UpdateEthernetInfo()
		{
			// Hostname only applies for the overall device, not per-adapter
			m_ControlSystem.MonitoredDeviceInfo.NetworkInfo.Hostname = IcdEnvironment.Hostnames.FirstOrDefault();
			
			// We currently only get DHCP status for the first adapter
			// todo: Have IcdEnvironment return a collection of adapters with all relevant info
			m_ControlSystem.MonitoredDeviceInfo.NetworkInfo.Adapters.GetOrAddAdapter(1).Dhcp = IcdEnvironment.DhcpStatus;
			
			
			// Set IPv4 Address per adapter
			// Network Adapter Addresses start at 1 by convention
			int i = 1;
			foreach (var ipv4Address in IcdEnvironment.NetworkAddresses)
			{
				m_ControlSystem.MonitoredDeviceInfo.NetworkInfo.Adapters.GetOrAddAdapter(i).Ipv4Address = ipv4Address;
				i++;
			}

			// Set Mac Address per adapter
			i = 1;
			foreach (var macAddress in IcdEnvironment.MacAddresses)
			{
				IcdPhysicalAddress mac;
				IcdPhysicalAddress.TryParse(macAddress, out mac);

				m_ControlSystem.MonitoredDeviceInfo.NetworkInfo.Adapters.GetOrAddAdapter(i).MacAddress = mac;
				i++;
			}
		}
	}
}