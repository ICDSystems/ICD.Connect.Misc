#if SIMPLSHARP
using System;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpProInternal;

namespace ICD.Connect.Misc.CrestronPro.Utils
{
	public static class PortDeviceUtils
	{
		/// <summary>
		/// Unregisters the given port.
		/// </summary>
		/// <param name="port"></param>
		public static void Unregister(PortDevice port)
		{
			if (port == null)
				throw new ArgumentNullException("port");
			
			if (!port.Registered)
				return;

			port.UnRegister();
		}

		/// <summary>
		/// Registers the port and then re-registers the parent.
		/// </summary>
		/// <param name="port"></param>
		public static void Register(PortDevice port)
		{
			if (port == null)
				throw new ArgumentNullException("port");

			eDeviceRegistrationUnRegistrationResponse result = port.Register();

			switch (result)
			{
				case eDeviceRegistrationUnRegistrationResponse.Success:
				case eDeviceRegistrationUnRegistrationResponse.NoAttempt:
					break;

				// If result is ParentRegistered, we have to unregister and re-register the parent after
				case eDeviceRegistrationUnRegistrationResponse.ParentRegistered:
					GenericDevice parent = port.Parent as GenericDevice;
					if (parent == null)
						throw new InvalidOperationException("No parent device");

					// Unregiser Parent
					eDeviceRegistrationUnRegistrationResponse parentResult = parent.UnRegister();
					if (parentResult != eDeviceRegistrationUnRegistrationResponse.Success)
						throw new InvalidOperationException(string.Format("Parent unregistration failed: {0}", parentResult));

					// Register Port
					result = port.Register();
					if (result != eDeviceRegistrationUnRegistrationResponse.Success)
						throw new InvalidOperationException(string.Format("Unable to register {0}: {1}", port.GetType().Name, result));

					// Register Parent
					parentResult = parent.Register();
					if (parentResult != eDeviceRegistrationUnRegistrationResponse.Success)
						throw new InvalidOperationException(string.Format("Parent registration failed: {0}", parentResult));
					break;

				default:
					throw new InvalidOperationException(string.Format("Unable to register {0}: {1}",  port.GetType().Name, result));
			}
		}
	}
}
#endif
