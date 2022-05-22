using ICD.Connect.Panels.Devices;
using ICD.Connect.Panels.SigCollections;
using ICD.Connect.Settings;
#if !NETSTANDARD
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Misc.CrestronPro.Extensions;
using ICD.Connect.Misc.CrestronPro.Utils;
using ICD.Connect.Protocol.Sigs;
using ICD.Connect.Misc.CrestronPro.Sigs;
using Crestron.SimplSharpPro.EthernetCommunication;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
#else
using System;
#endif

namespace ICD.Connect.Misc.CrestronPro.Eisc
{
	public sealed class EiscAdapter : AbstractSigDevice<EiscAdapterSettings>
	{
		public byte EiscIpid { get; private set; }

		public string EiscAddress { get; private set; }

#if !NETSTANDARD
		private EthernetIntersystemCommunications m_Eisc;

		private readonly DeviceBooleanInputCollectionAdapter m_BooleanInput;
		private readonly DeviceUShortInputCollectionAdapter m_UShortInput;
		private readonly DeviceStringInputCollectionAdapter m_StringInput;
		private readonly DeviceBooleanOutputCollectionAdapter m_BooleanOutput;
		private readonly DeviceUShortOutputCollectionAdapter m_UShortOutput;
		private readonly DeviceStringOutputCollectionAdapter m_StringOutput;
#endif


		public EiscAdapter()
		{
#if !NETSTANDARD

			m_BooleanInput = new DeviceBooleanInputCollectionAdapter();
			m_UShortInput = new DeviceUShortInputCollectionAdapter();
			m_StringInput = new DeviceStringInputCollectionAdapter();
			m_BooleanOutput = new DeviceBooleanOutputCollectionAdapter();
			m_UShortOutput = new DeviceUShortOutputCollectionAdapter();
			m_StringOutput = new DeviceStringOutputCollectionAdapter();

#endif
		}

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
#if !NETSTANDARD
			return m_Eisc != null && m_Eisc.IsOnline;
#else
			return false;
#endif
		}

		/// <summary>
		/// Collection of Boolean Inputs sent to the panel.
		/// </summary>
		public override IDeviceBooleanInputCollection BooleanInput
		{
			get
			{
#if !NETSTANDARD
				return m_BooleanInput;
#else
				throw new NotSupportedException();
#endif
			}
		}

		/// <summary>
		/// Collection of Integer Inputs sent to the panel.
		/// </summary>
		public override IDeviceUShortInputCollection UShortInput
		{
			get
			{
#if !NETSTANDARD
				return m_UShortInput;
#else
				throw new NotSupportedException();
#endif
			}
		}

		/// <summary>
		/// Collection of String Inputs sent to the panel.
		/// </summary>
		public override IDeviceStringInputCollection StringInput
		{
			get
			{
#if !NETSTANDARD
				return m_StringInput;
#else
				throw new NotSupportedException();
#endif
			}
		}

		/// <summary>
		/// Collection of Boolean Outputs sent from the panel.
		/// </summary>
		public override IDeviceBooleanOutputCollection BooleanOutput
		{
			get
			{
#if !NETSTANDARD
				return m_BooleanOutput;
#else
				throw new NotSupportedException();
#endif
			}
		}

		/// <summary>
		/// Collection of Integer Outputs sent from the panel.
		/// </summary>
		public override IDeviceUShortOutputCollection UShortOutput
		{
			get
			{
#if !NETSTANDARD
				return m_UShortOutput;
#else
				throw new NotSupportedException();
#endif
			}
		}

		/// <summary>
		/// Collection of String Outputs sent from the panel.
		/// </summary>
		public override IDeviceStringOutputCollection StringOutput
		{
			get
			{
#if !NETSTANDARD
				return m_StringOutput;
#else
				throw new NotSupportedException();
#endif
			}
		}

#if !NETSTANDARD
		private void SetEisc(EthernetIntersystemCommunications eisc)
		{
			if (m_Eisc != null)
			{
				Unsubscribe(m_Eisc);
				eDeviceRegistrationUnRegistrationResponse tearDownResult;
				if (GenericBaseUtils.TearDown(m_Eisc, out tearDownResult))
					Logger.Log(eSeverity.Error, "Unable to unregister {0} - {1}", m_Eisc.GetType().Name, tearDownResult);

			}

			m_Eisc = eisc;

			if (m_Eisc != null)
			{

				eDeviceRegistrationUnRegistrationResponse setUpResult;
				if (!GenericBaseUtils.SetUp(m_Eisc, this, out setUpResult))
					Logger.Log(eSeverity.Error, "Unable to register {0} - {1}", m_Eisc.GetType().Name, setUpResult);
			}

			m_BooleanInput.SetCollection(m_Eisc == null ? null : m_Eisc.BooleanInput);
			m_UShortInput.SetCollection(m_Eisc == null ? null : m_Eisc.UShortInput);
			m_StringInput.SetCollection(m_Eisc == null ? null : m_Eisc.StringInput);
			m_BooleanOutput.SetCollection(m_Eisc == null ? null : m_Eisc.BooleanOutput);
			m_UShortOutput.SetCollection(m_Eisc == null ? null : m_Eisc.UShortOutput);
			m_StringOutput.SetCollection(m_Eisc == null ? null : m_Eisc.StringOutput);

			Subscribe(m_Eisc);

			UpdateCachedOnlineStatus();

		}

		private void Subscribe(EthernetIntersystemCommunications eisc)
		{
			if (eisc == null)
				return;

			eisc.SigChange += EiscOnSigChange;
			eisc.OnlineStatusChange += EiscOnlineStatusChange;

		}

		private void EiscOnSigChange(BasicTriList currentdevice, SigEventArgs args)
		{
			SigInfo sigInfo = args.Sig.ToSigInfo();
			RaiseOutputSigChangeCallback(sigInfo);
		}

		private void EiscOnlineStatusChange(GenericBase currentdevice, OnlineOfflineEventArgs args)
		{
			UpdateCachedOnlineStatus();
		}

		private void Unsubscribe(EthernetIntersystemCommunications eisc)
		{
			if (eisc == null)
				return;

			eisc.SigChange -= EiscOnSigChange;
			eisc.OnlineStatusChange -= EiscOnlineStatusChange;

		}
#endif

		#region Settings

		protected override void ApplySettingsFinal(EiscAdapterSettings settings, IDeviceFactory factory)
		{

			base.ApplySettingsFinal(settings, factory);

			EiscIpid = settings.EiscIpid;
			EiscAddress = settings.EiscAddress;
#if !NETSTANDARD
			SetEisc(new EthernetIntersystemCommunications(EiscIpid, EiscAddress, ProgramInfo.ControlSystem));
#else
			throw new NotSupportedException();
#endif
		}

		protected override void CopySettingsFinal(EiscAdapterSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.EiscIpid = EiscIpid;
			settings.EiscAddress = EiscAddress;
		}

		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			EiscIpid = default(byte);
			EiscAddress = null;

#if !NETSTANDARD
			SetEisc(null);
#endif

		}

		#endregion
	}
}