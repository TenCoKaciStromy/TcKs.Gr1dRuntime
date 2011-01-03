using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gr1d.Api.Agent;
using Gr1d.Api.Node;
using Gr1d.Api.Player;
using Gr1d.Api.Sector;

namespace TcKs.Gr1dRuntime.Api {
	public class NodeInformation : NodeLocation, INodeInformation {
		public NodeInformation() {
			this.Init();
		}
		public NodeInformation(int layer, int row, int column) : base(layer, row, column) {
			this.Init();
		}
		public NodeInformation(int layer, int row, int column, IPlayer owner) : base(layer, row, column) {
			this.Init();
			this.Owner = owner;
		}
		public NodeInformation(int layer, int row, int column, ISectorInfo sector) : base(layer, row, column) {
			this.Init();
			this.Sector = sector;
		}
		public NodeInformation(int layer, int row, int column, ISectorInfo sector, IPlayer owner) : base(layer, row, column) {
			this.Init();
			this.Sector = sector;
			this.Owner = owner;
		}
		private void Init() {
			this.AllAgentsList = new List<IAgentInfo>();
			this.EffectsList = new List<NodeEffect>();
			this.Exits = new Dictionary<NodeExit, INodeInformation>();
		}

		public IPlayer CurrentPlayer { get; set; }

		public List<IAgentInfo> AllAgentsList { get; private set; }
		public IEnumerable<IAgentInfo> AllAgents {
			get { return this.AllAgentsList; }
		}

		public IEnumerable<IAgentInfo> AlliedAgents {
			get {
				IEnumerable<IAgentInfo> ret;
				var currentPlayer = this.CurrentPlayer;
				if (null == currentPlayer) {
					ret = new IAgentInfo[0];
				}
				else {
					ret = (from a in this.AllAgents
						   let aOwner = a.Owner
						   where null != aOwner
						   where aOwner.Id == currentPlayer.Id
						   select a);
				}

				return ret;
			}
		}

		public List<NodeEffect> EffectsList { get; private set; }
		public IEnumerable<NodeEffect> Effects {
			get { return this.EffectsList; }
		}

		public IDictionary<NodeExit, INodeInformation> Exits { get; private set; }

		public bool IsClaimable { get; set; }

		public IEnumerable<IAgentInfo> MyAgents {
			get {
				IEnumerable<IAgentInfo> ret;
				var currentPlayer = this.CurrentPlayer;
				if (null == currentPlayer) {
					ret = new IAgentInfo[0];
				}
				else {
					ret = (from a in this.AllAgents
						   let aOwner = a.Owner
						   where null != aOwner
						   where aOwner.Id == currentPlayer.Id
						   select a);
				}

				return ret;
			}
		}

		public IEnumerable<IAgentInfo> OpposingAgents {
			get {
				IEnumerable<IAgentInfo> ret;
				var currentPlayer = this.CurrentPlayer;
				if (null == currentPlayer) {
					ret = new IAgentInfo[0];
				}
				else {
					ret = (from a in this.AllAgents
						   let aOwner = a.Owner
						   where null != aOwner
						   where aOwner.Id != currentPlayer.Id
						   select a);
				}

				return ret;
			}
		}

		public IEnumerable<IAgentInfo> OtherAgents {
			get {
				IEnumerable<IAgentInfo> ret;
				var currentPlayer = this.CurrentPlayer;
				if (null == currentPlayer) {
					ret = new IAgentInfo[0];
				}
				else {
					ret = (from a in this.AllAgents
						   let aOwner = a.Owner
						   where null != aOwner
						   where aOwner.Id != currentPlayer.Id
						   select a);
				}

				return ret;
			}
		}

		public IPlayer Owner { get; set; }

		public ISectorInfo Sector { get; set; }
	}
}
