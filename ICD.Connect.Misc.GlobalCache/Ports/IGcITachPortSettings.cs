using ICD.Connect.Protocol.Ports;

namespace ICD.Connect.Misc.GlobalCache.Ports
{
	public interface IGcITachPortSettings : IPortSettings
	{
		/// <summary>
		/// Gets/sets the parent iTach device id.
		/// </summary>
		int? Device { get; set; }

		/// <summary>
		/// Gets/sets the module index.
		/// </summary>
		int Module { get; set; }

		/// <summary>
		/// Gets/sets the port index.
		/// </summary>
		int Address { get; set; }
	}
}
