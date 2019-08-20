using System.Text;
using ICD.Connect.Misc.CrestronPro.Extensions;
#if SIMPLSHARP
using System;
using System.Collections.Generic;
using Crestron.SimplSharpPro;
using ICD.Connect.Settings.Originators;
#endif

namespace ICD.Connect.Misc.CrestronPro.Utils
{
	public static class GenericBaseUtils
	{
#if SIMPLSHARP
		private static readonly Dictionary<eDeviceRegistrationUnRegistrationResponse, bool>
			s_RegistrationSuccess = new Dictionary<eDeviceRegistrationUnRegistrationResponse, bool>
			{
				{eDeviceRegistrationUnRegistrationResponse.NoAttempt, true},
				{eDeviceRegistrationUnRegistrationResponse.Failure, false},
				{eDeviceRegistrationUnRegistrationResponse.Success, true},
				{eDeviceRegistrationUnRegistrationResponse.NonRegisterableDevice, true},
				{eDeviceRegistrationUnRegistrationResponse.IntegerParameterNotSet, false},
				{eDeviceRegistrationUnRegistrationResponse.StringParameterNotSet, false},
				{eDeviceRegistrationUnRegistrationResponse.Incompatible, false},
				{eDeviceRegistrationUnRegistrationResponse.ParentRegistered, true},
				{eDeviceRegistrationUnRegistrationResponse.NotLicensed, false}
			};

		/// <summary>
		/// Sets up device for the given originator.
		/// Returns false if registration failed, true if registration was successful or not necessary (e.g. no attempt).
		/// </summary>
		/// <param name="device"></param>
		/// <param name="parent"></param>
		/// <param name="result"></param>
		public static bool SetUp(GenericBase device, IOriginator parent, out eDeviceRegistrationUnRegistrationResponse result)
		{
			if (device == null)
				throw new ArgumentNullException("device");

			if (parent == null)
				throw new ArgumentNullException("parent");

			return SetUp(device, parent, d => { }, out result);
		}

		/// <summary>
		/// Sets up device for the given originator.
		/// Returns false if registration failed, true if registration was successful or not necessary (e.g. no attempt).
		/// </summary>
		/// <param name="device"></param>
		/// <param name="parent"></param>
		/// <param name="preRegistration"></param>
		/// <param name="result"></param>
		public static bool SetUp<T>(T device, IOriginator parent, Action<T> preRegistration,
		                            out eDeviceRegistrationUnRegistrationResponse result)
			where T : GenericBase
		{
			if (device == null)
				throw new ArgumentNullException("device");

			if (parent == null)
				throw new ArgumentNullException("parent");

			if (preRegistration == null)
				throw new ArgumentNullException("preRegistration");

			SetDescription(device, parent);

			preRegistration(device);

			return Register(device, out result);
		}

		/// <summary>
		/// Unregisters and disposes the given device.
		/// </summary>
		/// <param name="device"></param>
		public static void TearDown(GenericBase device)
		{
			if (device == null)
				throw new ArgumentNullException("device");

			eDeviceRegistrationUnRegistrationResponse unused;
			TearDown(device, out unused);
		}

		/// <summary>
		/// Unregisters and disposes the given device.
		/// </summary>
		/// <param name="device"></param>
		/// <param name="result"></param>
		public static bool TearDown(GenericBase device, out eDeviceRegistrationUnRegistrationResponse result)
		{
			if (device == null)
				throw new ArgumentNullException("device");

			result = eDeviceRegistrationUnRegistrationResponse.NoAttempt;
			if (device.Registered)
				UnRegister(device, out result);

			try
			{
				device.Dispose();
			}
			catch
			{
				// I forget why this was throwing in the past, ObjectDisposedException?
			}

			return s_RegistrationSuccess[result];
		}

		/// <summary>
		/// Registers the device. Returns false if registration failed, true if registration was successful
		/// or not necessary (e.g. no attempt).
		/// 
		/// Will also attempt to re-register the parent device if present.
		/// </summary>
		/// <param name="device"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public static bool Register(GenericBase device, out eDeviceRegistrationUnRegistrationResponse result)
		{
			if (device == null)
				throw new ArgumentNullException("device");

			result = device.Register();
			if (!s_RegistrationSuccess[result])
				return false;

			GenericDevice parent = device.Parent as GenericDevice;
			if (parent == null)
				return true;

			eDeviceRegistrationUnRegistrationResponse parentResult = parent.ReRegister();
			return s_RegistrationSuccess[parentResult];
		}

		/// <summary>
		/// Unregisters the device. Returns false if unregistration failed, true if unregistration was successful
		/// or not necessary (e.g. no attempt).
		/// </summary>
		/// <param name="device"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public static bool UnRegister(GenericBase device, out eDeviceRegistrationUnRegistrationResponse result)
		{
			if (device == null)
				throw new ArgumentNullException("device");

			result = device.UnRegister();
			return s_RegistrationSuccess[result];
		}

		/// <summary>
		/// Updates the Crestron device description to represent the given parent. 
		/// </summary>
		/// <param name="device"></param>
		/// <param name="parent"></param>
		public static void SetDescription(GenericBase device, IOriginator parent)
		{
			if (device == null)
				throw new ArgumentNullException("device");

			if (parent == null)
				throw new ArgumentNullException("parent");

			if (device.Registered)
				throw new InvalidOperationException("Unable to set device description after registration");

			device.Description = GetDescription(parent);
		}

		/// <summary>
		/// Gets the description string for the given originator parent.
		/// </summary>
		/// <param name="parent"></param>
		/// <returns></returns>
		public static string GetDescription(IOriginator parent)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");

			// Limited to 50 characters! Everything else gets truncated.
			StringBuilder builder = new StringBuilder();
			{
				builder.AppendFormat("Id={0}", parent.Id);

				if (!string.IsNullOrEmpty(parent.CombineName))
					builder.AppendFormat(",CN={0}", parent.CombineName);
				else if (!string.IsNullOrEmpty(parent.Name))
					builder.AppendFormat(",N={0}", parent.Name);
			}
			return builder.ToString();
		}
#endif
	}
}
