using ICD.Connect.Protocol.Network.Ports.Web;
using ICD.Connect.Protocol.Network.Settings;

namespace ICD.Connect.Misc.GlobalCache.Devices.ITachFlex
{
	public sealed class GcITachFlexDevice : AbstractGcITachDevice<GcITachFlexDeviceSettings>
	{
		private readonly HttpPort m_HttpClient;

		/// <summary>
		/// Constructor.
		/// </summary>
		public GcITachFlexDevice()
		{
			m_HttpClient = new HttpPort
			{
				Name = GetType().Name
			};
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void DisposeFinal(bool disposing)
		{
			base.DisposeFinal(disposing);

			m_HttpClient.Dispose();
		}

		#region Methods

		/// <summary>
		/// Sends the data to the device.
		/// </summary>
		/// <param name="localUrl"></param>
		/// <param name="data"></param>
		public string Post(string localUrl, string data)
		{
			string result;

			m_HttpClient.UriProperties.SetUriFromAddress(Address);
			m_HttpClient.Post(localUrl, data, out result);

			return result;
		}

		#endregion

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
	}
}
