#if !NETSTANDARD
using Crestron.SimplSharpPro;
using ICD.Common.Utils.Collections;
using ICD.Connect.Protocol.Ports.ComPort;
#endif

namespace ICD.Connect.Misc.CrestronPro.Extensions
{
	public static class ComSpecExtensions
	{
#if !NETSTANDARD
		private static readonly BiDictionary<eComBaudRates, ComPort.eComBaudRates> s_BaudRateMap =
			new BiDictionary<eComBaudRates, ComPort.eComBaudRates>
			{
				{default(eComBaudRates), default(ComPort.eComBaudRates)},
				{eComBaudRates.BaudRate300, ComPort.eComBaudRates.ComspecBaudRate300},
				{eComBaudRates.BaudRate600, ComPort.eComBaudRates.ComspecBaudRate600},
				{eComBaudRates.BaudRate1200, ComPort.eComBaudRates.ComspecBaudRate1200},
				{eComBaudRates.BaudRate1800, ComPort.eComBaudRates.ComspecBaudRate1800},
				{eComBaudRates.BaudRate2400, ComPort.eComBaudRates.ComspecBaudRate2400},
				{eComBaudRates.BaudRate3600, ComPort.eComBaudRates.ComspecBaudRate3600},
				{eComBaudRates.BaudRate4800, ComPort.eComBaudRates.ComspecBaudRate4800},
				{eComBaudRates.BaudRate7200, ComPort.eComBaudRates.ComspecBaudRate7200},
				{eComBaudRates.BaudRate9600, ComPort.eComBaudRates.ComspecBaudRate9600},
				{eComBaudRates.BaudRate14400, ComPort.eComBaudRates.ComspecBaudRate14400},
				{eComBaudRates.BaudRate19200, ComPort.eComBaudRates.ComspecBaudRate19200},
				{eComBaudRates.BaudRate28800, ComPort.eComBaudRates.ComspecBaudRate28800},
				{eComBaudRates.BaudRate38400, ComPort.eComBaudRates.ComspecBaudRate38400},
				{eComBaudRates.BaudRate57600, ComPort.eComBaudRates.ComspecBaudRate57600},
				{eComBaudRates.BaudRate115200, ComPort.eComBaudRates.ComspecBaudRate115200}
			};

		private static readonly BiDictionary<eComDataBits, ComPort.eComDataBits> s_DataBitsMap =
			new BiDictionary<eComDataBits, ComPort.eComDataBits>
			{
				{default(eComDataBits), default(ComPort.eComDataBits)},
				{eComDataBits.DataBits7, ComPort.eComDataBits.ComspecDataBits7},
				{eComDataBits.DataBits8, ComPort.eComDataBits.ComspecDataBits8}
			};

		private static readonly BiDictionary<eComParityType, ComPort.eComParityType> s_ParityTypeMap =
			new BiDictionary<eComParityType, ComPort.eComParityType>
			{
				{eComParityType.None, ComPort.eComParityType.ComspecParityNone},
				{eComParityType.Even, ComPort.eComParityType.ComspecParityEven},
				{eComParityType.Odd, ComPort.eComParityType.ComspecParityOdd},
				{eComParityType.Mark, ComPort.eComParityType.ComspecParityMark},
			};

		private static readonly BiDictionary<eComStopBits, ComPort.eComStopBits> s_StopBitsMap =
			new BiDictionary<eComStopBits, ComPort.eComStopBits>
			{
				{default(eComStopBits), default(ComPort.eComStopBits)},
				{eComStopBits.StopBits1, ComPort.eComStopBits.ComspecStopBits1},
				{eComStopBits.StopBits2, ComPort.eComStopBits.ComspecStopBits2}
			};

		private static readonly BiDictionary<eComProtocolType, ComPort.eComProtocolType> s_ProtocolTypeMap =
			new BiDictionary<eComProtocolType, ComPort.eComProtocolType>
			{
				{eComProtocolType.Rs232, ComPort.eComProtocolType.ComspecProtocolRS232},
				{eComProtocolType.Rs422, ComPort.eComProtocolType.ComspecProtocolRS422},
				{eComProtocolType.Rs485, ComPort.eComProtocolType.ComspecProtocolRS485}
			};

