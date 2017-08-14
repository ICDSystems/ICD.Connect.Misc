#if SIMPLSHARP
using Crestron.SimplSharpPro.DM;
using ICD.Connect.Routing.Connections;

namespace ICD.Connect.Misc.CrestronPro.Utils
{
	public static class DmUtils
	{
		/// <summary>
		/// Gets the connection type for the given DM event. Returns None if the event does
		/// not correspond to a connection type.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static eConnectionType DmEventToConnectionType(int id)
		{
			switch (id)
			{
				case DMInputEventIds.AudioFormatEventId:
				case DMInputEventIds.AudioChannelsEventId:
				case DMInputEventIds.AudioSourceDetectedEventId:
					return eConnectionType.Audio;

				case DMInputEventIds.SourceSyncEventId:
				case DMInputEventIds.VideoDetectedEventId:
					return eConnectionType.Video;

				case DMInputEventIds.UsbRoutedToEventId:
					return eConnectionType.Usb;
			}

			return eConnectionType.None;
		}
	}
}
#endif
