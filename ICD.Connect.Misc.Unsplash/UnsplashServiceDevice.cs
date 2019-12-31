using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.Devices;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.Misc.Unsplash_NetStandard.Responses;
using ICD.Connect.Protocol.Extensions;
using ICD.Connect.Protocol.Network.Ports.Web;
using ICD.Connect.Protocol.Network.Settings;
using ICD.Connect.Settings;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.Unsplash_NetStandard
{
	public sealed class UnsplashServiceDevice : AbstractDevice<UnsplashServiceDeviceSettings>
	{
		private readonly UriProperties m_UriProperties;

		private IWebPort m_Port;

		#region Properties

		public string ClientId { get; set; }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public UnsplashServiceDevice()
		{
			m_UriProperties = new UriProperties();
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			base.DisposeFinal(disposing);

			SetPort(null);
		}

		#region Methods

		/// <summary>
		/// Sets the port for communication with the service.
		/// </summary>
		/// <param name="port"></param>
		[PublicAPI]
		public void SetPort(IWebPort port)
		{
			if (port == m_Port)
				return;

			ConfigurePort(port);

			Unsubscribe(m_Port);

			if (port != null)
				port.Accept = "application/json";

			m_Port = port;
			Subscribe(m_Port);

			UpdateCachedOnlineStatus();
		}

		/// <summary>
		/// Configures the given port for communication with the device.
		/// </summary>
		/// <param name="port"></param>
		private void ConfigurePort(IWebPort port)
		{
			// URI
			if (port != null)
				port.ApplyDeviceConfiguration(m_UriProperties);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return m_Port != null && m_Port.IsOnline;
		}

		/// <summary>
		/// Subscribe to the port events.
		/// </summary>
		/// <param name="port"></param>
		private void Subscribe(IWebPort port)
		{
			if (port == null)
				return;

			port.OnIsOnlineStateChanged += PortOnIsOnlineStateChanged;
		}

		/// <summary>
		/// Unsubscribe from the port events.
		/// </summary>
		/// <param name="port"></param>
		private void Unsubscribe(IWebPort port)
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

		#region Settings

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(UnsplashServiceDeviceSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			ClientId = settings.ClientId;

			IWebPort port = null;

			if (settings.PortId != null)
			{
				try
				{
					port = factory.GetPortById((int)settings.PortId) as IWebPort;
				}
				catch (KeyNotFoundException)
				{
					Log(eSeverity.Error, "No web port with id {0}", settings.PortId);
				}
			}

			SetPort(port);
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			ClientId = null;

			m_UriProperties.Clear();
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(UnsplashServiceDeviceSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.ClientId = ClientId;

			settings.PortId = m_Port == null ? (int?)null : m_Port.Id;

			settings.Copy(m_UriProperties);
		}

		#endregion
		#region Console
		public override string ConsoleHelp { get { return "The Unsplash service device"; } }

		public IEnumerable<UnsplashPhotoResult> GetPictureList(string query)
		{
			string url = string.Format("https://api.unsplash.com/search/photos?query={0}", query);

			string response;
			return m_Port.Get(url, out response)
				       ? JsonConvert.DeserializeObject<UnsplashPhotoListViewResponse>(response).Results
				       : Enumerable.Empty<UnsplashPhotoResult>();
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			yield return new GenericConsoleCommand<string>("GetPictureList", "", q => string.Join(", ", GetPictureList(q).Select(p => p.Id).ToArray()));
		}

		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		/// <summary>
		/// Downloads the image with the given id and returns the path to the image on disk.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public string DownloadPicture(string id)
		{
			// TODO - IWebPort needs to return byte array and headers, not just a string

			throw new NotImplementedException();
		}
#endregion

	}
}

