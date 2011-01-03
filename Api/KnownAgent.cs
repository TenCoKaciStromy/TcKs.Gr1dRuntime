using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gr1d.Api.Agent;
using Gr1d.Api.Node;
using Gr1d.Api.Player;

namespace TcKs.Gr1dRuntime.Api {
	public class KnownAgent<TAgent> : IKnownAgent<TAgent> where TAgent : IAgent {
		public TAgent Implementation { get; set; }
		public IAgentInfo OriginalAgentInfo { get; set; }

		public KnownAgent() { }
		public KnownAgent(TAgent agent, IAgentInfo agentInfo) {
			this.Implementation = agent;
		}

		public AgentAction Action {
			get { return this.OriginalAgentInfo.Action; }
		}

		public string Clan {
			get { return this.OriginalAgentInfo.Clan; }
		}

		public IEnumerable<AgentEffect> Effects {
			get { return this.OriginalAgentInfo.Effects; }
		}

		public string Group {
			get { return this.OriginalAgentInfo.Group; }
		}

		public int Heap {
			get { return this.OriginalAgentInfo.Heap; }
		}

		public Guid Id {
			get { return this.OriginalAgentInfo.Id; }
		}

		public int Level {
			get { return this.OriginalAgentInfo.Level; }
		}

		public INodeInformation Node {
			get { return this.OriginalAgentInfo.Node; }
		}

		public IPlayer Owner {
			get { return this.OriginalAgentInfo.Owner; }
		}

		public string Pack {
			get { return this.OriginalAgentInfo.Pack; }
		}

		public string Raid {
			get { return this.OriginalAgentInfo.Raid; }
		}

		public int Stack {
			get { return this.OriginalAgentInfo.Stack; }
		}

		public Type Type {
			get { return this.OriginalAgentInfo.Type; }
		}
	}
}
