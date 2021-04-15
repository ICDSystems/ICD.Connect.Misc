using System;
using ICD.Connect.Audio.Controls.Volume;
#if NETSTANDARD
using ICD.Common.Properties;
using ICD.Common.Utils;
using NAudio.CoreAudioApi;
#endif

namespace ICD.Connect.Misc.Windows.Devices.ControlSystems
{
	public sealed class WindowsControlSystemMasterVolumeControl : AbstractVolumeDeviceControl<WindowsControlSystem>
	{
#if NETSTANDARD
		[CanBeNull]
		private MMDevice m_MasterVolumeEndpoint;
#endif

		#region Properties

		/// <summary>
		/// Gets the minimum supported volume level.
		/// </summary>
		public override float VolumeLevelMin { get { return 0; } }

		/// <summary>
		/// Gets the maximum supported volume level.
		/// </summary>
		public override float VolumeLevelMax { get { return 1; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="id"></param>
		public WindowsControlSystemMasterVolumeControl(WindowsControlSystem parent, int id)
			: base(parent, id)
		{
#if NETSTANDARD
			SupportedVolumeFeatures = eVolumeFeatures.Mute |
			                          eVolumeFeatures.MuteAssignment |
			                          eVolumeFeatures.MuteFeedback |
			                          eVolumeFeatures.Volume |
			                          eVolumeFeatures.VolumeAssignment |
			                          eVolumeFeatures.VolumeFeedback;

			using (MMDeviceEnumerator devices = new MMDeviceEnumerator())
				m_MasterVolumeEndpoint = devices.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
			Subscribe(m_MasterVolumeEndpoint);

			UpdateAudioFeedback();
#endif
		}

#if NETSTANDARD
		/// <summary>
		/// Override to release resources.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void DisposeFinal(bool disposing)
		{
			base.DisposeFinal(disposing);

			Unsubscribe(m_MasterVolumeEndpoint);

			if (m_MasterVolumeEndpoint != null)
				m_MasterVolumeEndpoint.Dispose();
			m_MasterVolumeEndpoint = null;
		}
#endif

		#region Methods

		/// <summary>
		/// Sets the mute state.
		/// </summary>
		/// <param name="mute"></param>
		public override void SetIsMuted(bool mute)
		{
#if SIMPLSHARP
			throw new NotSupportedException();
#else
			if (m_MasterVolumeEndpoint == null)
				throw new InvalidOperationException("No internal audio endpoint");

			m_MasterVolumeEndpoint.AudioEndpointVolume.Mute = mute;
#endif
		}

		/// <summary>
		/// Toggles the current mute state.
		/// </summary>
		public override void ToggleIsMuted()
		{
#if SIMPLSHARP
			throw new NotSupportedException();
#else
			if (m_MasterVolumeEndpoint == null)
				throw new InvalidOperationException("No internal audio endpoint");

			m_MasterVolumeEndpoint.AudioEndpointVolume.Mute =
				!m_MasterVolumeEndpoint.AudioEndpointVolume.Mute;
#endif
		}

		/// <summary>
		/// Sets the raw volume level in the device volume representation.
		/// </summary>
		/// <param name="level"></param>
		public override void SetVolumeLevel(float level)
		{
#if SIMPLSHARP
			throw new NotSupportedException();
#else
			if (m_MasterVolumeEndpoint == null)
				throw new InvalidOperationException("No internal audio endpoint");

			// Scalar handles logarithmic db for us
			m_MasterVolumeEndpoint.AudioEndpointVolume.MasterVolumeLevelScalar = MathUtils.Clamp(level, 0, 1);
#endif
		}

		/// <summary>
		/// Raises the volume one time
		/// Amount of the change varies between implementations - typically "1" raw unit
		/// </summary>
		public override void VolumeIncrement()
		{
#if SIMPLSHARP
			throw new NotSupportedException();
#else
			if (m_MasterVolumeEndpoint == null)
				throw new InvalidOperationException("No internal audio endpoint");

			m_MasterVolumeEndpoint.AudioEndpointVolume.MasterVolumeLevelScalar =
				(float)MathUtils.Clamp(Math.Round(VolumeLevel, 2) + 0.01f, 0, 1);
#endif
		}

		/// <summary>
		/// Lowers the volume one time
		/// Amount of the change varies between implementations - typically "1" raw unit
		/// </summary>
		public override void VolumeDecrement()
		{
#if SIMPLSHARP
			throw new NotSupportedException();
#else
			if (m_MasterVolumeEndpoint == null)
				throw new InvalidOperationException("No internal audio endpoint");

			m_MasterVolumeEndpoint.AudioEndpointVolume.MasterVolumeLevelScalar =
				(float)MathUtils.Clamp(Math.Round(VolumeLevel, 2) - 0.01f, 0, 1);
#endif
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

		#endregion

#if NETSTANDARD

		#region Private Methods

		/// <summary>
		/// Updates the volume and mute feedback properties.
		/// </summary>
		private void UpdateAudioFeedback()
		{
			if (m_MasterVolumeEndpoint == null)
				throw new InvalidOperationException("No internal audio endpoint");

			VolumeLevel = m_MasterVolumeEndpoint.AudioEndpointVolume.MasterVolumeLevelScalar;
			IsMuted = m_MasterVolumeEndpoint.AudioEndpointVolume.Mute;
		}

		#endregion

		#region Master Volume Callbacks

		/// <summary>
		/// Subscribe to the master volume endpoint events.
		/// </summary>
		/// <param name="masterVolumeEndpoint"></param>
		private void Subscribe(MMDevice masterVolumeEndpoint)
		{
			if (masterVolumeEndpoint == null)
				return;

			masterVolumeEndpoint.AudioEndpointVolume.OnVolumeNotification += MasterVolumeEndpointOnVolumeNotification;
		}

		/// <summary>
		/// Unsubscribe from the master volume endpoint events.
		/// </summary>
		/// <param name="masterVolumeEndpoint"></param>
		private void Unsubscribe(MMDevice masterVolumeEndpoint)
		{
			if (masterVolumeEndpoint == null)
				return;

			masterVolumeEndpoint.AudioEndpointVolume.OnVolumeNotification -= MasterVolumeEndpointOnVolumeNotification;
		}

		/// <summary>
		/// Called when a master volume event occurs.
		/// </summary>
		/// <param name="data"></param>
		private void MasterVolumeEndpointOnVolumeNotification(AudioVolumeNotificationData data)
		{
			UpdateAudioFeedback();
		}

		#endregion

#endif
	}
}
