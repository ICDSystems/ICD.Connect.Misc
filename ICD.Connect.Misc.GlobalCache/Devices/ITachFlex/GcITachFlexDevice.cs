using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Services.Logging;
using ICD.Common.Utils.Timers;
using ICD.Connect.Protocol.Network.Ports.Web;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.GlobalCache.Devices.ITachFlex
{
	public sealed class GcITachFlexDevice : AbstractGcITachDevice<GcITachFlexDeviceSettings>
	{
		#region Members

		/// <summary>
		/// Poll for version & network telemetry every hour.
		/// </summary>
		private const long TELEMETRY_POLLING_INTERVAL = 1 * 60 * 60 * 1000;

		private const string GET_VERSION_LOCAL_URL = "api/v1/version";
		private const string GET_NETWORK_LOCAL_URL = "api/v1/network";

		private readonly HttpPort m_HttpClient;
		private readonly SafeTimer m_TelemetryPollingTimer;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public GcITachFlexDevice()
		{
			m_HttpClient = new HttpPort
			{
				Name = GetType().Name
			};

			m_TelemetryPollingTimer = SafeTimer.Stopped(() => PollTelemetry());
		}

		#endregion

		#region Methods

		/// <summary>
		/// Release resources.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void DisposeFinal(bool disposing)
		{
			base.DisposeFinal(disposing);

			m_HttpClient.Dispose();
		}

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			bool tcp = base.GetIsOnlineStatus();
			bool http = m_HttpClient != null && m_HttpClient.IsOnline;

			return tcp && http;
		}

		protected override void StartSettingsFinal()
		{
			base.StartSettingsFinal();

			m_TelemetryPollingTimer.Reset(0, TELEMETRY_POLLING_INTERVAL);
		}

		/// <summary>
		/// Sends the data to the device.
		/// </summary>
		/// <param name="localUrl"></param>
		/// <param name="data"></param>
		public string Post(string localUrl, string data)
		{
			m_HttpClient.Uri = new Uri(Address, UriKind.RelativeOrAbsolute);
			WebPortResponse response = m_HttpClient.Post(localUrl, StringUtils.ToBytes(data));
			return response.DataAsString;
		}

		/// <summary>
		/// Sends a get request to the device.
		/// </summary>
		/// <param name="localUrl"></param>
		/// <returns></returns>
		public string Get(string localUrl)
		{
			m_HttpClient.Uri = new Uri(Address, UriKind.RelativeOrAbsolute);
			WebPortResponse response = m_HttpClient.Get(localUrl);
			return response.DataAsString;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Polls the device for version & network telemetry.
		/// </summary>
		private void PollTelemetry()
		{
			string versionResponse;
			string networkResponse;

			try
			{
				versionResponse = Get(GET_VERSION_LOCAL_URL);
				networkResponse = Get(GET_NETWORK_LOCAL_URL);
			}
			catch (Exception e)
			{
				Logger.Log(eSeverity.Error, "Failed to poll Global Caché flex device telemetry - {0}", e);
				return;
			}

			FlexApi.RestApi.Version version = JsonConvert.DeserializeObject<FlexApi.RestApi.Version>(versionResponse);
			FlexApi.RestApi.Network network = JsonConvert.DeserializeObject<FlexApi.RestApi.Network>(networkResponse);

			UpdateVersion(version);
			UpdateNetwork(network);
		}

		/// <summary>
		/// Updates the device's monitored version info.
		/// </summary>
		/// <param name="versionResponse"></param>
		private void UpdateVersion(FlexApi.RestApi.Version versionResponse)
		{
			MonitoredDeviceInfo.Make = versionResponse.Make;
			MonitoredDeviceInfo.Model = versionResponse.Model;
			MonitoredDeviceInfo.FirmwareVersion = versionResponse.FirmwareVersion;
		}

		/// <summary>
		/// Updates the device's monitored network info.
		/// </summary>
		/// <param name="networkResponse"></param>
		private void UpdateNetwork(FlexApi.RestApi.Network networkResponse)
		{
			MonitoredDeviceInfo.NetworkInfo.Adapters.GetOrAddAdapter(1).Dhcp = networkResponse.Dhcp;
			MonitoredDeviceInfo.NetworkInfo.Adapters.GetOrAddAdapter(1).Ipv4Gateway = networkResponse.Gateway;
			MonitoredDeviceInfo.NetworkInfo.Adapters.GetOrAddAdapter(1).Ipv4Address = networkResponse.IpAddress;
			MonitoredDeviceInfo.NetworkInfo.Adapters.GetOrAddAdapter(1).Ipv4SubnetMask = networkResponse.SubnetMask;
			MonitoredDeviceInfo.NetworkInfo.Dns = networkResponse.PrimaryDnsServer;
		}

		#endregion
	}
}
