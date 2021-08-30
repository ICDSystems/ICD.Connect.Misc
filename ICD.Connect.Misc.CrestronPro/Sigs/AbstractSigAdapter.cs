#if !NETSTANDARD
using System;
using ICD.Common.Properties;
using ICD.Connect.Protocol.Sigs;
using Sig = Crestron.SimplSharpPro.Sig;

namespace ICD.Connect.Misc.CrestronPro.Sigs
{
	public abstract class AbstractSigAdapter<T> : ISig
		where T : Sig
	{
		private readonly T m_Sig;

		#region Properties

		protected T Sig { get { return m_Sig; } }

		/// <summary>
		/// Type of data this sig uses when communicating with the device.
		/// </summary>
		public eSigType Type { get { return GetSigType(Sig.Type); } }

		/// <summary>
		/// Number of this sig.
		/// </summary>
		public uint Number { get { return Sig.Number; } }

		/// <summary>
		/// Get/Set the name of this Sig.
		/// </summary>
		public string Name { get { return Sig.Name; } }

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="sig"></param>
		protected AbstractSigAdapter(T sig)
		{
			m_Sig = sig;
		}

		#region Methods

		/// <summary>
		/// Get the string representation of this Sig.
		/// </summary>
		/// <exception cref="T:System.InvalidOperationException">Sig is in an invalid state.</exception>
		public virtual string GetStringValue()
		{
			return Sig.StringValue;
		}

		/// <summary>
		/// Get the UShort representation of this Sig.
		/// </summary>
		/// <exception cref="T:System.InvalidOperationException">Sig is in an invalid state.</exception>
		public virtual ushort GetUShortValue()
		{
			return Sig.UShortValue;
		}

		/// <summary>
		/// Get the bool representation of this Sig.
		/// </summary>
		/// <exception cref="T:System.InvalidOperationException">Sig is in an invalid state.</exception>
		public virtual bool GetBoolValue()
		{
			return Sig.BoolValue;
		}

		public override string ToString()
		{
			return string.Format("{0} - Number {1}, Name {2}, String {3}, UShort {4}, Bool {5}", GetType().Name,
			                     Number, Name, GetStringValue(), GetUShortValue(), GetBoolValue());
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Converts a Crestron sig type to an ICD sig type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		[PublicAPI]
		protected eSigType GetSigType(Crestron.SimplSharpPro.eSigType type)
		{
			switch (type)
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
					throw new ArgumentOutOfRangeException("type");
			}
		}

		#endregion
	}
}

#endif
