﻿#if !NETSTANDARD
using System;
using Crestron.SimplSharpPro;

namespace ICD.Connect.Misc.CrestronPro.Extensions
{
	public static class GenericBaseExtensions
	{
		/// <summary>
		/// Unregisters the device if it is registered, and then registers the device.
		/// </summary>
		/// <param name="extends"></param>
		public static eDeviceRegistrationUnRegistrationResponse ReRegister(this GenericBase extends)
		{
			if (extends == null)
				throw new ArgumentNullException("extends");

			if (extends.Registered)
			{
				eDeviceRegistrationUnRegistrationResponse output = extends.UnRegister();
				if (output != eDeviceRegistrationUnRegistrationResponse.Success)
					return output;
			}

			return extends.Register();
		}
	}
}

#endif
