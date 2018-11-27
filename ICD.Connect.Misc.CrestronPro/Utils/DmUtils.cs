using System;
using ICD.Common.Utils;
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

		/// <summary>
		/// Routes the input to the output for the given type.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="output"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static eConnectionType Route(DMInput input, DMOutput output, eConnectionType type)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			if (output == null)
				throw new ArgumentNullException("output");

			eConnectionType changed = eConnectionType.None;

			foreach (eConnectionType flag in EnumUtils.GetFlagsExceptNone(type))
			{
				switch (flag)
				{
					case eConnectionType.Audio:
						try
						{
							output.AudioOut = input;
							changed |= eConnectionType.Audio;
						}
						catch (NotSupportedException)
						{
							try
							{
								// DMPS 4K
								output.AudioOutSource = GetAudioSourceForInput((int)input.Number);
								changed |= eConnectionType.Audio;
							}
							catch (Exception)
							{
							}
						}

						break;

					case eConnectionType.Video:
						output.VideoOut = input;
						changed |= eConnectionType.Video;
						break;

					case eConnectionType.Usb:
						output.USBRoutedTo = input;
						changed |= eConnectionType.Usb;
						break;
				}
			}

			return changed;
		}

		/// <summary>
		/// Clears the output for the given type.
		/// </summary>
		/// <param name="output"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static eConnectionType ClearOutput(DMOutput output, eConnectionType type)
		{
			if (output == null)
				throw new ArgumentNullException("output");

			eConnectionType changed = eConnectionType.None;

			foreach (eConnectionType flag in EnumUtils.GetFlagsExceptNone(type))
			{
				switch (flag)
				{
					case eConnectionType.Video:
						output.VideoOut = null;
						changed |= eConnectionType.Video;
						break;

					case eConnectionType.Audio:
						try
						{
							output.AudioOut = null;
							changed |= eConnectionType.Audio;
						}
						catch (NotSupportedException)
						{
							try
							{
								// DMPS 4K
								output.AudioOutSource = GetAudioSourceForInput(null);
								changed |= eConnectionType.Audio;
							}
							catch (Exception)
							{
							}
						}

						break;

					case eConnectionType.Usb:
						output.USBRoutedTo = null;
						changed |= eConnectionType.Usb;
						break;
				}
			}

			return changed;
		}

		/// <summary>
		/// Gets the input for the given AudioOutSource value.
		/// 
		/// TODO - This does not support analog inputs
		/// </summary>
		/// <param name="audioOutSource"></param>
		/// <returns></returns>
		public static int? GetInputForAudioSource(eDmps34KAudioOutSource audioOutSource)
		{
			switch (audioOutSource)
			{
				case eDmps34KAudioOutSource.NoRoute:
					return null;

				case eDmps34KAudioOutSource.Analog1:
				case eDmps34KAudioOutSource.Analog2:
				case eDmps34KAudioOutSource.Analog3:
				case eDmps34KAudioOutSource.Analog4:
				case eDmps34KAudioOutSource.Analog5:
					return null;

				case eDmps34KAudioOutSource.Hdmi1:
					return 1;
				case eDmps34KAudioOutSource.Hdmi2:
					return 2;
				case eDmps34KAudioOutSource.Hdmi3:
					return 3;
				case eDmps34KAudioOutSource.Hdmi4:
					return 4;
				case eDmps34KAudioOutSource.Hdmi5:
					return 5;
				case eDmps34KAudioOutSource.Hdmi6:
					return 6;
				case eDmps34KAudioOutSource.Dm7:
					return 7;
				case eDmps34KAudioOutSource.Dm8:
					return 8;

				case eDmps34KAudioOutSource.AirMedia8:
				case eDmps34KAudioOutSource.AirMedia9:
					return null;

				default:
					throw new ArgumentOutOfRangeException("audioOutSource");
			}
		}

		/// <summary>
		/// Gets the AudioOutSource value for the given input.
		/// 
		/// TODO - This does not support analog inputs
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static eDmps34KAudioOutSource GetAudioSourceForInput(int? input)
		{
			switch (input)
			{
				case null:
					return eDmps34KAudioOutSource.NoRoute;
				case 1:
					return eDmps34KAudioOutSource.Hdmi1;
				case 2:
					return eDmps34KAudioOutSource.Hdmi2;
				case 3:
					return eDmps34KAudioOutSource.Hdmi3;
				case 4:
					return eDmps34KAudioOutSource.Hdmi4;
				case 5:
					return eDmps34KAudioOutSource.Hdmi5;
				case 6:
					return eDmps34KAudioOutSource.Hdmi6;
				case 7:
					return eDmps34KAudioOutSource.Dm7;
				case 8:
					return eDmps34KAudioOutSource.Dm8;

				default:
					throw new ArgumentOutOfRangeException("input");
			}
		}
	}
}

#endif
