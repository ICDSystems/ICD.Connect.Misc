using ICD.Connect.Protocol.Data;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components
{
	public struct VibeCommand : ISerialData
	{
		private const string COMMAND_FORMAT = "vm {0} {1}\n";
		private const string COMMAND_WITH_RESULTID_FORMAT = "vm {0} {1} resultId=\"{2}\"";

		private readonly string m_Command;
		private readonly string m_Parameters;
		private readonly string m_ResultId;

		public string Command { get { return m_Command; } }

		public string Parameters { get { return m_Parameters; } }

		public string ResultId { get { return m_ResultId; } }

		public VibeCommand(string command, string parameters, string resultId)
		{
			m_Command = command;
			m_Parameters = parameters;
			m_ResultId = resultId;
		}

		public VibeCommand(string command, string parameters)
		{
			m_Command = command;
			m_Parameters = parameters;
			m_ResultId = null;
		}

		public string Serialize()
		{
			if (ResultId == null)
				return string.Format(COMMAND_FORMAT, m_Command, m_Parameters);
			return string.Format(COMMAND_WITH_RESULTID_FORMAT, m_Command, m_Parameters, m_ResultId);
		}
	}
}
