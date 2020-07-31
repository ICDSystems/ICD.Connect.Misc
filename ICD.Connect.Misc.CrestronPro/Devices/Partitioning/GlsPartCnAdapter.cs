using System;
using System.Collections.Generic;
using ICD.Common.Logging.LoggingContexts;
using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Misc.CrestronPro.Cresnet;
using ICD.Connect.Partitioning.Controls;
using ICD.Connect.Partitioning.Devices;
using ICD.Connect.Protocol.FeedbackDebounce;
using ICD.Connect.Settings;
#if SIMPLSHARP
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.GeneralIO;
using ICD.Connect.Misc.CrestronPro.Extensions;
using ICD.Connect.Misc.CrestronPro.Utils;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.Partitioning
{
	public sealed class GlsPartCnAdapter : AbstractPartitionDevice<GlsPartCnAdapterSettings>, ICresnetDevice
	{
#if SIMPLSHARP
		private GlsPartCn m_PartitionDevice;
#endif

		private CresnetDeviceInfo m_CresnetDeviceInfo;

		public CresnetDeviceInfo CresnetDeviceInfo { get { return m_CresnetDeviceInfo; } }

		private readonly FeedbackDebounce<bool> m_Debounce;

		/// <summary>
		/// Returns the mask for the type of feedback that is supported,
		/// I.e. if we can set the open state of the partition, and if the partition
		/// gives us feedback for the current open state.
		/// </summary>
		public override ePartitionFeedback SupportsFeedback { get { return ePartitionFeedback.Get; } }

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
				GenericBaseUtils.TearDown(m_PartitionDevice);

			m_PartitionDevice = device;

			eDeviceRegistrationUnRegistrationResponse result;
			if (m_PartitionDevice != null && !GenericBaseUtils.SetUp(m_PartitionDevice, this, out result))
				Logger.Log(eSeverity.Error, "Unable to register {0} - {1}", m_PartitionDevice.GetType().Name, result);

			// Actually enable feedback from the device!
			if (m_PartitionDevice != null)
				m_PartitionDevice.Enable.BoolValue = true;

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
			throw new NotSupportedException();
		}

		/// <summary>
		/// Closes the partition.
		/// </summary>
		public override void Close()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Sets the sensitivity of the sensor.
		/// </summary>
		/// <param name="sensitivity"></param>
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

			m_CresnetDeviceInfo.CopySettings(settings);

#if SIMPLSHARP
			settings.Sensitivity = m_PartitionDevice == null
									   ? (ushort?)null
									   : m_PartitionDevice.SensitivityFeedback.GetUShortValueOrDefault();
#else
			settings.Sensitivity = 0;
#endif
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			m_CresnetDeviceInfo.ClearSettings();

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

			m_CresnetDeviceInfo = new CresnetDeviceInfo(settings);

#if SIMPLSHARP
			if (m_CresnetDeviceInfo.CresnetId == null || !CresnetUtils.IsValidId(m_CresnetDeviceInfo.CresnetId.Value))
			{
				Logger.Log(eSeverity.Error, "Failed to instantiate {0} - CresnetId {1} is out of range",
						   typeof(GlsPartCn).Name, m_CresnetDeviceInfo.CresnetId == null ? "NULL" : m_CresnetDeviceInfo.CresnetId.ToString());
				return;
			}

			GlsPartCn device = null;

			try
			{
				device = CresnetUtils.InstantiateCresnetDevice(m_CresnetDeviceInfo.CresnetId.Value,
															   m_CresnetDeviceInfo.BranchId,
															   m_CresnetDeviceInfo.ParentId, 
															   factory, 
															   cresnetId => new GlsPartCn(cresnetId, ProgramInfo.ControlSystem), 
															   (cresnetId, branch) => new GlsPartCn(cresnetId, branch));

			}
			catch (ArgumentException e)
			{
				Logger.Log(eSeverity.Error, e, "Failed to instantiate {0} with Cresnet ID {1}",
						   typeof(GlsPartCnAdapter).Name, m_CresnetDeviceInfo.CresnetId);
			}

			SetDevice(device);

			if (settings.Sensitivity.HasValue)
				SetSensitivity(settings.Sensitivity.Value);
#else
            throw new NotSupportedException();
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
			bool open = m_PartitionDevice != null && m_PartitionDevice.PartitionNotSensedFeedback.GetBoolValueOrDefault();
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

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

#if SIMPLSHARP
			CresnetDeviceConsole.BuildConsoleStatus(this, addRow);
			addRow("Sensitivity", m_PartitionDevice == null ? (ushort?)null : m_PartitionDevice.SensitivityFeedback.GetUShortValueOrDefault());
#endif
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (IConsoleCommand command in GetBaseConsoleCommands())
				yield return command;

			yield return new GenericConsoleCommand<ushort>("SetSensitivity", "SetSensitivity <0-65535>", s => SetSensitivity(s));
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		#endregion
	}
}
