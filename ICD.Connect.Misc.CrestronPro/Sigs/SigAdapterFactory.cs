using System;
using Crestron.SimplSharpPro;
using ICD.Connect.Protocol.Sigs;
using Sig = Crestron.SimplSharpPro.Sig;

namespace ICD.Connect.Misc.CrestronPro.Sigs
{
	public static class SigAdapterFactory
	{
		/// <summary>
		/// Returns an ISig for the given sig.
		/// </summary>
		/// <param name="sig"></param>
		/// <returns></returns>
		public static ISig GetSigAdapter(Sig sig)
		{
			if (sig is BoolInputSig)
				return new BoolInputSigAdapter(sig as BoolInputSig);
			if (sig is BoolOutputSig)
				return new BoolOutputSigAdapter(sig as BoolOutputSig);
			if (sig is UShortInputSig)
				return new UShortInputSigAdapter(sig as UShortInputSig);
			if (sig is UShortOutputSig)
				return new UShortOutputSigAdapter(sig as UShortOutputSig);
			if (sig is StringInputSig)
				return new StringInputSigAdapter(sig as StringInputSig);
			if (sig is StringOutputSig)
				return new StringOutputSigAdapter(sig as StringOutputSig);

			throw new ArgumentOutOfRangeException();
		}
	}
}
