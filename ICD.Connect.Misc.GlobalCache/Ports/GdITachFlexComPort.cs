using ICD.Connect.Protocol.Ports.ComPort;

namespace ICD.Connect.Misc.GlobalCache.Ports
{
    public sealed class GdITachFlexComPort : AbstractComPort<GdITachFlexComPortSettings>
    {
		protected override bool SendFinal(string data)
		{
			throw new System.NotImplementedException();
		}

		public override int SetComPortSpec(eComBaudRates baudRate, eComDataBits numberOfDataBits, eComParityType parityType,
										   eComStopBits numberOfStopBits, eComProtocolType protocolType, eComHardwareHandshakeType hardwareHandShake,
										   eComSoftwareHandshakeType softwareHandshake, bool reportCtsChanges)
		{
			throw new System.NotImplementedException();
		}
	}
}
