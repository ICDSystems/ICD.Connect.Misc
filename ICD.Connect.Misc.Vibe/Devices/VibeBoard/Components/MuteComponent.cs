using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components
{
	public sealed class MuteComponent : AbstractVibeComponent
	{
		public event EventHandler<MuteChangedEventArgs> OnMuteChanged;

		private const string COMMAND = "mute";
		private const string PARAM_GET_MUTE = "-l";
		private const string PARAM_MUTE_ON = "on";
		private const string PARAM_MUTE_OFF = "off";

		private bool m_Mute;

		public bool Mute
		{
			get { return m_Mute; }
			private set
			{
				if (m_Mute == value)
					return;

				m_Mute = value;
				OnMuteChanged.Raise(this, new MuteChangedEventArgs(value));
			}
		}

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

		#region Parent Callbacks

		protected override void Subscribe(VibeBoard vibe)
		{
			base.Subscribe(vibe);

			vibe.ResponseHandler.RegisterResponseCallback<MuteResponse>(response => Mute = response.Value.IsMute);
		}

		#endregion
	}

	public sealed class MuteChangedEventArgs : GenericEventArgs<bool>
	{
		public MuteChangedEventArgs(bool data) : base(data)
		{
		}
	}
}
