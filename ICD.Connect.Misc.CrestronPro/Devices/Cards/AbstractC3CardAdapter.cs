using System;
using System.Collections.Generic;
using ICD.Common.Properties;
using ICD.Common.Utils.Services.Logging;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Extensions;
using ICD.Connect.Misc.CrestronPro.Devices.CardFrames;
using ICD.Connect.Settings;
#if SIMPLSHARP
using Crestron.SimplSharpPro;
using Crestron.SimplSharpProInternal;
using Crestron.SimplSharpPro.DM;
#endif

namespace ICD.Connect.Misc.CrestronPro.Devices.Cards
{
#if SIMPLSHARP
	public abstract class AbstractC3CardAdapter<TCard, TSettings> : AbstractDevice<TSettings>, IPortParent
		where TCard : C3Card
#else
    public abstract class AbstractC3CardAdapter<TSettings> : AbstractDevice<TSettings>
#endif
		where TSettings : AbstractC3CardAdapterSettings, new()
	{
		// Used with settings.
		private int? m_ParentId;

#if SIMPLSHARP
		/// <summary>
		/// Gets the wrapped card.
		/// </summary>
		public TCard Card { get; private set; }
#endif

		#region Methods

#if SIMPLSHARP
		/// <summary>
		/// Sets the wrapped card device.
		/// </summary>
		/// <param name="card"></param>
		/// <param name="parentId"></param>
		public void SetCard(TCard card, int? parentId)
		{
			m_ParentId = parentId;

			Unsubscribe(Card);

			if (Card != null)
			{
				if (Card.Registered)
					Card.UnRegister();

				try
				{
					Card.Dispose();
				}
				catch
				{
				}
			}

			Card = card;

			if (Card != null && !Card.Registered)
			{
				if (Name != null)
					Card.Description = Name;

				eDeviceRegistrationUnRegistrationResponse result = Card.Register();
				if (result != eDeviceRegistrationUnRegistrationResponse.Success)
					Log(eSeverity.Error, "Unable to register {0} - {1}", Card.GetType().Name, result);
			}

			Subscribe(Card);
			UpdateCachedOnlineStatus();
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public virtual ComPort GetComPort(int address)
		{
			string message = string.Format("{0} has no {1}", this, typeof(ComPort).Name);
			throw new NotSupportedException(message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public virtual IROutputPort GetIrOutputPort(int address)
		{
			string message = string.Format("{0} has no {1}", this, typeof(IROutputPort).Name);
			throw new NotSupportedException(message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public virtual Relay GetRelayPort(int address)
		{
			string message = string.Format("{0} has no {1}", this, typeof(Relay).Name);
			throw new NotSupportedException(message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public virtual Versiport GetIoPort(int address)
		{
			string message = string.Format("{0} has no {1}", this, typeof(Versiport).Name);
			throw new NotSupportedException(message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public virtual DigitalInput GetDigitalInputPort(int address)
		{
			string message = string.Format("{0} has no {1}", this, typeof(DigitalInput).Name);
			throw new NotSupportedException(message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="io"></param>
		/// <param name="address"></param>
		/// <returns></returns>
		public virtual Cec GetCecPort(eInputOuptut io, int address)
		{
			string message = string.Format("{0} has no {1}", this, typeof(Cec).Name);
			throw new ArgumentOutOfRangeException("address", message);
		}
#endif

		#endregion

		#region Settings

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(TSettings settings)
		{
			base.CopySettingsFinal(settings);

#if SIMPLSHARP
			settings.CardId = Card == null ? 0 : Card.ID;
#else
            settings.CardId = 0;
#endif
			settings.CardFrame = m_ParentId;
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

#if SIMPLSHARP
			SetCard(null, null);
#endif
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(TSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

#if SIMPLSHARP
			TCard card = null;

			if (settings.CardFrame.HasValue)
				card = InstantiateCard(settings.CardId, settings.CardFrame.Value, factory);
			else
				Log(eSeverity.Warning, "No CardFrame ID specified, unable to instantiate internal card");

			SetCard(card, settings.CardFrame);
#endif
		}

#if SIMPLSHARP
		/// <summary>
		/// Instantiates the internal card based on provided parameters
		/// </summary>
		/// <param name="cardId"></param>
		/// <param name="cardFrameId"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		[CanBeNull]
		private TCard InstantiateCard(uint? cardId, int cardFrameId, IDeviceFactory factory)
		{
			IDevice cardFrame;

			try
			{
				cardFrame = factory.GetDeviceById(cardFrameId);
			}
			catch (KeyNotFoundException)
			{
				Log(eSeverity.Error, "No device with id {0}", cardFrameId);
				return null;
			}

			// If an IPID is specified the CardFrame has multiple slots
			if (cardId.HasValue)
			{
				CenCi33Adapter ci33 = cardFrame as CenCi33Adapter;
				if (ci33 != null)
					return InstantiateCard(cardId.Value, ci33.CardFrame);
				Log(eSeverity.Error, "Device {0} is not a {1}.", cardFrameId, typeof(CenCi33Adapter).Name);
			}
			else
			{
				CenCi31Adapter ci31 = cardFrame as CenCi31Adapter;
				if (ci31 != null)
					return InstantiateCard(ci31.CardFrame);
				Log(eSeverity.Error, "Device {0} is not a {1}.", cardFrameId, typeof(CenCi31Adapter).Name);
			}

			return null;
		}

		/// <summary>
		/// Instantiates the card for the given card frame parent.
		/// </summary>
		/// <param name="cardFrame"></param>
		/// <returns></returns>
		protected abstract TCard InstantiateCard(Ci3SingleCardCage cardFrame);

		/// <summary>
		/// Instantiates the card for the given card frame parent.
		/// </summary>
		/// <param name="cardId"></param>
		/// <param name="cardFrame"></param>
		/// <returns></returns>
		protected abstract TCard InstantiateCard(uint cardId, Ci3MultiCardCage cardFrame);
#endif

		#endregion

		#region Card Callbacks

#if SIMPLSHARP
		/// <summary>
		/// Subscribe to the card events.
		/// </summary>
		/// <param name="card"></param>
		private void Subscribe(TCard card)
		{
			if (card == null)
				return;

			card.OnlineStatusChange += CardOnLineStatusChange;
		}

		/// <summary>
		/// Unsubscribe from the card events.
		/// </summary>
		/// <param name="card"></param>
		private void Unsubscribe(TCard card)
		{
			if (card == null)
				return;

			card.OnlineStatusChange += CardOnLineStatusChange;
		}

		/// <summary>
		/// Called when the card online status changes.
		/// </summary>
		/// <param name="currentDevice"></param>
		/// <param name="args"></param>
		private void CardOnLineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
		{
			UpdateCachedOnlineStatus();
		}
#endif

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
#if SIMPLSHARP
			return Card != null && Card.IsOnline;
#else
            return false;
#endif
		}

		#endregion
	}
}
