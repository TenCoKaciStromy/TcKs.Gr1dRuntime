using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gr1d.Api;
using Gr1d.Api.Node;
using Gr1d.Api.Player;

using TcKs.Gr1dRuntime.Api;
using TcKs.Gr1dRuntime.Mechanics;
using TcKs.Gr1dRuntime.Console;

using TcKs.Runtime.CmdTest.Agents.Pirates;

namespace TcKs.Runtime.CmdTest.Scenarios.SimpleTest {
	public class SimpleTestWorld : World {
		public IPlayer EmptyPlayer { get; set; }
		public static SimpleTestWorld Create() {
			var emptyPlayer = new Player {
				DisplayHandle = Guid.Empty.ToString(),
				Id = Guid.Empty,
				Sector = null
			};
			var ret = new SimpleTestWorld();
			ret.EmptyPlayer = emptyPlayer;
			ret.Nodes = CreateSector("SimpleSector_Size_20", 20, emptyPlayer);

			return ret;
		}
		public static SimpleTestWorld CreateInitialized() {
			var ret = Create();
			Initialize(ret);
			return ret;
		}
		public static void Initialize(World world) {
			var playerHugo = new Player {
				Level = 1,
				DisplayHandle = "Hugo",
				Id = Guid.NewGuid(),
				Sector = new SectorInfo {
					Id = Guid.NewGuid(),
					Name = "Sector of Hugo",
					Zone = new ZoneInfo {
						Name = "Zone of Hugo",
						Region = new RegionInfo {
							Name = "Region of Hugo"
						}
					}
				}
			};

			var playerMorg = new Player {
				Level = 1,
				DisplayHandle = "Morg",
				Id = Guid.NewGuid(),
				Sector = new SectorInfo {
					Id = Guid.NewGuid(),
					Name = "Sector of Morg",
					Zone = new ZoneInfo {
						Name = "Zone of Morg",
						Region = new RegionInfo {
							Name = "Region of Morg"
						}
					}
				}
			};

			#region Hugo
			var deck_Hugo = (ConsoleDeck)world.AddPlayer(playerHugo);
			deck_Hugo.TracePrefix = "Hugo>";
			deck_Hugo.TraceForeColor = ConsoleColor.Cyan;
			#region Scout
			var hugo_Scout = new Pirate01Scout();
			world.AddAgent(hugo_Scout, 1, playerHugo);
			world.LaunchAgent(hugo_Scout);
			#endregion Scout

			#region Defender
			var hugo_Defender = new Pirate01Defender();
			world.AddAgent(hugo_Defender, 1, playerHugo);
			world.LaunchAgent(hugo_Defender);
			#endregion Defender
			#endregion Hugo

			var deck_Morg = (ConsoleDeck)world.AddPlayer(playerMorg);
			deck_Morg.TracePrefix = "Morg>";
			deck_Morg.TraceForeColor = ConsoleColor.Magenta;
			#region Scout
			var morg_Scout = new Pirate01Scout();
			world.AddAgent(morg_Scout, 1, playerMorg);
			world.LaunchAgent(morg_Scout);
			#endregion Scout

			#region Dummy
			var morg_Dummy = new Pirate01Dummy();
			world.AddAgent(morg_Dummy, 1, playerMorg);
			world.LaunchAgent(morg_Dummy);
			#endregion Dummy
		}

