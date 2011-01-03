using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gr1d.Api.Region;
using Gr1d.Api.Zone;

namespace TcKs.Gr1dRuntime.Api {
	public class ZoneInfo : IZoneInfo {
		public string Name { get; set; }

		public IRegionInfo Region { get; set; }
	}
}
