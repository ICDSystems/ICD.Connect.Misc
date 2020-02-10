using System;
using System.Collections.Generic;
using System.Linq;
using ICD.Common.Utils;
using ICD.Connect.API.Nodes;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components
{
	/// <summary>
	/// VibeComponentFactory provides a facility for lazy-loading components.
	/// </summary>
	public sealed class VibeComponentFactory : IDisposable, IConsoleNodeGroup
	{
		private static readonly Dictionary<Type, Func<VibeBoard, IVibeComponent>> s_Factories =
			new Dictionary<Type, Func<VibeBoard, IVibeComponent>>
			{
				{typeof(ScreenComponent), vibe => new ScreenComponent(vibe)},
				{typeof(KeyComponent), vibe => new KeyComponent(vibe)},
				{typeof(DumpComponent), vibe => new DumpComponent(vibe)},
				{typeof(VolumeComponent), vibe => new VolumeComponent(vibe)},
				{typeof(TaskComponent), vibe => new TaskComponent(vibe)},
				{typeof(PackageComponent), vibe => new PackageComponent(vibe)},
				{typeof(StartComponent), vibe => new StartComponent(vibe)},
				{typeof(MuteComponent), vibe => new MuteComponent(vibe)},
				{typeof(OTAComponent), vibe => new OTAComponent(vibe)},
				{typeof(SessionComponent), vibe => new SessionComponent(vibe)}
			};

		private readonly Dictionary<Type, IVibeComponent> m_Components;
		private readonly SafeCriticalSection m_ComponentsSection;

		private readonly VibeBoard m_Vibe;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="vibe"></param>
		public VibeComponentFactory(VibeBoard vibe)
		{
			m_Components = new Dictionary<Type, IVibeComponent>();
			m_ComponentsSection = new SafeCriticalSection();

			m_Vibe = vibe;

			// Load components
			foreach (Type type in s_Factories.Keys)
				GetComponent(type);
		}

		/// <summary>
		/// Deconstructor.
		/// </summary>
		~VibeComponentFactory()
		{
			Dispose(false);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		/// <param name="disposing"></param>
		private void Dispose(bool disposing)
		{
			m_ComponentsSection.Enter();

			try
			{
				foreach (IVibeComponent component in m_Components.Values)
					component.Dispose();
				m_Components.Clear();
			}
			finally
			{
				m_ComponentsSection.Leave();
			}
		}

		#region Methods

		/// <summary>
		/// Gets the component with the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetComponent<T>()
			where T : IVibeComponent
		{
			return (T)GetComponent(typeof(T));
		}

		/// <summary>
		/// Gets the component with the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IVibeComponent GetComponent(Type type)
		{
			m_ComponentsSection.Enter();

			try
			{
				IVibeComponent component;
				if (!m_Components.TryGetValue(type, out component))
				{
					component = s_Factories[type](m_Vibe);
					m_Components.Add(type, component);
				}

				return component;
			}
			finally
			{
				m_ComponentsSection.Leave();
			}
		}

		/// <summary>
		/// Returns the cached components.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IVibeComponent> GetComponents()
		{
			return m_ComponentsSection.Execute(() => m_Components.Values.OrderBy(c => c.GetType().Name).ToArray());
		}

		#endregion

		#region Console 

		public string ConsoleName
		{
			get { return "Components"; }
		}

		public string ConsoleHelp
		{
			get { return "Vibe Board components"; }
		}

		public IDictionary<uint, IConsoleNodeBase> GetConsoleNodes()
		{
			return m_ComponentsSection.Execute(() =>
				ConsoleNodeGroup.IndexNodeMap("Components", "Vibe Board Components", m_Components.Values).GetConsoleNodes()
			);
		}

		IEnumerable<IConsoleNodeBase> IConsoleNodeBase.GetConsoleNodes()
		{
			return GetConsoleNodes().Select(kvp => kvp.Value);
		}

		#endregion
	}
}
