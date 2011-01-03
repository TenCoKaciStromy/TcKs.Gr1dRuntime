using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gr1d.Api.Agent;
using Gr1d.Api.Node;

namespace TcKs.Gr1dRuntime.Api {
	public class AgentUpdateInfo : AgentInfo, IAgentUpdateInfo {
		public AgentUpdateInfo() : base() {
			this.EffectsPreviousList = new List<AgentEffect>();
		}
		public AgentUpdateInfo(IAgent agent) : base(agent) {
			this.Type = agent.GetType();this.EffectsPreviousList = new List<AgentEffect>();
		}

		public List<AgentEffect> EffectsPreviousList { get; private set; }
		public IEnumerable<AgentEffect> EffectsPrevious {
			get { return this.EffectsPreviousList; }
		}

		public int HeapPrevious { get; set; }

		public INodeInformation NodePrevious { get; set; }

		public int StackPrevious { get; set; }

		public int UpdateFlags { get; set; }
	}
}
