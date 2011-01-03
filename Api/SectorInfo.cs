using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gr1d.Api.Sector;
using Gr1d.Api.Zone;

namespace TcKs.Gr1dRuntime.Api {
	public class SectorInfo : ISectorInfo {
		public Guid Id { get; set; }

		public string Name { get; set; }

		public IZoneInfo Zone { get; set; }
	}
}
