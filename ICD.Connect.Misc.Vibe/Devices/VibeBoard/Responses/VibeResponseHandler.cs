using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ICD.Common.Utils.Extensions;
using Newtonsoft.Json;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Responses
{
	public delegate void VibeResponseCallback(IVibeResponse response);

	public delegate void VibeResponseCallback<TResponse>(TResponse response) where TResponse : IVibeResponse;

	public sealed class VibeResponseHandler
	{
		private const string REGEX_RESPONSE =
			"\"type\"\\s*:\\s*\"(?'type'[^\"\\\\]*(?:\\\\.[^\"\\\\]*)*)\"(?:\\s*,.*\"resultId\"\\s*:\\s*\"(?'resultId'[^\"\\\\]*(?:\\\\.[^\"\\\\]*)*)\")?";

		private class CallbackPair
		{
			public VibeResponseCallback WrappedCallback { get; set; }
			public object ActualCallback { get; set; }
		}

		private readonly Dictionary<Type, List<CallbackPair>> m_TypeCallbacks;
		private readonly Dictionary<string, List<CallbackPair>> m_ResultIdCallbacks;

		public VibeResponseHandler()
		{
			m_TypeCallbacks = new Dictionary<Type, List<CallbackPair>>();
			m_ResultIdCallbacks = new Dictionary<string, List<CallbackPair>>();
		}

		public void RegisterResponseCallback<TResponse>(VibeResponseCallback<TResponse> callback) where TResponse: IVibeResponse
		{
			var pair = new CallbackPair
			{
				ActualCallback = callback, 
				WrappedCallback = WrapCallback(callback)
			};

			var callbackList = m_TypeCallbacks.GetOrAddDefault(typeof(TResponse), new List<CallbackPair>());
			if (!callbackList.Any(c => ReferenceEquals(c.ActualCallback, callback)))
				callbackList.Add(pair);
		}
		
		public void RegisterResponseCallback<TResponse>(string resultId, VibeResponseCallback<TResponse> callback) where TResponse: IVibeResponse
		{
			var pair = new CallbackPair
			{
				ActualCallback = callback, 
				WrappedCallback = WrapCallback(callback)
			};

			var callbackList = m_ResultIdCallbacks.GetOrAddDefault(resultId, new List<CallbackPair>());
			if (!callbackList.Any(c => ReferenceEquals(c.ActualCallback,callback)))
				callbackList.Add(pair);
		}

		public void HandleResponse(string data)
		{
			Match match = Regex.Match(data, REGEX_RESPONSE, RegexOptions.Multiline | RegexOptions.Singleline);
			if (!match.Success || !match.Groups["type"].Success)
				return;

			string typeString = match.Groups["type"].Value;
			Type type = Type.GetType(typeString);
			IVibeResponse response = (IVibeResponse)JsonConvert.DeserializeObject(data, type);
		}

		private static VibeResponseCallback WrapCallback<T>(VibeResponseCallback<T> callback) where T : IVibeResponse
		{
			// separate static method to prevent lambda capture context wackiness
			return (resp) => callback((T)resp);
		}
	}
}
