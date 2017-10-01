using System;
using System.Linq;
using System.Text;
using ICD.Common.Properties;
using ICD.Connect.Protocol.Data;

namespace ICD.Connect.Misc.GlobalCache.FlexApi
{
    public sealed class FlexData : ISerialData
	{
		public const char NEWLINE = '\r';

		private readonly string m_Command;
		private readonly uint m_Module;
		private readonly uint m_Port;
		private readonly object[] m_Parameters;
		private readonly string m_ErrorCode;

		#region Properties

		/// <summary>
		/// Gets the command.
		/// </summary>
		[CanBeNull]
		public string Command { get { return m_Command; } }

		/// <summary>
		/// Gets the module id.
		/// </summary>
		public uint Module { get { return m_Module; } }

		/// <summary>
		/// Gets the port address.
		/// </summary>
		public uint Port { get { return m_Port; } }

		/// <summary>
		/// Gets the number of parameters.
		/// </summary>
		public int ParameterCount { get { return m_Parameters.Length; } }

		/// <summary>
		/// Returns true if this FlexData is an error response.
		/// </summary>
		public bool IsError { get { return m_ErrorCode != null; } }

		/// <summary>
		/// Gets the error code from the response.
		/// </summary>
		[CanBeNull]
		public string ErrorCode { get { return m_ErrorCode; } }

		#endregion

		#region Constructors

		/// <summary>
		/// Instantiates a FlexData request.
		/// </summary>
		/// <param name="command"></param>
		/// <param name="module"></param>
		/// <param name="port"></param>
		/// <param name="parameters"></param>
		public FlexData(string command, uint module, uint port, params object[] parameters)
			: this(command, module, port, parameters, null)
		{
		}

		/// <summary>
		/// Instantiates a FlexData from a given error code.
		/// </summary>
		/// <param name="errorCode"></param>
		private FlexData(string errorCode)
			: this(null, 0, 0, new object[0], errorCode)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="command"></param>
		/// <param name="module"></param>
		/// <param name="port"></param>
		/// <param name="parameters"></param>
		/// <param name="errorCode"></param>
		private FlexData(string command, uint module, uint port, object[] parameters, string errorCode)
		{
			m_Command = command;
			m_Module = module;
			m_Port = port;
			m_Parameters = parameters;
			m_ErrorCode = errorCode;
		}

		#endregion

		#region Serialization

		/// <summary>
		/// Instantiates a FlexData instance from a serial response.
		/// </summary>
		/// <param name="response"></param>
		/// <returns></returns>
		public static FlexData Deserialize(string response)
		{
			if (response == null)
				throw new ArgumentNullException("response");

			response = response.Trim();

			if (response.StartsWith("ERR "))
				return new FlexData(response.Substring(4));

			string[] split = response.Split(':');
			string[] commandModuleSplit = split[0].Split(',');
			string[] portParametersSplit = split[1].Split(',');

			string command = commandModuleSplit[0];
			uint module = uint.Parse(commandModuleSplit[1]);
			uint port = uint.Parse(portParametersSplit[0]);
			object[] parameters = portParametersSplit.Skip(1).Cast<object>().ToArray();

			return new FlexData(command, module, port, parameters);
		}

		/// <summary>
		/// Serializes the FlexData to a serial request.
		/// </summary>
		/// <returns></returns>
		public string Serialize()
		{
			if (IsError)
				return string.Format("ERR {0}{1}", m_ErrorCode, NEWLINE);

			StringBuilder builder = new StringBuilder();

			builder.Append(m_Command);
			builder.Append(',');
			builder.Append(m_Module);
			builder.Append(':');
			builder.Append(m_Port);

			foreach (object parameter in m_Parameters)
			{
				builder.Append(',');
				builder.Append(parameter);
			}

			builder.Append(NEWLINE);

			return builder.ToString();
		}

		#endregion
	}
}
