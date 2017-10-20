#if SIMPLSHARP
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.GeneralIO;
using ICD.Connect.Partitioning.Controls;

namespace ICD.Connect.Misc.CrestronPro.Devices.Partitioning
{
	public sealed class GlsPartCnPartitionDeviceControl : AbstractPartitionDeviceControl<GlsPartCnAdapter>
	{
		private GlsPartCn m_PartitionDevice;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="id"></param>
		public GlsPartCnPartitionDeviceControl(GlsPartCnAdapter parent, int id)
			: base(parent, id)
		{
			Subscribe(parent);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeFinal(bool disposing)
		{
			Unsubscribe(Parent);
			Unsubscribe(m_PartitionDevice);

			base.DisposeFinal(disposing);
		}

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

		#region Parent Callbacks

		/// <summary>
		/// Subscribe to the parent events.
		/// </summary>
		/// <param name="parent"></param>
		private void Subscribe(GlsPartCnAdapter parent)
		{
			if (parent == null)
				return;

			parent.OnPartitionDeviceChanged += ParentOnPartitionDeviceChanged;
		}

		/// <summary>
		/// Unsubscribe from the parent events.
		/// </summary>
		/// <param name="parent"></param>
		private void Unsubscribe(GlsPartCnAdapter parent)
		{
			if (parent == null)
				return;

			parent.OnPartitionDeviceChanged -= ParentOnPartitionDeviceChanged;
		}

		/// <summary>
		/// Called when the wrapped sensor changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="device"></param>
		private void ParentOnPartitionDeviceChanged(GlsPartCnAdapter sender, GlsPartCn device)
		{
			SetPartitionDevice(device);
		}

		#endregion

		#region Partition Device Callbacks

		/// <summary>
		/// Sets the wrapped partition sensor.
		/// </summary>
		/// <param name="device"></param>
		private void SetPartitionDevice(GlsPartCn device)
		{
			if (device == m_PartitionDevice)
				return;

			Unsubscribe(m_PartitionDevice);
			m_PartitionDevice = device;
			Subscribe(m_PartitionDevice);

			UpdateStatus();
		}

		/// <summary>
		/// Subscribe to the partition sensor events.
		/// </summary>
		/// <param name="partitionDevice"></param>
		private void Subscribe(GlsPartCn partitionDevice)
		{
			if (partitionDevice == null)
				return;

			partitionDevice.BaseEvent += PartitionDeviceOnBaseEvent;
		}

		/// <summary>
		/// Unsubscribe from the partition sensor events.
		/// </summary>
		/// <param name="partitionDevice"></param>
		private void Unsubscribe(GlsPartCn partitionDevice)
		{
			if (partitionDevice == null)
				return;

			partitionDevice.BaseEvent -= PartitionDeviceOnBaseEvent;
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
			IsOpen = m_PartitionDevice != null && m_PartitionDevice.PartitionNotSensedFeedback.BoolValue;
		}

		#endregion
	}
}
#endif
