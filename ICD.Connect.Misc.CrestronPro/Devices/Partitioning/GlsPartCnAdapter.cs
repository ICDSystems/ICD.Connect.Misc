using System;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Nodes;
using ICD.Connect.Misc.CrestronPro.Utils;
using ICD.Connect.Partitioning.Devices;
using ICD.Connect.Protocol.FeedbackDebounce;
using ICD.Connect.Settings.Core;
#if SIMPLSHARP
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.GeneralIO;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.Partitioning
{
	public sealed class GlsPartCnAdapter : AbstractPartitionDevice<GlsPartCnAdapterSettings>
	{
#if SIMPLSHARP
		private GlsPartCn m_PartitionDevice;
#endif

		private readonly FeedbackDebounce<bool> m_Debounce;

		/// <summary>
		/// Constructor.
		/// </summary>
		public GlsPartCnAdapter()
		{
			m_Debounce = new FeedbackDebounce<bool>();
			m_Debounce.OnValue += DebounceOnValue;
		}

		#region Methods

#if SIMPLSHARP

		/// <summary>
		/// Sets the wrapped device.
		/// </summary>
		/// <param name="device"></param>
		public void SetDevice(GlsPartCn device)
		{
			if (device == m_PartitionDevice)
				return;

			Unsubscribe(m_PartitionDevice);

			if (m_PartitionDevice != null)
			{
				if (m_PartitionDevice.Registered)
					m_PartitionDevice.UnRegister();

				try
				{
					m_PartitionDevice.Dispose();
				}
				catch
				{
				}
			}

			m_PartitionDevice = device;

			if (m_PartitionDevice != null && !m_PartitionDevice.Registered)
			{
				if (Name != null)
					m_PartitionDevice.Description = Name;
				eDeviceRegistrationUnRegistrationResponse result = m_PartitionDevice.Register();
				if (result != eDeviceRegistrationUnRegistrationResponse.Success)
					Logger.AddEntry(eSeverity.Error, "Unable to register {0} - {1}", m_PartitionDevice.GetType().Name, result);

				// Actually enable feedback from the device!
				m_PartitionDevice.Enable.BoolValue = true;
			}

			Subscribe(m_PartitionDevice);
			UpdateCachedOnlineStatus();

			UpdateStatus();
		}
#endif

		/// <summary>
		/// Opens the partition.
		/// </summary>
		public override void Open()
		{
		}

		/// <summary>
		/// Closes the partition.
		/// </summary>
		public override void Close()
		{
		}

		public void SetSensitivity(ushort sensitivity)
		{
#if SIMPLSHARP
			if (m_PartitionDevice != null)
				m_PartitionDevice.Sensitivity.UShortValue = sensitivity;
#endif
		}

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
#if SIMPLSHARP
			return m_PartitionDevice != null && m_PartitionDevice.IsOnline;
#else
            return false;
#endif
		}

		#endregion

		#region Settings

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(GlsPartCnAdapterSettings settings)
		{
			base.CopySettingsFinal(settings);

#if SIMPLSHARP
			settings.CresnetId = m_PartitionDevice == null ? (byte)0 : (byte)m_PartitionDevice.ID;
			settings.Sensitivity = m_PartitionDevice == null
				                       ? (ushort)0
				                       : m_PartitionDevice.SensitivityFeedback.UShortValue;
#else
            settings.CresnetId = 0;
			settings.Sensitivity = 0;
#endif
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

#if SIMPLSHARP
			SetDevice(null);
#endif
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(GlsPartCnAdapterSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

#if SIMPLSHARP
			if (!CresnetUtils.IsValidId(settings.CresnetId))
			{
				Logger.AddEntry(eSeverity.Error, "{0} failed to instantiate {1} - CresnetId {2} is out of range",
				                this, typeof(GlsPartCnAdapter).Name, settings.CresnetId);
				return;
			}

			GlsPartCn device = null;

			try
			{
				device = new GlsPartCn(settings.CresnetId, ProgramInfo.ControlSystem);
			}
			catch (ArgumentException e)
			{
				string message = string.Format("{0} failed to instantiate {1} with Cresnet ID {2} - {3}",
											   this, typeof(GlsPartCnAdapter).Name, settings.CresnetId, e.Message);
				Logger.AddEntry(eSeverity.Error, e, message);
			}

			SetDevice(device);
			SetSensitivity(settings.Sensitivity);
#else
            throw new NotImplementedException();
#endif
		}

		#endregion

		#region Device Callbacks

#if SIMPLSHARP
		/// <summary>
		/// Subscribe to the device events.
		/// </summary>
		/// <param name="partitionDevice"></param>
		private void Subscribe(GlsPartCn partitionDevice)
		{
			if (partitionDevice == null)
				return;

			partitionDevice.OnlineStatusChange += PortsDeviceOnLineStatusChange;
			partitionDevice.BaseEvent += PartitionDeviceOnBaseEvent;
		}

		/// <summary>
		/// Unsubscribe from the device events.
		/// </summary>
		/// <param name="partitionDevice"></param>
		private void Unsubscribe(GlsPartCn partitionDevice)
		{
			if (partitionDevice == null)
				return;

			partitionDevice.OnlineStatusChange -= PortsDeviceOnLineStatusChange;
			partitionDevice.BaseEvent -= PartitionDeviceOnBaseEvent;
		}

		/// <summary>
		/// Called when the device online status changes.
		/// </summary>
		/// <param name="currentDevice"></param>
		/// <param name="args"></param>
		private void PortsDeviceOnLineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
		{
			UpdateCachedOnlineStatus();
		}

		/// <summary>
		/// Called when the partition sensor fires an event.
		/// </summary>
		/// <param name="device"></param>
		/// <param name="args"></param>
		private void PartitionDeviceOnBaseEvent(GenericBase device, BaseEventArgs args)
		{
			switch (args.EventId)
			{
				case GlsPartCn.PartitionNotSensedFeedbackEventId:
				case GlsPartCn.PartitionSensedFeedbackEventId:
					UpdateStatus();
					break;
			}
		}

		/// <summary>
		/// Updates the state of the control.
		/// </summary>
		private void UpdateStatus()
		{
			bool open = m_PartitionDevice != null && m_PartitionDevice.PartitionNotSensedFeedback.BoolValue;
			m_Debounce.Enqueue(open);
		}
#endif

		/// <summary>
		/// Called when the debouncer decides on a new value for the open status.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="genericEventArgs"></param>
		private void DebounceOnValue(object sender, GenericEventArgs<bool> genericEventArgs)
		{
			IsOpen = genericEventArgs.Data;
		}

		#endregion

		#region Console

		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

#if SIMPLSHARP
			addRow("Sensitivity", m_PartitionDevice == null ? 0 : m_PartitionDevice.SensitivityFeedback.UShortValue);
#endif
		}

		#endregion
	}
}
