using Crestron.SimplSharpPro;
using ICD.Connect.Protocol.Sigs;

namespace ICD.Connect.Misc.CrestronPro.Sigs
{
	public sealed class BoolInputSigAdapter : AbstractSigAdapter<BoolInputSig>, IBoolInputSig
	{
		private bool m_Cache;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="sig"></param>
		public BoolInputSigAdapter(BoolInputSig sig) : base(sig)
		{
		}

		/// <summary>
		/// Sets the bool value. Returns true if the value changed.
		/// </summary>
		public bool SetBoolValue(bool value)
		{
			if (value == m_Cache)
				return false;

			m_Cache = value;
			Sig.BoolValue = m_Cache;

			return true;
		}

		/// <summary>
		/// Gets the bool value.
		/// </summary>
		/// <returns></returns>
		public override bool GetBoolValue()
		{
			return m_Cache;
		}
	}

	public sealed class BoolOutputSigAdapter : AbstractSigAdapter<BoolOutputSig>, IBoolOutputSig
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="sig"></param>
		public BoolOutputSigAdapter(BoolOutputSig sig) : base(sig)
		{
		}
	}

	public sealed class StringInputSigAdapter : AbstractSigAdapter<StringInputSig>, IStringInputSig
	{
		private string m_Cache;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="sig"></param>
		public StringInputSigAdapter(StringInputSig sig) : base(sig)
		{
		}

		/// <summary>
		/// Property to get the last value sent to the device or to send a new value.
		/// 
		/// </summary>
		public bool SetStringValue(string value)
		{
			if (value == m_Cache)
				return false;

			m_Cache = value;
			Sig.StringValue = m_Cache;

			return true;
		}

		public override string GetStringValue()
		{
			return m_Cache;
		}
	}

	public sealed class StringOutputSigAdapter : AbstractSigAdapter<StringOutputSig>, IStringOutputSig
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="sig"></param>
		public StringOutputSigAdapter(StringOutputSig sig) : base(sig)
		{
		}
	}

	public sealed class UShortInputSigAdapter : AbstractSigAdapter<UShortInputSig>, IUShortInputSig
	{
		private ushort m_Cache;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="sig"></param>
		public UShortInputSigAdapter(UShortInputSig sig) : base(sig)
		{
		}

		/// <summary>
		/// Property to get the last value sent to the device or to send a new value.
		/// 
		/// </summary>
		public bool SetUShortValue(ushort value)
		{
			if (value == m_Cache)
				return false;

			m_Cache = value;
			Sig.UShortValue = m_Cache;

			return true;
		}

		public override ushort GetUShortValue()
		{
			return m_Cache;
		}
	}

	public sealed class UShortOutputSigAdapter : AbstractSigAdapter<UShortOutputSig>, IUShortOutputSig
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="sig"></param>
		public UShortOutputSigAdapter(UShortOutputSig sig) : base(sig)
		{
		}
	}
}
