using System.Collections.Generic;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.ThreeSeriesCards;
using Crestron.SimplSharpProInternal;
using ICD.Common.Properties;
using ICD.Common.Services.Logging;
using ICD.Connect.Devices;
using ICD.Connect.Devices.Extensions;
using ICD.Connect.Misc.CrestronPro.Devices.CardFrames;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Misc.CrestronPro.Devices.Cards
{
	public abstract class AbstractCardAdapter<TCard, TSettings> : AbstractDevice<TSettings>, IPortParent
		where TCard : C3Card
		where TSettings : AbstractCardAdapterSettings, new()
	{
		// Used with settings.
		private int? m_ParentId;

		/// <summary>
		/// Gets the wrapped card.
		/// </summary>
		public TCard Card { get; private set; }

		#region Methods

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
					Logger.AddEntry(eSeverity.Error, "Unable to register {0} - {1}", Card.GetType().Name, result);
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
			string message = string.Format("{0} has no {1} with address {2}", this, typeof(ComPort).Name, address);
			throw new KeyNotFoundException(message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public virtual IROutputPort GetIrOutputPort(int address)
		{
			string message = string.Format("{0} has no {1} with address {2}", this, typeof(IROutputPort).Name, address);
			throw new KeyNotFoundException(message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public virtual Relay GetRelayPort(int address)
		{
			string message = string.Format("{0} has no {1} with address {2}", this, typeof(Relay).Name, address);
			throw new KeyNotFoundException(message);
		}

		/// <summary>
		/// Gets the port at the given address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public virtual Versiport GetIoPort(int address)
		{
			string message = string.Format("{0} has no {1} with address {2}", this, typeof(Versiport).Name, address);
			throw new KeyNotFoundException(message);
		}

		#endregion

		#region Settings

		/// <summary>
		/// Override to apply properties to the settings instance.
		/// </summary>
		/// <param name="settings"></param>
		protected override void CopySettingsFinal(TSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Ipid = Card == null ? (byte?)null : (byte)Card.ID;
			settings.CardFrame = m_ParentId;
		}

		/// <summary>
		/// Override to clear the instance settings.
		/// </summary>
		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			SetCard(null, null);
		}

		/// <summary>
		/// Override to apply settings to the instance.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="factory"></param>
		protected override void ApplySettingsFinal(TSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			if (!settings.CardFrame.HasValue)
			{
				Logger.AddEntry(eSeverity.Error, "Unable to instantiate {0} - No CardFrame DeviceId specified.", typeof(TCard).Name);
				return;
			}

			TCard card = InstantiateCard(settings.Ipid, settings.CardFrame.Value, factory);
			SetCard(card, settings.CardFrame);
		}

		/// <summary>
		/// Instantiates the internal card based on provided parameters
		/// </summary>
		/// <param name="ipid"></param>
		/// <param name="cardFrameId"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		[CanBeNull]
		private TCard InstantiateCard(byte? ipid, int cardFrameId, IDeviceFactory factory)
		{
			IDevice cardFrame = factory.GetDeviceById(cardFrameId);

			// If an IPID is specified the CardFrame has multiple slots
			if (ipid.HasValue)
			{
				CenCi33Adapter ci33 = cardFrame as CenCi33Adapter;
				if (ci33 != null)
					return InstantiateCard(ipid.Value, ci33.CardFrame);
				Logger.AddEntry(eSeverity.Error, "Device {0} is not a {1}.", cardFrameId, typeof(CenCi33Adapter).Name);
			}
			else
			{
				CenCi31Adapter ci31 = cardFrame as CenCi31Adapter;
				if (ci31 != null)
					return InstantiateCard(ci31.CardFrame);
				Logger.AddEntry(eSeverity.Error, "Device {0} is not a {1}.", cardFrameId, typeof(CenCi31Adapter).Name);
			}

			return null;
		}

		/// <summary>
		/// Instantiates the card for the given card frame parent.
		/// </summary>
		/// <param name="cardFrame"></param>
		/// <returns></returns>
		protected abstract TCard InstantiateCard(CenCi31 cardFrame);

		/// <summary>
		/// Instantiates the card for the given card frame parent.
		/// </summary>
		/// <param name="ipid"></param>
		/// <param name="cardFrame"></param>
		/// <returns></returns>
		protected abstract TCard InstantiateCard(byte ipid, CenCi33 cardFrame);

		#endregion

		#region Card Callbacks

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

		/// <summary>
		/// Gets the current online status of the device.
		/// </summary>
		/// <returns></returns>
		protected override bool GetIsOnlineStatus()
		{
			return Card != null && Card.IsOnline;
		}

		#endregion
	}
}
