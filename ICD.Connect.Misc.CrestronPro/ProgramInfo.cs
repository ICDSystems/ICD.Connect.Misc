#if SIMPLSHARP
using System;
using Crestron.SimplSharpPro;
using ICD.Common.Properties;

namespace ICD.Connect.Misc.CrestronPro
{
	/// <summary>
	/// Provides information about the running program.
	/// </summary>
	public static class ProgramInfo
	{
		private static CrestronControlSystem s_ControlSystem;

		#region Properties

		/// <summary>
		/// Gets the entry point for this program.
		/// </summary>
		public static CrestronControlSystem ControlSystem
		{
			get
			{
				if (s_ControlSystem == null)
					throw new InvalidOperationException("No registered control system");
				return s_ControlSystem;
			}
		}

		/// <summary>
		/// Gets the program number.
		/// </summary>
		[PublicAPI]
		public static uint ProgramNumber { get { return ControlSystem.ProgramNumber; } }

		/// <summary>
		/// Gets the number of program slots.
		/// </summary>
		[PublicAPI]
		public static uint ProgramSlots { get { return ControlSystem.NumProgramsSupported; } }

		#endregion

		/// <summary>
		/// Registers the given control system.
		/// </summary>
		/// <param name="controlSystem"></param>
		[PublicAPI]
		public static void RegisterControlSystem(CrestronControlSystem controlSystem)
		{
			if (controlSystem == s_ControlSystem)
				return;

			if (s_ControlSystem != null)
				throw new InvalidOperationException("Different control system already registered");

			s_ControlSystem = controlSystem;
		}
	}
}

#endif
