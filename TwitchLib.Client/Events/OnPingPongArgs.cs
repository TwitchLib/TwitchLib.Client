using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchLib.Client.Events
{
	/// <summary>
	/// Class OnPingPongArgs.
	/// Implements the <see cref="System.EventArgs" />
	/// </summary>
	/// <seealso cref="System.EventArgs" />
	public class OnPingPongArgs : EventArgs
	{
		/// <summary>
		/// The raw IRC message
		/// </summary>
		public string RawMessage;
	}
}
