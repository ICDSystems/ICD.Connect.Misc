﻿#if !NETSTANDARD
using System;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpProInternal;
#endif

namespace ICD.Connect.Misc.CrestronPro.Extensions
{
	public static class PortDeviceExtensions
	{
#if !NETSTANDARD
		/// <summary>
		/// PortDevice.IsOnline property is worthless, so this method provides a more reliable
		/// check to determine if the port is currently available for communication.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static bool GetIsRegisteredAndParentOnline(this PortDevice extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			return extends.Registered && extends.GetParentOnline();
		}

		public static bool GetParentOnline(this PortDevice extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			CrestronControlSystem controlSystem = extends.Parent as CrestronControlSystem;
			if (controlSystem != null)
				return true;

			GenericBase parent = extends.Parent as GenericBase;
			return parent != null && parent.IsOnline;
		}
#endif
	}
}