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
			: this(null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="inputCollection"></param>
		public DeviceStringInputCollectionAdapter(DeviceStringInputCollection inputCollection)
			: base(sig => new StringInputSigAdapter(sig), inputCollection)
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
			: this(null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="inputCollection"></param>
		public DeviceUShortInputCollectionAdapter(DeviceUShortInputCollection inputCollection)
			: base(sig => new UShortInputSigAdapter(sig), inputCollection)
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
			: this(null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="inputCollection"></param>
		public DeviceBooleanInputCollectionAdapter(DeviceBooleanInputCollection inputCollection)
			: base(sig => new BoolInputSigAdapter(sig), inputCollection)
		{
		}
	}
}

#endif
