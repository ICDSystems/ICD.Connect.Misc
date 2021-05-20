using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ICD.Common.Utils;
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

		private static Type[] s_ResponseTypes;

		private static Type[] ResponseTypes
		{
			get
			{
				return s_ResponseTypes = s_ResponseTypes ?? typeof(VibeResponseHandler).GetAssembly().GetTypes()
					                         .Where(t =>((Type)t).IsAssignableTo<IVibeResponse>())
#if SIMPLSHARP
											 .Select(t => (Type)t)
#endif
											 .ToArray();
			}
		}

		private sealed class CallbackPair
		{
			public VibeResponseCallback WrappedCallback { get; set; }
			public object ActualCallback { get; set; }
		}

		private readonly Dictionary<Type, List<CallbackPair>> m_TypeCallbacks;
		private readonly Dictionary<string, List<CallbackPair>> m_ResultIdCallbacks;
		private readonly SafeCriticalSection m_CallbackSection;

		public VibeResponseHandler()
		{
			m_TypeCallbacks = new Dictionary<Type, List<CallbackPair>>();
			m_ResultIdCallbacks = new Dictionary<string, List<CallbackPair>>();
			m_CallbackSection = new SafeCriticalSection();
		}

		public void RegisterResponseCallback<TResponse>(VibeResponseCallback<TResponse> callback) where TResponse: IVibeResponse
		{
			var pair = new CallbackPair
			{
				ActualCallback = callback, 
				WrappedCallback = WrapCallback(callback)
			};

			m_CallbackSection.Enter();
			try
			{
				var callbackList = m_TypeCallbacks.GetOrAddDefault(typeof(TResponse), new List<CallbackPair>());
				if (!callbackList.Any(c => ReferenceEquals(c.ActualCallback, callback)))
					callbackList.Add(pair);
			}
			finally
			{
				m_CallbackSection.Leave();
			}
		}

		public void UnregisterResponseCallback<TResponse>(VibeResponseCallback<TResponse> callback) where TResponse: IVibeResponse
		{
			m_CallbackSection.Enter();
			try
			{
				if (!m_TypeCallbacks.ContainsKey(typeof(TResponse)))
					return;

				var callbackList = m_TypeCallbacks[typeof(TResponse)];
				if (callbackList == null || !callbackList.Any())
					return;

				var callbackPair = callbackList.FirstOrDefault(c => c.ActualCallback.Equals(callback));
				if (callbackPair != null)
					callbackList.Remove(callbackPair);
			}
			finally
			{
				m_CallbackSection.Leave();
			}
		}
		
		public void RegisterResponseCallback<TResponse>(string resultId, VibeResponseCallback<TResponse> callback) where TResponse: IVibeResponse
		{
			var pair = new CallbackPair
			{
				ActualCallback = callback, 
				WrappedCallback = WrapCallback(callback)
			};

			m_CallbackSection.Enter();
			try
			{
				var callbackList = m_ResultIdCallbacks.GetOrAddDefault(resultId, new List<CallbackPair>());
				if (!callbackList.Any(c => ReferenceEquals(c.ActualCallback, callback)))
					callbackList.Add(pair);
			}
			finally
			{
				m_CallbackSection.Leave();
			}
		}

		public void HandleResponse(string data)
		{
			Match match = Regex.Match(data, REGEX_RESPONSE, RegexOptions.Multiline | RegexOptions.Singleline);
			if (!match.Success || !match.Groups["type"].Success)
				return;

			string typeString = match.Groups["type"].Value;
			Type type = ResponseTypes.SingleOrDefault(t => t.Name.Equals(typeString));
			IVibeResponse response = JsonConvert.DeserializeObject(data, type) as IVibeResponse;

			if (response == null)
				return;

			m_CallbackSection.Enter();
			try
			{
				if (!string.IsNullOrEmpty(response.ResultId) && m_ResultIdCallbacks.ContainsKey(response.ResultId))
					foreach (var callback in m_ResultIdCallbacks[response.ResultId])
						callback.WrappedCallback(response);
				else if (m_TypeCallbacks.ContainsKey(response.GetType()))
					foreach (var callback in m_TypeCallbacks[response.GetType()])
						callback.WrappedCallback(response);
			}
			finally
			{
				m_CallbackSection.Leave();
			}
		}

		private static VibeResponseCallback WrapCallback<T>(VibeResponseCallback<T> callback) where T : IVibeResponse
		{
			// separate static method to prevent lambda capture context wackiness
			return resp => callback((T)resp);
		}
	}
}
