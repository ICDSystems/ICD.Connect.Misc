#if !NETSTANDARD
using System;
using Crestron.SimplSharpPro;
using ICD.Connect.Protocol.Sigs;
using eSigType = ICD.Connect.Protocol.Sigs.eSigType;

namespace ICD.Connect.Misc.CrestronPro.Extensions
{
	public static class SigExtensions
	{
		/// <summary>
		/// Converts the Sig to a SigInfo.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static SigInfo ToSigInfo(this Sig extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.ToSigInfo(0);
		}

		/// <summary>
		/// Converts the Sig to a SigInfo.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="smartObjectId"></param>
		/// <returns></returns>
		public static SigInfo ToSigInfo(this Sig extends, ushort smartObjectId)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			switch (extends.GetSigType())
			{
				case eSigType.Na:
					return new SigInfo(extends.Number, extends.Name, smartObjectId);

				case eSigType.Digital:
					return new SigInfo(extends.Number, extends.Name, smartObjectId, extends.BoolValue);

				case eSigType.Analog:
					return new SigInfo(extends.Number, extends.Name, smartObjectId, extends.UShortValue);

				case eSigType.Serial:
					return new SigInfo(extends.Number, extends.Name, smartObjectId, extends.StringValue);

				default:
					throw new ArgumentOutOfRangeException("extends");
			}
		}

		/// <summary>
		/// Returns the sig type for the given Sig.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static eSigType GetSigType(this Sig extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			switch (extends.Type)
			{
				case Crestron.SimplSharpPro.eSigType.NA:
					return eSigType.Na;

				case Crestron.SimplSharpPro.eSigType.Bool:
					return eSigType.Digital;

				case Crestron.SimplSharpPro.eSigType.UShort:
					return eSigType.Analog;

				case Crestron.SimplSharpPro.eSigType.String:
					return eSigType.Serial;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Returns null if the sig is a nullsig.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static string GetSerialValueOrDefault(this StringOutputSig extends)
		{
			if (extends == null)
				return null;

			return extends.Type == Crestron.SimplSharpPro.eSigType.NA ? null : extends.StringValue;
		}

		/// <summary>
		/// Returns false if the sig is a nullsig.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static bool GetBoolValueOrDefault(this BoolOutputSig extends)
		{
			if (extends == null)
				return false;

			return extends.Type != Crestron.SimplSharpPro.eSigType.NA && extends.BoolValue;
		}

		/// <summary>
		/// Returns 0 if the sig is a nullsig.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static short GetShortValueOrDefault(this UShortOutputSig extends)
		{
			if (extends == null)
				return 0;

			return extends.Type == Crestron.SimplSharpPro.eSigType.NA ? (short)0 : extends.ShortValue;
		}

		/// <summary>
		/// Returns 0 if the sig is a nullsig.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static ushort GetUShortValueOrDefault(this UShortOutputSig extends)
		{
			if (extends == null)
				return 0;

			return extends.Type == Crestron.SimplSharpPro.eSigType.NA ? (ushort)0 : extends.UShortValue;
		}
	}
}

#endif