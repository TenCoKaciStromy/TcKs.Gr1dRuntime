using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gr1d.Api.Agent;
using Gr1d.Api.Node;
using Gr1d.Api.Player;

namespace TcKs.Gr1dRuntime.Api {
	public class AgentInfo : IAgentInfo {
		public AgentInfo() {
			this.EffectsList = new List<AgentEffect>();
		}
		public AgentInfo(IAgent agent) : this() {
			this.Type = agent.GetType();
		}

		public AgentAction Action { get; set; }

		public string Clan { get; set; }

		public List<AgentEffect> EffectsList { get; private set; }
		public IEnumerable<AgentEffect> Effects {
			get { return this.EffectsList; }
		}

		public string Group { get; set; }

		public int Heap { get; set; }

		public Guid Id { get; set; }

		public int Level { get; set; }

		public INodeInformation Node { get; set; }

		public IPlayer Owner { get; set; }

		public string Pack { get; set; }

		public string Raid { get; set; }

		public int Stack { get; set; }

		public Type Type { get; set; }
	}
}
