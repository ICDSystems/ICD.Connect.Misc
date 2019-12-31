using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Audio.Controls.Mute;
using ICD.Connect.Audio.Controls.Volume;
using ICD.Connect.Audio.EventArguments;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Controls
{
	public sealed class VibeBoardVolumeControl : AbstractVolumeLevelDeviceControl<VibeBoard>, IVolumeMuteFeedbackDeviceControl
	{
		public event EventHandler<MuteDeviceMuteStateChangedApiEventArgs> OnMuteStateChanged;

		private VolumeComponent m_VolumeComponent;
		private MuteComponent m_MuteComponent;

		private int m_VolumeLevel;
		private bool m_IsMuted;

		#region Properties

		public override float VolumeLevel { get { return m_VolumeLevel; } }

		public bool VolumeIsMuted
		{
			get { return m_IsMuted; }
			private set
			{
				if (m_IsMuted == value)
					return;

				m_IsMuted = value;
				OnMuteStateChanged.Raise(this, new MuteDeviceMuteStateChangedApiEventArgs(value));
			}
		}

		protected override float VolumeRawMinAbsolute { get { return 0; } }

		protected override float VolumeRawMaxAbsolute { get { return 100; } }

		#endregion

		public VibeBoardVolumeControl(VibeBoard parent, int id) : base(parent, id)
		{
		}

		public override void SetVolumeLevel(float volume)
		{
			if (m_VolumeComponent == null)
				return;
			
			m_VolumeComponent.SetVolume((int)volume);
		}
		

		public void VolumeMuteToggle()
		{
			if (m_MuteComponent == null)
				return;

			m_MuteComponent.SetMute(!VolumeIsMuted);
		}

		public void SetVolumeMute(bool mute)
		{
			if (m_MuteComponent == null)
				return;

			m_MuteComponent.SetMute(mute);
		}

		#region Component Callbacks
		protected override void Subscribe(VibeBoard parent)
		{
			base.Subscribe(parent);
			
			m_VolumeComponent = parent == null ? null : parent.Components.GetComponent<VolumeComponent>();
			if (m_VolumeComponent != null) 
				m_VolumeComponent.OnVolumeChanged += VolumeComponentOnVolumeChanged;

			m_MuteComponent = parent == null ? null : parent.Components.GetComponent<MuteComponent>();
			if (m_MuteComponent != null)
				m_MuteComponent.OnMuteChanged += MuteComponentOnMuteChanged;
		}

		protected override void Unsubscribe(VibeBoard parent)
		{
			base.Subscribe(parent);
			
			if (m_VolumeComponent != null)
				m_VolumeComponent.OnVolumeChanged -= VolumeComponentOnVolumeChanged;
			m_VolumeComponent = null;

			if (m_MuteComponent != null)
				m_MuteComponent.OnMuteChanged -= MuteComponentOnMuteChanged;
			m_MuteComponent = null;
		}

		private void VolumeComponentOnVolumeChanged(object sender, VolumeChangedEventArgs e)
		{
			m_VolumeLevel = e.Data;
			VolumeFeedback(m_VolumeLevel);
		}

		private void MuteComponentOnMuteChanged(object sender, MuteChangedEventArgs e)
		{
			VolumeIsMuted = e.Data;
		}

		#endregion
	}
}
