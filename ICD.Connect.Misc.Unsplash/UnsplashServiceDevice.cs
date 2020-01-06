using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Common.Utils.IO;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.Devices;
using ICD.Connect.Devices.EventArguments;
using ICD.Connect.Misc.Unsplash.Responses;
using ICD.Connect.Protocol.Extensions;
using ICD.Connect.Protocol.Network.Ports.Web;
using ICD.Connect.Protocol.Network.Settings;
using ICD.Connect.Settings;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.Unsplash
{
	public sealed class UnsplashServiceDevice : AbstractDevice<UnsplashServiceDeviceSettings>
	{
		private readonly UriProperties m_UriProperties;
		private readonly WebProxyProperties m_WebProxyProperties;

		private IWebPort m_Port;

		#region Properties

		public string ClientId { get; set; }
		public string BaseQuery { get; set; }
		public int? Width { get; set; }
		public int? Height { get; set; }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public UnsplashServiceDevice()
		{
			m_UriProperties = new UriProperties();
			m_WebProxyProperties = new WebProxyProperties();
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
			{
				port.ApplyDeviceConfiguration(m_UriProperties);
				port.ApplyDeviceConfiguration(m_WebProxyProperties);
			}
		}

		public IEnumerable<UnsplashPhotoResult> GetPictureList(params string[] query)
		{
			IEnumerable<string> baseQuery =
				BaseQuery == null
					? Enumerable.Empty<string>()
					: BaseQuery.Split();

			query = baseQuery.Concat(query).ToArray();
			string queryString = string.Join("-", query);

			UriQueryBuilder builder = new UriQueryBuilder();
			builder.Append("query", queryString);
			builder.Append("client_id", ClientId);
			
			string url = "https://api.unsplash.com/search/photos" + builder;

			WebPortResponse response = m_Port.Get(url);

			if (response.Success)
				return JsonConvert.DeserializeObject<UnsplashPhotoListViewResponse>(response.DataAsString).Results;

			throw new Exception(string.Format("Failed to get picture list - {0}", response.DataAsString));
		}

		public UnsplashPhotoResult GetPicture(string id)
		{
			string url = string.Format("https://api.unsplash.com/photos/{0}?client_id={1}", id, ClientId);

			WebPortResponse response = m_Port.Get(url);

			if (!response.Success)
				throw new Exception(string.Format("Failed to find picture - {0}", response.DataAsString));

			return JsonConvert.DeserializeObject<UnsplashPhotoResult>(response.DataAsString);
		}

		/// <summary>
		/// Downloads the image with the given id to the web server directory and returns the new URL.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public string DownloadPicture(string id)
		{
			// Get the photo result for the id
			UnsplashPhotoResult picture = GetPicture(id);
			string url = picture.Urls.Raw;

			// Replace the query
			url = url.Split('?').First();
			UriQueryBuilder builder = new UriQueryBuilder();

			if (Width.HasValue)
				builder.Append("w", Width.ToString());
			if (Height.HasValue)
				builder.Append("h", Height.ToString());
			if (Width.HasValue || Height.HasValue)
			{ 
				builder.Append("fit", "crop");
				builder.Append("crop", "entropy");
			}

			url += builder;

			// Get the byte array for the photo
			WebPortResponse response = m_Port.Get(url);
			if (!response.Success)
				throw new Exception(string.Format("Failed to download picture - {0}", response.DataAsString));
			
			byte[] photo = response.Data;

			// Write the byte array to /HTML/Unsplash/<id>.jpeg
			string path = PathUtils.GetWebServerPath("Unsplash", string.Format("{0}.jpeg", id));
			string directory = IcdPath.GetDirectoryName(path);
			IcdDirectory.CreateDirectory(directory);
			IcdFile.WriteAllBytes(path, photo);

			// Return the path to <host>/Unsplash/<id>.jpeg
			return PathUtils.GetUrl(path);
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

			m_UriProperties.Copy(settings);
			m_WebProxyProperties.Copy(settings);

			ClientId = settings.ClientId;
			BaseQuery = settings.BaseQuery;
			Width = settings.Width;
			Height = settings.Height;

			IWebPort port = null;

			if (settings.Port != null)
			{
				try
				{
					port = factory.GetPortById((int)settings.Port) as IWebPort;
				}
				catch (KeyNotFoundException)
				{
					Log(eSeverity.Error, "No web port with id {0}", settings.Port);
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
			BaseQuery = null;
			Width = null;
			Height = null;

			m_UriProperties.ClearUriProperties();
			m_WebProxyProperties.ClearProxyProperties();
		}

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(UnsplashServiceDeviceSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.ClientId = ClientId;
			settings.BaseQuery = BaseQuery;
			settings.Width = Width;
			settings.Height = Height;

			settings.Port = m_Port == null ? (int?)null : m_Port.Id;

			settings.Copy(m_UriProperties);
			settings.Copy(m_WebProxyProperties);
		}

		#endregion
		#region Console
		public override string ConsoleHelp { get { return "The Unsplash service device"; } }

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			yield return new GenericConsoleCommand<string>("GetPictureList", "", q => string.Join(", ", GetPictureList(q).Select(p => p.Id).ToArray()));
			yield return new GenericConsoleCommand<string>("GetPicture", "Returns information of specific picture.", q => GetPicture(q));
			yield return new GenericConsoleCommand<string>("DownloadPicture", "Downloads a specific picture.", q => DownloadPicture(q));
			
		}

		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		#endregion

	}
}

