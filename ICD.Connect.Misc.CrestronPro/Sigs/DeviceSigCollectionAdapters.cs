#if SIMPLSHARP
using Crestron.SimplSharpPro;
using ICD.Connect.Panels.SigCollections;
using ICD.Connect.Protocol.Sigs;

namespace ICD.Connect.Misc.CrestronPro.Sigs
{
	public sealed class DeviceStringInputCollectionAdapter :
		AbstractSigCollectionAdapter<IStringInputSig, StringInputSig>, IDeviceStringInputCollection
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public DeviceStringInputCollectionAdapter()
			: base(sig => new StringInputSigAdapter(sig))
		{
		}
	}

	public sealed class DeviceUShortInputCollectionAdapter :
		AbstractSigCollectionAdapter<IUShortInputSig, UShortInputSig>, IDeviceUShortInputCollection
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public DeviceUShortInputCollectionAdapter()
			: base(sig => new UShortInputSigAdapter(sig))
		{
		}
	}

	public sealed class DeviceBooleanInputCollectionAdapter :
		AbstractSigCollectionAdapter<IBoolInputSig, BoolInputSig>, IDeviceBooleanInputCollection
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public DeviceBooleanInputCollectionAdapter()
			: base(sig => new BoolInputSigAdapter(sig))
		{
		}
	}
}
#endif