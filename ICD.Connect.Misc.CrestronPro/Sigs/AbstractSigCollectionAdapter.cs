﻿#if !NETSTANDARD
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharpPro;
using ICD.Common.Utils;
using ICD.Connect.Panels.SigCollections;
using ICD.Connect.Protocol.Sigs;
using Sig = Crestron.SimplSharpPro.Sig;

namespace ICD.Connect.Misc.CrestronPro.Sigs
{
	public abstract class AbstractSigCollectionAdapter<TAdapter, T> : ISigCollectionBase<TAdapter>
		where TAdapter : ISig
		where T : Sig
	{
		private readonly Func<T, TAdapter> m_Factory;
		private readonly Dictionary<uint, TAdapter> m_SigAdapterNumberCache;
		private readonly SafeCriticalSection m_CacheSection;

		private SigCollectionBase<T> m_Collection;

		/// <summary>
		/// Get the sig with the specified number.
		/// </summary>
		/// <param name="sigNumber">Number of the sig to return.</param>
		/// <returns/>
		/// <exception cref="T:System.IndexOutOfRangeException">Invalid Sig Number specified.</exception>
		public TAdapter this[uint sigNumber]
		{
			get
			{
				m_CacheSection.Enter();

				try
				{
					if (m_Collection == null)
						throw new InvalidOperationException("No collection assigned");

					TAdapter adapter;
					if (!m_SigAdapterNumberCache.TryGetValue(sigNumber, out adapter))
					{
						T sig = m_Collection[sigNumber];
						adapter = m_Factory(sig);
						m_SigAdapterNumberCache[sigNumber] = adapter;
					}

					return adapter;
				}
				finally
				{
					m_CacheSection.Leave();
				}
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		protected AbstractSigCollectionAdapter(Func<T, TAdapter> factory, SigCollectionBase<T> collection)
		{
			if (factory == null)
				throw new ArgumentNullException("factory");

			m_Factory = factory;

			m_SigAdapterNumberCache = new Dictionary<uint, TAdapter>();
			m_CacheSection = new SafeCriticalSection();

			SetCollection(collection);
		}

		/// <summary>
		/// Sets the wrapped collection.
		/// </summary>
		/// <param name="collection"></param>
		public void SetCollection(SigCollectionBase<T> collection)
		{
			m_CacheSection.Enter();

			try
			{
				if (collection == m_Collection)
					return;

				Clear();

				m_Collection = collection;
			}
			finally
			{
				m_CacheSection.Leave();
			}
		}

		/// <summary>
		/// Clears the cache.
		/// </summary>
		private void Clear()
		{
			m_CacheSection.Execute(() => m_SigAdapterNumberCache.Clear());
		}

		/// <summary>
		/// Doesn't really do a whole lot, since sigs are only instantiated by SigCollectionBase on request.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<TAdapter> GetEnumerator()
		{
			m_CacheSection.Enter();

			try
			{
				return m_Collection.Select(i => i.Number)
				                   .Select(n => this[n])
				                   .ToList()
				                   .GetEnumerator();
			}
			finally
			{
				m_CacheSection.Leave();
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}

#endif
