using System;
using ICD.Connect.Audio.Controls.Volume;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Controls
{
	public sealed class VibeBoardVolumeControl : AbstractVolumeDeviceControl<VibeBoard>
	{
		private VolumeComponent m_VolumeComponent;
		private MuteComponent m_MuteComponent;

		#region Properties

		/// <summary>
		/// Gets the minimum supported volume level.
		/// </summary>
		public override float VolumeLevelMin { get { return 0; } }

		/// <summary>
		/// Gets the maximum supported volume level.
		/// </summary>
		public override float VolumeLevelMax { get { return 100; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="id"></param>
		public VibeBoardVolumeControl(VibeBoard parent, int id)
			: base(parent, id)
		{
			SupportedVolumeFeatures = eVolumeFeatures.Mute |
			                          eVolumeFeatures.MuteAssignment |
			                          eVolumeFeatures.MuteFeedback |
			                          eVolumeFeatures.Volume |
			                          eVolumeFeatures.VolumeAssignment |
			                          eVolumeFeatures.VolumeFeedback;
		}

		#region Methods

		/// <summary>
		/// Sets the raw volume. This will be clamped to the min/max and safety min/max.
		/// </summary>
		/// <param name="level"></param>
		public override void SetVolumeLevel(float level)
		{
			if (m_VolumeComponent == null)
				return;

			m_VolumeComponent.SetVolume((int)Math.Round(level));
		}

		/// <summary>
		/// Raises the volume one time
		/// Amount of the change varies between implementations - typically "1" raw unit
		/// </summary>
		public override void VolumeIncrement()
		{
			SetVolumeLevel(VolumeLevel + 1);
		}

		/// <summary>
		/// Lowers the volume one time
		/// Amount of the change varies between implementations - typically "1" raw unit
		/// </summary>
		public override void VolumeDecrement()
		{
			SetVolumeLevel(VolumeLevel - 1);
		}

		/// <summary>
		/// Starts ramping the volume, and continues until stop is called or the timeout is reached.
		/// If already ramping the current timeout is updated to the new timeout duration.
		/// </summary>
		/// <param name="increment">Increments the volume if true, otherwise decrements.</param>
		/// <param name="timeout"></param>
		public override void VolumeRamp(bool increment, long timeout)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Stops any current ramp up/down in progress.
		/// </summary>
		public override void VolumeRampStop()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Toggles the current mute state.
		/// </summary>
		public override void ToggleIsMuted()
		{
			if (m_MuteComponent == null)
				return;

			m_MuteComponent.SetMute(!IsMuted);
		}

		/// <summary>
		/// Sets the mute state.
		/// </summary>
		/// <param name="mute"></param>
		public override void SetIsMuted(bool mute)
		{
			if (m_MuteComponent == null)
				return;

			m_MuteComponent.SetMute(mute);
		}

		#endregion

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
			VolumeLevel = e.Data;
		}

		private void MuteComponentOnMuteChanged(object sender, MuteChangedEventArgs e)
		{
			IsMuted = e.Data;
		}

		#endregion
	}
}
