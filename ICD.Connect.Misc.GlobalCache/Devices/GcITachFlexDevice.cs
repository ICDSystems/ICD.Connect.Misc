using System;
using System.Collections.Generic;
using ICD.Common.Utils;
using ICD.Common.Utils.EventArguments;
using ICD.Connect.API.Nodes;
using ICD.Connect.Devices;
using ICD.Connect.Misc.GlobalCache.FlexApi;
using ICD.Connect.Protocol.Network.Tcp;
using ICD.Connect.Protocol.SerialBuffers;
using ICD.Connect.Settings.Core;

namespace ICD.Connect.Misc.GlobalCache.Devices
{
    public sealed class GcITachFlexDevice : AbstractDevice<GcITachFlexDeviceSettings>
	{
		private const ushort TCP_PORT = 4998;

		private readonly AsyncTcpClient m_Client;
		private readonly DelimiterSerialBuffer m_Buffer;

		/// <summary>
		/// Constructor.
		/// </summary>
		public GcITachFlexDevice()
		{
			m_Client = new AsyncTcpClient
			{
				Port = TCP_PORT,
				DebugRx = true,
				DebugTx = true
			};

			m_Buffer = new DelimiterSerialBuffer(FlexData.NEWLINE);
			
			Subscribe(m_Buffer);
			Subscribe(m_Client);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void DisposeFinal(bool disposing)
		{
			base.DisposeFinal(disposing);

			Unsubscribe(m_Buffer);
			Unsubscribe(m_Client);

			m_Client.Dispose();
		}

		protected override bool GetIsOnlineStatus()
		{
			return m_Client != null && m_Client.IsOnline;
		}

		#region Client Callbacks

		/// <summary>
		/// Subscribe to the client callbacks.
		/// </summary>
		/// <param name="client"></param>
		private void Subscribe(AsyncTcpClient client)
		{
			client.OnIsOnlineStateChanged += ClientOnOnIsOnlineStateChanged;
			client.OnSerialDataReceived += ClientOnOnSerialDataReceived;
		}

		/// <summary>
		/// Unsubscribe from the client callbacks.
		/// </summary>
		/// <param name="client"></param>
		private void Unsubscribe(AsyncTcpClient client)
		{
			client.OnIsOnlineStateChanged -= ClientOnOnIsOnlineStateChanged;
			client.OnSerialDataReceived -= ClientOnOnSerialDataReceived;
		}

		/// <summary>
		/// Called when we receive data from the device.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="stringEventArgs"></param>
		private void ClientOnOnSerialDataReceived(object sender, StringEventArgs stringEventArgs)
		{
			m_Buffer.Enqueue(stringEventArgs.Data);
		}

		/// <summary>
		/// Called when the 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="boolEventArgs"></param>
		private void ClientOnOnIsOnlineStateChanged(object sender, BoolEventArgs boolEventArgs)
		{
			UpdateCachedOnlineStatus();
		}

		#endregion

		#region Buffer Callbacks

		/// <summary>
		/// Subsribe to the buffer events.
		/// </summary>
		/// <param name="buffer"></param>
		private void Subscribe(DelimiterSerialBuffer buffer)
		{
			buffer.OnCompletedSerial += BufferOnOnCompletedSerial;
		}

		/// <summary>
		/// Unsubscribe from the buffer events.
		/// </summary>
		/// <param name="buffer"></param>
		private void Unsubscribe(DelimiterSerialBuffer buffer)
		{
			buffer.OnCompletedSerial -= BufferOnOnCompletedSerial;
		}

		/// <summary>
		/// Called when we receive a complete message from the device.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="stringEventArgs"></param>
		private void BufferOnOnCompletedSerial(object sender, StringEventArgs stringEventArgs)
		{
			IcdConsole.PrintLine(stringEventArgs.Data);
		}

		#endregion

		#region Settings

		protected override void ClearSettingsFinal()
		{
			base.ClearSettingsFinal();

			m_Client.Address = null;
		}

		protected override void ApplySettingsFinal(GcITachFlexDeviceSettings settings, IDeviceFactory factory)
		{
			base.ApplySettingsFinal(settings, factory);

			m_Client.Address = settings.Address;

			m_Client.Connect();

			m_Client.Send("getdevices\r");
		}

		protected override void CopySettingsFinal(GcITachFlexDeviceSettings settings)
		{
			base.CopySettingsFinal(settings);

			settings.Address = m_Client.Address;
		}

		#endregion

		#region Console

		public override IEnumerable<IConsoleNodeBase> GetConsoleNodes()
		{
			foreach (IConsoleNodeBase node in GetBaseConsoleNodes())
				yield return node;

			yield return m_Client;
		}

		/// <summary>
		/// Workaround for "unverifiable code" warning.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IConsoleNodeBase> GetBaseConsoleNodes()
		{
			return base.GetConsoleNodes();
		}

		#endregion
	}
}
