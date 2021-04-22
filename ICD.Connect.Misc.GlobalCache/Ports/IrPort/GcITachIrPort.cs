using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Properties;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.API.Nodes;
using ICD.Connect.Devices.Extensions;
using ICD.Connect.Misc.GlobalCache.Devices;
using ICD.Connect.Misc.GlobalCache.FlexApi.RestApi;
using ICD.Connect.Protocol.Data;
using ICD.Connect.Protocol.Ports.IrPort;
using ICD.Connect.Protocol.Settings;
using ICD.Connect.Protocol.Utils;
using ICD.Connect.Settings;
using IrCommand = ICD.Connect.Protocol.Data.IrCommand;

namespace ICD.Connect.Misc.GlobalCache.Ports.IrPort
{
	public sealed class GcITachIrPort : AbstractIrPort<GcITachIrPortSettings>, IGcITachPort
	{
		#region Private Members

		private const Module.eType DEFAULT_IR_MODULE_TYPE = FlexApi.RestApi.Module.eType.OneEmitter;

		private readonly IrDriverProperties m_LoadedDriverProperties;

		private IrDriver m_LoadedDriver;

		#endregion

		#region Properties

		public override IIrDriverProperties IrDriverProperties { get { return m_LoadedDriverProperties; } }

		public override string DriverPath { get { return IrDriverProperties.IrDriverPath; } }
		public override ushort PulseTime { get; set; }
		public override ushort BetweenTime { get; set; }

		public IGcITachDevice Device { get; private set; }
		public int Module { get; private set; }
		public int Address { get; private set; }
		public Module.eType IrModuleType { get; private set; }

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public GcITachIrPort()
		{
			m_LoadedDriverProperties = new IrDriverProperties();
		}

		#endregion

		#region IR

		public override void LoadDriver(string path)
		{
			m_LoadedDriver = IrFormatUtils.ImportDriverFromPath(path);
		}

		public override IEnumerable<string> GetCommands()
		{
			if (m_LoadedDriver == null)
				yield break;

			foreach (string s in m_LoadedDriver.GetCommands().Select(c => c.Name))
			{
				yield return s;
			}
		}

		protected override void PressFinal(string command)
		{
			if (m_LoadedDriver == null)
				return;

			IrCommand krangIrCommand = m_LoadedDriver.GetCommands()
			                                         .FirstOrDefault(irc => string.Equals(irc.Name, command, StringComparison.CurrentCultureIgnoreCase));

			if (krangIrCommand == null)
			{
				Logger.Log(eSeverity.Error, "Cannot find command data for command - {0}", command);
				return;
			}

			Device.SendCommand(SerializeIrCommand(krangIrCommand));
		}

		protected override void ReleaseFinal()
		{
			Device.SendCommand(GetStopIrCommand());
		}

		#endregion

		#region Methods

		protected override bool GetIsOnlineStatus()
		{
			return Device != null && Device.IsOnline;
		}

		/// <summary>
		/// Sets the parent device.
		/// </summary>
		/// <param name="device"></param>
		[PublicAPI]
		public void SetDevice(IGcITachDevice device)
		{
			if (device == Device)
				return;

			Device = device;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Sets the configuration for the model.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="class"></param>
		/// <param name="type"></param>
		private void SetModuleType(Module.eId id, Module.eClass @class, Module.eType type)
		{
			GcITachPortHelper.SetModuleType(this, id, @class, type);
		}

		/// <summary>
		/// Serializes the given IR Command into a GC IR command string.
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		private string SerializeIrCommand(IrCommand command)
		{
			return string.Format("sendir,{0}:{1},{2},{3},{4},{5}{6}\r", Module, Address, 1, command.Frequency,
			                     command.RepeatCount, command.Offset ? "1," : "",
			                     string.Join(",", command.Data.Select(i => i.ToString()).ToArray()));
		}

		/// <summary>
		/// Serializes a GC stop IR transmission command string for the current module and address.
		/// </summary>
		/// <returns></returns>
		private string GetStopIrCommand()
		{
			return string.Format("stopir,{0}:{1}\r", Module, Address);
		}

		#endregion

		#region Settings

		protected override void StartSettingsFinal()
		{
			base.StartSettingsFinal();

			// Ensure the port's module type is configured correctly.
			switch (IrModuleType)
			{
				case FlexApi.RestApi.Module.eType.TripleIrEmitter:
					SetModuleType(FlexApi.RestApi.Module.eId.Flc3Emitter, FlexApi.RestApi.Module.eClass.Infrared, IrModuleType);
					break;
				case FlexApi.RestApi.Module.eType.TwoIrEmitterOneIrBlaster:
					SetModuleType(FlexApi.RestApi.Module.eId.Flc2E1B, FlexApi.RestApi.Module.eClass.Infrared, IrModuleType);
					break;
				case FlexApi.RestApi.Module.eType.OneBlaster:
					SetModuleType(FlexApi.RestApi.Module.eId.FlcBlaster, FlexApi.RestApi.Module.eClass.Infrared, IrModuleType);
					break;
				case FlexApi.RestApi.Module.eType.OneEmitter:
					SetModuleType(FlexApi.RestApi.Module.eId.GlobalIrEmitter, FlexApi.RestApi.Module.eClass.Infrared, IrModuleType);
					break;
				default:
					throw new InvalidOperationException("Invalid configured module type.");
			}
		}

		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			Module = 1;
			Address = 1;
			IrModuleType = DEFAULT_IR_MODULE_TYPE;

			SetDevice(null);
		}

		protected override void CopySettingsFinal(GcITachIrPortSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Module = Module;
			settings.Address = Address;
			settings.Device = Device == null ? (int?)null : Device.Id;
			settings.IrModuleType = IrModuleType;
		}

		protected override void ApplySettingsFinal(GcITachIrPortSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			Module = settings.Module;
			Address = settings.Address;
			IrModuleType = settings.IrModuleType ?? DEFAULT_IR_MODULE_TYPE;

			IGcITachDevice device = null;

			if (settings.Device != null)
			{
				try
				{
					device = factory.GetDeviceById((int)settings.Device) as IGcITachDevice;
				}
				catch (KeyNotFoundException)
				{
					Logger.Log(eSeverity.Error, "No device with id {0}", Device);
				}
			}

			SetDevice(device);

			ApplyConfiguration();
		}

		#endregion

		#region Console

		public override void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
			base.BuildConsoleStatus(addRow);

			GcITachPortHelper.BuildConsoleStatus(this, addRow);
		}

		#endregion
	}
}