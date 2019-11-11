using System;
using ICD.Common.Utils.EventArguments;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components
{
	public sealed class MuteComponent : AbstractVibeComponent
	{
		public event EventHandler<MuteChangedEventArgs> OnMuteChanged;

		private const string COMMAND = "mute";
		private const string PARAM_GET_MUTE = "-l";
		private const string PARAM_MUTE_ON = "on";
		private const string PARAM_MUTE_OFF = "off";

		public MuteComponent(VibeBoard parent) : base(parent)
		{
		}

		public void GetCurrentMute()
		{
			Parent.SendCommand(new VibeCommand(COMMAND, PARAM_GET_MUTE));
		}

		public void SetMute(bool mute)
		{
			Parent.SendCommand(new VibeCommand(COMMAND, mute ? PARAM_MUTE_ON : PARAM_MUTE_OFF));
		}

		public void MuteOn()
		{
			SetMute(true);
		}

		public void MuteOff()
		{
			SetMute(false);
		}
	}

	public sealed class MuteChangedEventArgs : GenericEventArgs<bool>
	{
		public MuteChangedEventArgs(bool data) : base(data)
		{
		}
	}
}
