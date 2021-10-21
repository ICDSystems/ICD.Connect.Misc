using System;
using ICD.Connect.Audio.Controls.Volume;
#if !SIMPLSHARP
using System.Runtime.InteropServices;
using ICD.Common.Properties;
using ICD.Common.Utils;
using NAudio.CoreAudioApi;
#endif

namespace ICD.Connect.Misc.Windows.Devices.ControlSystems
{
	public sealed class WindowsControlSystemMasterVolumeControl : AbstractVolumeDeviceControl<WindowsControlSystem>
	{
#if !SIMPLSHARP
		[CanBeNull]
		private MMDevice m_MasterVolumeEndpoint;
#endif

		#region Properties

#if !SIMPLSHARP
		[CanBeNull]
		private MMDevice MasterVolumeEndpoint
		{
			get { return m_MasterVolumeEndpoint; }
			set
			{
				if (value == m_MasterVolumeEndpoint)
					return;

				if (m_MasterVolumeEndpoint != null)
					m_MasterVolumeEndpoint.Dispose();

				Unsubscribe(m_MasterVolumeEndpoint);
				m_MasterVolumeEndpoint = value;
				Subscribe(m_MasterVolumeEndpoint);

				UpdateAudioFeedback();
			}
		}
#endif

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
#if !SIMPLSHARP
			SupportedVolumeFeatures = eVolumeFeatures.Mute |
			                          eVolumeFeatures.MuteAssignment |
			                          eVolumeFeatures.MuteFeedback |
			                          eVolumeFeatures.Volume |
			                          eVolumeFeatures.VolumeAssignment |
			                          eVolumeFeatures.VolumeFeedback;

			InitializeDevice();
#endif
		}

#if !SIMPLSHARP
		/// <summary>
		/// Override to release resources.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void DisposeFinal(bool disposing)
		{
			base.DisposeFinal(disposing);

			MasterVolumeEndpoint = null;
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
			AttemptCom(d => d.AudioEndpointVolume.Mute = mute);
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
			AttemptCom(d => d.AudioEndpointVolume.Mute = !d.AudioEndpointVolume.Mute);
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
			// Scalar handles logarithmic db for us
			AttemptCom(d => d.AudioEndpointVolume.MasterVolumeLevelScalar = MathUtils.Clamp(level, 0, 1));
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
			AttemptCom(d => d.AudioEndpointVolume.MasterVolumeLevelScalar =
				           (float)MathUtils.Clamp(Math.Round(VolumeLevel, 2) + 0.01f, 0, 1));
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
			AttemptCom(d => d.AudioEndpointVolume.MasterVolumeLevelScalar =
				           (float)MathUtils.Clamp(Math.Round(VolumeLevel, 2) - 0.01f, 0, 1));
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

#if !SIMPLSHARP

		#region Private Methods

		/// <summary>
		/// Attempts to perform the given action for the audio device.
		/// If the action fails with a COMException, attempts to reinitialize the device.
		/// </summary>
		/// <param name="action"></param>
		private void AttemptCom(Action<MMDevice> action)
		{
			if (MasterVolumeEndpoint == null)
				InitializeDevice();

			// First attempt
			try
			{
				action(MasterVolumeEndpoint);
				return;
			}
			catch (COMException)
			{
			}

			// Second attempt
			try
			{
				InitializeDevice();
				action(MasterVolumeEndpoint);
			}
			catch (COMException)
			{
				MasterVolumeEndpoint = null;
				throw;
			}
		}

		/// <summary>
		/// Updates the wrapped volume endpoint.
		/// </summary>
		private void InitializeDevice()
		{
			using (MMDeviceEnumerator devices = new MMDeviceEnumerator())
				MasterVolumeEndpoint = devices.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
		}

		/// <summary>
		/// Updates the volume and mute feedback properties.
		/// </summary>
		private void UpdateAudioFeedback()
		{
			try
			{
				VolumeLevel = MasterVolumeEndpoint?.AudioEndpointVolume.MasterVolumeLevelScalar ?? 0;
				IsMuted = MasterVolumeEndpoint?.AudioEndpointVolume.Mute ?? false;
			}
			catch (COMException)
			{
			}
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
