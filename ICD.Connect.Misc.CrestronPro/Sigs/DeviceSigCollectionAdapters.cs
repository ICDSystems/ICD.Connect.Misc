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
		/// <param name="collection"></param>
		public DeviceStringInputCollectionAdapter(DeviceStringInputCollection collection)
			: base(collection, sig => new StringInputSigAdapter(sig))
		{
		}
	}

	public sealed class DeviceStringOutputCollectionAdapter :
		AbstractSigCollectionAdapter<IStringOutputSig, StringOutputSig>, IDeviceStringOutputCollection
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="collection"></param>
		public DeviceStringOutputCollectionAdapter(DeviceStringOutputCollection collection)
			: base(collection, sig => new StringOutputSigAdapter(sig))
		{
		}
	}

	public sealed class DeviceUShortInputCollectionAdapter :
		AbstractSigCollectionAdapter<IUShortInputSig, UShortInputSig>, IDeviceUShortInputCollection
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="collection"></param>
		public DeviceUShortInputCollectionAdapter(DeviceUShortInputCollection collection)
			: base(collection, sig => new UShortInputSigAdapter(sig))
		{
		}
	}

	public sealed class DeviceUShortOutputCollectionAdapter :
		AbstractSigCollectionAdapter<IUShortOutputSig, UShortOutputSig>, IDeviceUShortOutputCollection
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="collection"></param>
		public DeviceUShortOutputCollectionAdapter(DeviceUShortOutputCollection collection)
			: base(collection, sig => new UShortOutputSigAdapter(sig))
		{
		}
	}

	public sealed class DeviceBooleanInputCollectionAdapter :
		AbstractSigCollectionAdapter<IBoolInputSig, BoolInputSig>, IDeviceBooleanInputCollection
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="collection"></param>
		public DeviceBooleanInputCollectionAdapter(DeviceBooleanInputCollection collection)
			: base(collection, sig => new BoolInputSigAdapter(sig))
		{
		}
	}

	public sealed class DeviceBooleanOutputCollectionAdapter :
		AbstractSigCollectionAdapter<IBoolOutputSig, BoolOutputSig>, IDeviceBooleanOutputCollection
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="collection"></param>
		public DeviceBooleanOutputCollectionAdapter(DeviceBooleanOutputCollection collection)
			: base(collection, sig => new BoolOutputSigAdapter(sig))
		{
		}
	}
}
#endif