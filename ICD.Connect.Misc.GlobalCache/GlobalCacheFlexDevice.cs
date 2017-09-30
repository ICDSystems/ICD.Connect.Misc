using ICD.Connect.Devices;

namespace ICD.Connect.Misc.GlobalCache
{
    public sealed class GlobalCacheFlexDevice : AbstractDevice<GlobalCacheFlexDeviceSettings>
    {
		protected override bool GetIsOnlineStatus()
		{
			throw new System.NotImplementedException();
		}
	}
}
