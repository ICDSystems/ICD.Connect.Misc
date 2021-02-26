using System;
using ICD.Common.Properties;
using ICD.Common.Utils;
using ICD.Connect.Telemetry.Attributes;
using ICD.Connect.Telemetry.Providers.External;

namespace ICD.Connect.Misc.ControlSystems
{
	[UsedImplicitly]
	public sealed class ControlSystemExternalTelemetryProvider : AbstractExternalTelemetryProvider<IControlSystemDevice>
	{
		private const string PROGRAMMER_NAME = "ICD Systems";
		private const string SYSTEM_NAME = "ICD.Connect";

		#region Properties

		[PropertyTelemetry(ControlSystemExternalTelemetryNames.PROGRAM_START_TIME, null, null)]
		public DateTime? ProgramUptime { get { return ProcessorUtils.GetProgramStartTime(); } }

		[PropertyTelemetry(ControlSystemExternalTelemetryNames.PROGRAMMER_NAME, null, null)]
		public string ProgrammerName { get { return PROGRAMMER_NAME; } }

		[PropertyTelemetry(ControlSystemExternalTelemetryNames.SYSTEM_NAME, null, null)]
		public string SystemName { get { return SYSTEM_NAME; } }

		[PropertyTelemetry(ControlSystemExternalTelemetryNames.PROGRAM_SOURCE_FILE, null, null)]
		public string ProgramSourceFile { get { return ProgramUtils.ProgramFile; } }

		[PropertyTelemetry(ControlSystemExternalTelemetryNames.PROGRAM_COMPLIE_DATE, null, null)]
		public DateTime ProgramCompileDate { get { return ProgramUtils.CompiledDate; } }

		#endregion
	}
}
