#if !NETSTANDARD
using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp.Reflection;
using Crestron.SimplSharpPro;
using ICD.Common.Properties;

namespace ICD.Connect.Misc.CrestronPro.Extensions
{
	public static class DeviceExtenderExtensions
	{
		/// <summary>
		/// Gets the available output sigs for the extender.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static IEnumerable<Sig> GetOutputSigs([NotNull] this DeviceExtender extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.GetType()
			              .GetCType()
			              .GetProperties(BindingFlags.Public |
			                             BindingFlags.Instance |
			                             BindingFlags.FlattenHierarchy)
			              .Where(p => typeof(StringOutputSig).IsAssignableFrom(p.PropertyType) ||
			                          typeof(StringOutputSig).IsAssignableFrom(p.PropertyType) ||
			                          typeof(StringOutputSig).IsAssignableFrom(p.PropertyType))
			              .Select(p => p.GetValue(extends, null))
			              .OfType<Sig>();
		}
	}
}
#endif
