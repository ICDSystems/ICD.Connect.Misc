using ICD.Common.Properties;

namespace ICD.Connect.Misc.CrestronPro.Ports.IrPort
{
	/// <summary>
	/// Simple command/duration pair.
	/// </summary>
	public struct IrPulse
	{
		private readonly string m_Command;
		private readonly ushort m_PulseTime;
		private readonly ushort m_BetweenTime;

		/// <summary>
		/// Gets the command.
		/// </summary>
		public string Command { get { return m_Command; } }

		/// <summary>
		/// Gets the pulse duration.
		/// </summary>
		public ushort PulseTime { get { return m_PulseTime; } }

		/// <summary>
		/// Gets the duration between this and the next pulse.
		/// </summary>
		[PublicAPI]
		public ushort BetweenTime { get { return m_BetweenTime; } }

		/// <summary>
		/// Returns the full duration of the pulse including the between time.
		/// </summary>
		public ushort Duration { get { return (ushort)(m_PulseTime + m_BetweenTime); } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="command"></param>
		/// <param name="pulseTime"></param>
		/// <param name="betweenTime"></param>
		public IrPulse(string command, ushort pulseTime, ushort betweenTime)
		{
			m_Command = command;
			m_PulseTime = pulseTime;
			m_BetweenTime = betweenTime;
		}
	}
}
