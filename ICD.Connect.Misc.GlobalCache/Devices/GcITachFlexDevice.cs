using ICD.Connect.Devices;

namespace ICD.Connect.Misc.GlobalCache.Devices
{
    public sealed class GcITachFlexDevice : AbstractDevice<GcITachFlexDeviceSettings>
	{
		private const ushort TCP_PORT = 4998;

		protected override bool GetIsOnlineStatus()
		{
			return false;
		}
	}
}
