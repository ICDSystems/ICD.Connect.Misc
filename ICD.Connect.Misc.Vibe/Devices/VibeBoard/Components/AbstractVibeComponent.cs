using ICD.Common.Utils.EventArguments;
using ICD.Common.Utils.Extensions;
using ICD.Connect.API.Commands;
using ICD.Connect.API.Nodes;
using System.Collections.Generic;

namespace ICD.Connect.Misc.Vibe.Devices.VibeBoard.Components
{
	/// <summary>
	/// AbstractVibeComponent is a base class for Vibe modules.
	/// </summary>
	public abstract class AbstractVibeComponent : IVibeComponent
	{
		#region Properties

		/// <summary>
		/// Gets the Codec.
		/// </summary>
		public VibeBoard Parent { get; private set; }

		/// <summary>
		/// Gets the name of the node in the console.
		/// </summary>
		public virtual string ConsoleName { get { return GetType().GetNameWithoutGenericArity(); } }

		/// <summary>
		/// Gets the help information for the node.
		/// </summary>
		public virtual string ConsoleHelp { get { return string.Empty; } }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent"></param>
		protected AbstractVibeComponent(VibeBoard parent)
		{
			Parent = parent;
		}

		/// <summary>
		/// Deconstructor.
		/// </summary>
		~AbstractVibeComponent()
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
		protected virtual void Dispose(bool disposing)
		{
			Unsubscribe(Parent);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Calls the delegate for each console status item.
		/// </summary>
		/// <param name="addRow"></param>
		public virtual void BuildConsoleStatus(AddStatusRowDelegate addRow)
		{
		}

		/// <summary>
		/// Gets the child console node groups.
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerable<IConsoleNodeBase> GetConsoleNodes()
		{
			yield break;
		}

		/// <summary>
		/// Gets the child console commands.
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerable<IConsoleCommand> GetConsoleCommands()
		{
			yield break;
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Subscribes to the vibe events.
		/// </summary>
		/// <param name="vibe"></param>
		protected virtual void Subscribe(VibeBoard vibe)
		{
			if (vibe == null)
				return;

			vibe.OnInitializedChanged += CodecOnInitializedChanged;
			vibe.OnConnectedStateChanged += CodecOnConnectionStateChanged;
		}

		/// <summary>
		/// Unsubscribes from the vibe events.
		/// </summary>
		/// <param name="vibe"></param>
		protected virtual void Unsubscribe(VibeBoard vibe)
		{
			if (vibe == null)
				return;

			vibe.OnInitializedChanged -= CodecOnInitializedChanged;
			vibe.OnConnectedStateChanged -= CodecOnConnectionStateChanged;
		}

		/// <summary>
		/// Called to initialize the component.
		/// </summary>
		protected virtual void Initialize()
		{
		}

		/// <summary>
		/// Called when the component connects/disconnects to the vibe.
		/// </summary>
		protected virtual void ConnectionStatusChanged(bool state)
		{
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Called when the vibe initializes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void CodecOnInitializedChanged(object sender, BoolEventArgs args)
		{
			if (args.Data)
				Initialize();
		}

		/// <summary>
		/// Called when the vibe connects/disconnects
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void CodecOnConnectionStateChanged(object sender, BoolEventArgs args)
		{
			ConnectionStatusChanged(args.Data);
		}

		#endregion
	}
}
