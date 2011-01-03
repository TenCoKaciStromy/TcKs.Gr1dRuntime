using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gr1d.Api;
using Gr1d.Api.Agent;
using Gr1d.Api.Deck;
using Gr1d.Api.Node;
using Gr1d.Api.Player;

namespace TcKs.Gr1dRuntime.Api {
	/// <summary>
	/// Base class for implementation of IDeck.
	/// </summary>
	public abstract class DeckBase : IDeck {
		public Func<IEnumerable<KeyValuePair<IAgent, IAgentInfo>>> AgentsProvider { get; set; }

		public IAgentInfo Endpoint {
			get { throw new NotImplementedException(); }
		}

		public IEnumerable<IKnownAgent<AgentType>> GetInitialisedAgents<AgentType>() where AgentType : IAgent {
			IEnumerable<IKnownAgent<AgentType>> ret = null;
			var agentsProvider = this.AgentsProvider;
			if (null != agentsProvider) {
				var agents = agentsProvider();
				if (null != agents) {
					ret = (from kp in agents
						   let agent = kp.Key
						   where agent is AgentType
						   let typedAgent = (AgentType)agent
						   let info = kp.Value
						   where null != info
						   let knownAgent = new KnownAgent<AgentType>(typedAgent, info)
						   select knownAgent).ToArray();
				}
			}

			if (null == ret) {
				ret = new IKnownAgent<AgentType>[0];
			}
			return ret;
		}

		public int Resolve(INodeInformation node) {
			throw new NotImplementedException();
		}

		public int TickNumber { get; set; }

		public virtual void Trace(string message, string category, params object[] args) {
			var str = string.Format(message, args);
			this.Trace(str, category);
		}

		public abstract void Trace(string message, string category);
		public virtual void Trace(string message, TraceType type, params object[] args) {
			var str = string.Format(message, args);
			this.Trace(str, type);
		}

		public virtual void Trace(string message, TraceType type) {
			var category = type.ToString();
			this.Trace(message, category);
		}
	}
}
