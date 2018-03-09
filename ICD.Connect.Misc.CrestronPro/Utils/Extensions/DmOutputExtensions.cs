#if SIMPLSHARP
using System;
using Crestron.SimplSharpPro.DM;
using ICD.Common.Properties;

namespace ICD.Connect.Misc.CrestronPro.Utils.Extensions
{
	/// <summary>
	/// Extension methods for use with DMOutputs.
	/// </summary>
	public static class DmOutputExtensions
	{
		/// <summary>
		/// Currently there is an issue where DMOutput.AudioOutFeedback will throw an IndexOutOfRangeException.
		/// http://www.crestronlabs.com/showthread.php?13713
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[CanBeNull]
		public static DMInput GetSafeAudioOutFeedback(this DMOutput extends)
		{
			try
			{
				return extends.AudioOutFeedback;
			}
			catch (IndexOutOfRangeException)
			{
				return null;
			}
		}

		/// <summary>
		/// Related to GetSafeAudioOutFeedback. It's possible VideoOutFeedback may cause the same problem.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[CanBeNull]
		public static DMInput GetSafeVideoOutFeedback(this DMOutput extends)
		{
			try
			{
				return extends.VideoOutFeedback;
			}
			catch (IndexOutOfRangeException)
			{
				return null;
			}
		}

		/// <summary>
		/// Related to GetSafeAudioOutFeedback. It's possible USBRoutedToFeedback may cause the same problem.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		[CanBeNull]
		public static DMInputOutputBase GetSafeUsbRoutedToFeedback(this DMOutput extends)
		{
			try
			{
				return extends.USBRoutedToFeedback;
			}
			catch (IndexOutOfRangeException)
			{
				return null;
			}
		}
	}
}

#endif
