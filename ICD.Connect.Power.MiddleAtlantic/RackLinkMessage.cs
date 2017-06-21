using System;
using ICD.Common.Utils;
using ICD.Common.Utils.Extensions;
using ICD.Connect.Protocol.Data;

namespace ICD.Connect.Power.MiddleAtlantic
{
	/// <summary>
	/// [Header][Length][data envelope][Checksum][Tail]
	/// • Header
	///		o Value: 0xfe
	///		o Data Length: 1 Byte
	/// • Length
	///		o Total Length of the data envelope.
	///			 Value Example 1: 0x3c (60 bytes)
	///			 Value Example 2: 0x06 (6 bytes)
	///		o Data Length: 1 Byte
	///	• Data Envelope
	///		o Contents: Varied
	///		o Data Length: 3 – 250 Bytes
	/// • Checksum
	///		o Summation of all the bytes starting and including the header byte all the way to the end of
	///			the data envelope. The checksum will only include the least significant 7 bits. The eighth bit
	///			gets set to zero.
	///		o Data Length: 1 Byte
	/// • Tail
	///		o Value: 0xff
	///		o Data Length: 1 Byte
	/// </summary>
	public sealed class RackLinkMessage : ISerialData
	{
		public enum eSubCommand
		{
			Set = 0x01,
			Get = 0x02,
			Response = 0x10,
			StatusChange = 0x12,
			LogAlert = 0x30
		}

		public enum eErrorCode
		{
			None = 0x00,
			BadChecksum = 0x01,
			BadLength = 0x02,
			EscapedError = 0x03,
			InvalidCommand = 0x04,
			InvalidSubCommand = 0x05,
			InvalidQtyDataBytes = 0x06,
			InvalidDataByteValues = 0x07,
			AccessDeniedCredentials = 0x08,
			Unknown = 0x10,
			AccessDeniedEpo = 0x11
		}

		public const byte HEADER = 0xFE;
		public const byte TAIL = 0XFF;
		private readonly string m_Data;

		#region Properties

		/// <summary>
		/// Gets the envelope length from the data serial.
		/// </summary>
		public byte Length { get { return GetLength(m_Data); } }

		/// <summary>
		/// Gets the envelope from the data serial.
		/// </summary>
		public string Envelope { get { return GetDataEnvelope(m_Data); } }

		/// <summary>
		/// Gets the address value from the envelope.
		/// </summary>
		public byte Address { get { return (byte)Envelope[0]; } }

		/// <summary>
		/// Gets the command from the envelope.
		/// </summary>
		public byte Command { get { return (byte)Envelope[1]; } }

		/// <summary>
		/// Gets the sub command value from the envelope.
		/// </summary>
		public eSubCommand SubCommand { get { return (eSubCommand)Envelope[2]; } }

		/// <summary>
		/// Gets the value portion of the envelope.
		/// </summary>
		public string Value { get { return Envelope.Substring(3); } }

		/// <summary>
		/// Gets the error code from a response.
		/// </summary>
		public eErrorCode Error
		{
			get
			{
				if (SubCommand != eSubCommand.Response)
					return eErrorCode.None;

				string value = Value;
				if (value.Length != 1)
					return eErrorCode.None;

				return (eErrorCode)value[0];
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="data"></param>
		public RackLinkMessage(string data)
		{
			if (!data.EndsWith((char)TAIL))
				data += (char)TAIL;
			m_Data = data;
		}

		/// <summary>
		/// Creates a message from the given values.
		/// </summary>
		/// <param name="command"></param>
		/// <param name="subCommand"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public RackLinkMessage CreateMessage(byte command, eSubCommand subCommand, string value)
		{
			return CreateMessage((char)command + (char)subCommand + value);
		}

		/// <summary>
		/// Creates a message from the given envelope.
		/// </summary>
		/// <param name="envelope"></param>
		/// <returns></returns>
		private RackLinkMessage CreateMessage(string envelope)
		{
			if (envelope.Length < 3 || envelope.Length > 250)
				throw new FormatException("Envelope must be between 3 and 250 characters");

			string data = (char)HEADER + (char)envelope.Length + envelope;
			data += (char)BuildChecksum(data) + (char)TAIL;

			return new RackLinkMessage(data);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Serialize this instance to a string.
		/// </summary>
		/// <returns></returns>
		public string Serialize()
		{
			return m_Data;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the envelope length from the data serial.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private static byte GetLength(string data)
		{
			return (byte)data[1];
		}

		/// <summary>
		/// Gets the data envelope portion of the data serial.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private static string GetDataEnvelope(string data)
		{
			int length = GetLength(data);
			return data.Substring(2, length);
		}

		/// <summary>
		/// Builds the checksum for the given header, length and envelope.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private static byte BuildChecksum(string data)
		{
			unchecked
			{
				byte sum = StringUtils.ToBytes(data).Sum();
				return (byte)(sum & 0x7F);
			}
		}

		#endregion
	}
}
