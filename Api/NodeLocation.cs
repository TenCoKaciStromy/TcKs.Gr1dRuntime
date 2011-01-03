using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gr1d.Api.Agent;
using Gr1d.Api.Node;
using Gr1d.Api.Player;

namespace TcKs.Gr1dRuntime.Api {
	public class NodeLocation : INodeLocation {
		public NodeLocation() { }
		public NodeLocation(int layer, int row, int column) {
			this.Layer = layer;
			this.Row = row;
			this.Column = column;
		}

		public int Column { get; set; }

		public int Distance(INodeLocation destination) {
			throw new NotImplementedException();
		}

		public INodeLocation Exit(NodeExit exit) {
			throw new NotImplementedException();
		}

		public int Layer { get; set; }

		public NodeExit RouteTo(INodeInformation origin, int destinationLayer, int destinationRow, int destinationColumn) {
			throw new NotImplementedException();
		}

		public int Row { get; set; }
	}
}
