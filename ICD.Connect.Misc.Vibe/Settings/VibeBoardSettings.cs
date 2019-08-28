using ICD.Connect.Displays.Settings;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard;
using ICD.Connect.Protocol.Network.Settings;
using ICD.Connect.Protocol.Settings;
using ICD.Connect.Settings.Attributes;
using System;

namespace ICD.Connect.Misc.Vibe.Settings
{
	[KrangSettings("VibeBoard", typeof(VibeBoard))]
	public class VibeBoardSettings : AbstractDisplayWithAudioSettings
	{
		protected override void UpdateNetworkDefaults(SecureNetworkProperties networkProperties)
		{
			networkProperties.ApplyDefaultValues(null, 2222, "root", "123456");
		}

		protected override void UpdateComSpecDefaults(ComSpecProperties comSpecProperties)
		{
			// vibe does not have a serial port
		}
	}
}