		private static readonly BiDictionary<eComHardwareHandshakeType, ComPort.eComHardwareHandshakeType>
			s_HardwareHandshakeTypeMap =
				new BiDictionary<eComHardwareHandshakeType, ComPort.eComHardwareHandshakeType>
				{
					{eComHardwareHandshakeType.None, ComPort.eComHardwareHandshakeType.ComspecHardwareHandshakeNone},
					{eComHardwareHandshakeType.Rts, ComPort.eComHardwareHandshakeType.ComspecHardwareHandshakeRTS},
					{eComHardwareHandshakeType.Cts, ComPort.eComHardwareHandshakeType.ComspecHardwareHandshakeCTS},
					{eComHardwareHandshakeType.RtsCts, ComPort.eComHardwareHandshakeType.ComspecHardwareHandshakeRTSCTS}
				};

		private static readonly BiDictionary<eComSoftwareHandshakeType, ComPort.eComSoftwareHandshakeType>
			s_SoftwareHandshakeTypeMap =
				new BiDictionary<eComSoftwareHandshakeType, ComPort.eComSoftwareHandshakeType>
				{
					{eComSoftwareHandshakeType.None, ComPort.eComSoftwareHandshakeType.ComspecSoftwareHandshakeNone},
					{eComSoftwareHandshakeType.XOn, ComPort.eComSoftwareHandshakeType.ComspecSoftwareHandshakeXON},
					{eComSoftwareHandshakeType.XOnTransmit, ComPort.eComSoftwareHandshakeType.ComspecSoftwareHandshakeXONT},
					{eComSoftwareHandshakeType.XOnReceive, ComPort.eComSoftwareHandshakeType.ComspecSoftwareHandshakeXONR},
				};

		public static eComBaudRates FromCrestron(this ComPort.eComBaudRates extends)
		{
			return s_BaudRateMap.GetKey(extends);
		}

		public static eComDataBits FromCrestron(this ComPort.eComDataBits extends)
		{
			return s_DataBitsMap.GetKey(extends);
		}

		public static eComParityType FromCrestron(this ComPort.eComParityType extends)
		{
			return s_ParityTypeMap.GetKey(extends);
		}

		public static eComStopBits FromCrestron(this ComPort.eComStopBits extends)
		{
			return s_StopBitsMap.GetKey(extends);
		}

		public static eComProtocolType FromCrestron(this ComPort.eComProtocolType extends)
		{
			return s_ProtocolTypeMap.GetKey(extends);
		}

		public static eComHardwareHandshakeType FromCrestron(this ComPort.eComHardwareHandshakeType extends)
		{
			return s_HardwareHandshakeTypeMap.GetKey(extends);
		}

		public static eComSoftwareHandshakeType FromCrestron(this ComPort.eComSoftwareHandshakeType extends)
		{
			return s_SoftwareHandshakeTypeMap.GetKey(extends);
		}

		public static ComPort.eComBaudRates ToCrestron(this eComBaudRates extends)
		{
			return s_BaudRateMap.GetValue(extends);
		}

		public static ComPort.eComDataBits ToCrestron(this eComDataBits extends)
		{
			return s_DataBitsMap.GetValue(extends);
		}

		public static ComPort.eComParityType ToCrestron(this eComParityType extends)
		{
			return s_ParityTypeMap.GetValue(extends);
		}

		public static ComPort.eComStopBits ToCrestron(this eComStopBits extends)
		{
			return s_StopBitsMap.GetValue(extends);
		}

		public static ComPort.eComProtocolType ToCrestron(this eComProtocolType extends)
		{
			return s_ProtocolTypeMap.GetValue(extends);
		}

		public static ComPort.eComHardwareHandshakeType ToCrestron(this eComHardwareHandshakeType extends)
		{
			return s_HardwareHandshakeTypeMap.GetValue(extends);
		}

		public static ComPort.eComSoftwareHandshakeType ToCrestron(this eComSoftwareHandshakeType extends)
		{
			return s_SoftwareHandshakeTypeMap.GetValue(extends);
		}
#endif
	}
}
