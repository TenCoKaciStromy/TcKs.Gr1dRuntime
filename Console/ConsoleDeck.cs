using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gr1d.Api;
using Gr1d.Api.Agent;
using Gr1d.Api.Deck;
using Gr1d.Api.Node;
using Gr1d.Api.Player;

using TcKs.Gr1dRuntime.Api;

namespace TcKs.Gr1dRuntime.Console {
	/// <summary>
	/// Implementation of IDeck.
	/// Trace to Console.
	/// </summary>
	public class ConsoleDeck : DeckBase {
		private static readonly object consoleSyncRoot = new object();

		public string TracePrefix { get; set; }
		public ConsoleColor? TraceForeColor { get; set; }

		public override void Trace(string message, string category) {
			lock (consoleSyncRoot) {
				System.Console.ResetColor();
				var color = this.TraceForeColor;
				if (color.HasValue) {
					System.Console.ForegroundColor = color.Value;
				}

				System.Console.WriteLine(string.Format("{0}{1}: {2}", this.TracePrefix, category, message));
				System.Console.WriteLine();

				System.Console.ResetColor();
			}
		}
	}
}