		private static LinkedList<INodeInformation> CreateSector(string name, int sectorSideSize, IPlayer emptyPlayer) {
			LinkedList<INodeInformation> ret = new LinkedList<INodeInformation>();
			var sector = new SectorInfo();

			/* building the nodes >>> */
			int layersInSector = sectorSideSize;
			for (int layer = 0; layer < sectorSideSize; layer++) {
				int rowsInLayer = layersInSector - layer;

				int indexInLayer = 0;
				for (int row = 0; row < rowsInLayer; row++) {
					int columnsInRow = rowsInLayer - row;
					for (int column = 0; column < columnsInRow; column++) {
						var node = new SampleMockNodeInformation(layer, row, column, sector, emptyPlayer);
						node.IndexInLayer = indexInLayer;
						ret.AddLast(node);

						indexInLayer++;
					}
				}
			}
			/* <<< building the nodes */

			/* joining the nodes >>> */
			var exits = new NodeExit[] {
				NodeExit.Alpha, NodeExit.Beta, NodeExit.Delta, NodeExit.Epsilon, NodeExit.Eta, NodeExit.Gamma,
				NodeExit.Iota, NodeExit.Kappa, NodeExit.Lambda, NodeExit.Mu, NodeExit.Theta, NodeExit.Zeta
			};
			foreach (SampleMockNodeInformation node in ret) {
				int startLayer = node.Layer;
				int startRow = node.Row;
				int startColumn = node.Column;
				int startIndex = node.IndexInLayer;

				var currRow0 = startRow - 1;
				var currColumns0 = new int[]{ startColumn, startColumn + 1 };
				var currRow1 = startRow;
				var currColumns1 = new int[] { startColumn - 1, startColumn + 1 };
				var currRow2 = startRow + 1;
				var currColumns2 = new int[] { startColumn - 1, startColumn };
				var currNodes = (from n in ret
								 where n.Layer == startLayer
								 where (n.Row == currRow0 && currColumns0.Contains(n.Column))
									|| (n.Row == currRow1 && currColumns1.Contains(n.Column))
									|| (n.Row == currRow2 && currColumns2.Contains(n.Column))
								 select n).ToList();

				int nextLayer = startLayer + 1;
				var nextRow0 = startRow;
				var nextColumns0 = new int[] { startColumn };
				var nextRow1 = startRow - 1;
				var nextColumns1 = new int[] { startColumn - 1, startColumn };

				var nextNodes = (from n in ret
								 where n.Layer == nextLayer
								 where (n.Row == nextRow0 && nextColumns0.Contains(n.Column))
									|| (n.Row == nextRow1 && nextColumns1.Contains(n.Column))
								 select n).ToList();

				var nestedNodes = currNodes.Concat(nextNodes).ToList();
				//var connectedNodes = currNodes.Concat(nextNodes).ToList();
				var connectedNodes = (from n in nestedNodes
									  let currN = n.Exits.Where(kp => kp.Value == node).Select(kp => kp.Value).FirstOrDefault()
									  where null == currN
									  select n).ToList();

				var currAvailableExits = new List<NodeExit>(exits.Except(node.Exits.Keys));
				for (int i = 0; i < connectedNodes.Count; i++) {
					var nextNode = connectedNodes[i];
					var exit = currAvailableExits[i];
					node.Exits.Add(exit, nextNode);

					var connAvailableExits = new List<NodeExit>(exits.Except(nextNode.Exits.Keys));
					var nextExit = connAvailableExits.First();
					nextNode.Exits.Add(nextExit, node);
				}

			}
			/* <<< joining the nodes */

			/*
			 * 0 1 2 3
			 *				0 1 2
			 *  4 5 6				0 1
			 *				 3 4			0
			 *   7 8				 2
			 *				  5
			 *    9
			 *    
			 * 
			 * L0:0 -> L0:{1,4}			+ L1:0						= 3
			 * L0:1 -> L0:{0,2,4,5}		+ L1:{0,1}					= 6
			 * L0:2 -> L0:{1,3,5,6}		+ L1:{1,2}					= 6
			 * L0:3 -> L0:{2,6}			+ L1:2						= 3
			 * L0:4 -> L0:{0,1,5}		+ L1:{0,3}					= 5
			 * L0:5 -> L0:{1,2,4,6,7,8}	+ L1:{1,3,4}				= 9
			 * L0:6 -> L0:{2,3,5,8}		+ L1:{2,4}					= 6
			 * L0:7 -> L0:{4,5,8,9}		+ L1:{3,5}					= 6
			 * L0:8 -> L0:{5,6,7,9}		+ L1:{4,5}					= 6
			 * L0:9 -> L0:{7,8}			+ L1:5						= 3
			 * 
			 * L1:0 -> L1:{1,3}			+ L0:{0,1,4}	+ L2:0		= 6
			 * L1:1 -> L1:{0,2,3,4}		+ L0:{1,2,5}	+ L2:{0,1}	= 9
			 * L1:2 -> L1:{1,4}			+ L0:{2,3,4}	+ L2:1		= 6
			 * L1:3 -> L1:{0,1,4,5}		+ L0:{4,5,7}	+ L2:{0,2}	= 9
			 * L1:4 -> L1:{1,2,3,5}		+ L0:{5,6,8}	+ L2:{1,2}	= 9
			 * L1:5 -> L1:{3,4}			+ L0:{7,8,9}	+ L2:2		= 6
			 * 
			 * L2:0 -> L2:{1,2}			+ L1:{0,1,3}	+ L3:0		= 6
			 * L2:1 -> L2:{0,2}			+ L1:{1,2,4}	+ L3:0		= 6
			 * L2:2 -> L2:{0,1}			+ L1:{3,4,5}	+ L3:0		= 6
			 * 
			 * L3:0 -> L3:<->			+ L2:{0,1,2}				= 3
			 * 
			 * 
			 * X X X X	XXXX
			 *  X X X	XXX
			 *   X X	XX
			 *    X		X
			 *    
			 * 0 1 2 3	0123
			 *  4 5 6	456
			 *   7 8	78
			 *    9		9
			 *    
			 *    
			 *  R 0123		012		12		1
			 * C  
			 * 0  0123		012		12		1
			 * 1  456		34		3
			 * 2  78		5
			 * 3  9
			*/


			return ret;
		}

		public class SampleMockNodeInformation : NodeInformation {
			public int IndexInLayer { get; set; }

			public SampleMockNodeInformation() : base() { }
			public SampleMockNodeInformation(int layer, int row, int column) : base(layer, row, column) { }
			public SampleMockNodeInformation(int layer, int row, int column, IPlayer owner) : base(layer, row, column, owner) { }
			public SampleMockNodeInformation(int layer, int row, int column, Gr1d.Api.Sector.ISectorInfo sector) : base(layer, row, column, sector) { }
			public SampleMockNodeInformation(int layer, int row, int column, Gr1d.Api.Sector.ISectorInfo sector, IPlayer owner) : base(layer, row, column, sector, owner) { }

			public override string ToString() {
				//return base.ToString();
				var ret = string.Format("L{0}:{1}", this.Layer, this.IndexInLayer);
				return ret;
			}
		}
	}
}
