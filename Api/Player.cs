using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gr1d.Api.Player;
using Gr1d.Api.Sector;

namespace TcKs.Gr1dRuntime.Api {
	public class Player : IPlayer {
		public string DisplayHandle { get; set; }

		public Guid Id { get; set; }

		public ISectorInfo Sector { get; set; }

		/// <summary>
		/// This property is not member of IPlayer interface.
		/// </summary>
		public int Level { get; set; }
	}
}
