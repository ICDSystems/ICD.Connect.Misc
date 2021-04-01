using System;
using System.Collections.Generic;
using ICD.Common.Utils.Extensions;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components
{
	public sealed class DumpComponent : AbstractVibeComponent
	{
		public event EventHandler OnUsbDevicesUpdated;

		private const string COMMAND = "dump";
		private const string PARAM_USB = "usb";
		
		private readonly List<UsbDeviceData> m_UsbDeviceList;

		#region Properties

		public IEnumerable<UsbDeviceData> UsbDevices
		{
			get { return m_UsbDeviceList.AsReadOnly(); }
		}

		#endregion

		public DumpComponent(VibeBoard parent)
			: base(parent)
		{
			m_UsbDeviceList = new List<UsbDeviceData>();

			Subscribe(parent);
		}

		protected override void Dispose(bool disposing)
		{
			Unsubscribe(Parent);

			base.Dispose(disposing);
		}

		#region API Methods

		public void DumpUsbDevices()
		{
			Parent.SendCommand(new VibeCommand(COMMAND, PARAM_USB));
		}

		#endregion

		#region Parent Callbacks

		protected override void Subscribe(VibeBoard vibe)
		{
			if (vibe == null)
				return;

			vibe.ResponseHandler.RegisterResponseCallback<DumpResponse>(DumpCallback);
		}

		protected override void Unsubscribe(VibeBoard vibe)
		{
			if (vibe == null)
				return;

			vibe.ResponseHandler.UnregisterResponseCallback<DumpResponse>(DumpCallback);
		}

		private void DumpCallback(DumpResponse response)
		{
			if (response.Error != null)
			{
				Parent.Logger.Log(eSeverity.Error, "Error getting usb dump info - {0}", response.Error.Message);
				return;
			}

			m_UsbDeviceList.Clear();
			m_UsbDeviceList.AddRange(response.Value);

			Parent.Logger.Log(eSeverity.Informational, "USB devices updated");
			OnUsbDevicesUpdated.Raise(this);
		}

		#endregion

		#region Console

		public override IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			foreach (var command in GetBaseConsoleCommands())
				yield return command;

			yield return new ConsoleCommand("DumpUsb", "Gets dump info about USB devices", () => DumpUsbDevices());
		}

		private IEnumerable<IConsoleCommand> GetBaseConsoleCommands()
		{
			return base.GetConsoleCommands();
		}

		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			addRow("USB Device Count", m_UsbDeviceList.Count);
		}

		#endregion
	}
}
