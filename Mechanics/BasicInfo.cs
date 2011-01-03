using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gr1d.Api.Agent;
using Gr1d.Api.Node;

namespace TcKs.Gr1dRuntime.Mechanics {
	public abstract class BasicInfo {
		public static AgentEffect[] GetBadAgentEffects() {
			var ret = new AgentEffect[]{
				AgentEffect.Pin,
				AgentEffect.Decompile,
				AgentEffect.Worm,
				AgentEffect.Bribe,
				AgentEffect.Feint,
				AgentEffect.Botnet,
				AgentEffect.Scam,
				AgentEffect.Compress,
				AgentEffect.Banshee,
				AgentEffect.Interfere
			};

			return ret;
		}
		public static AgentEffect[] GetGoodAgentEffects() {
			var ret = new AgentEffect[]{
				AgentEffect.UnitTest,
				AgentEffect.Mentor,
				AgentEffect.Haste,
				AgentEffect.Mercenary,
				AgentEffect.Copy,
				AgentEffect.StreetSmarts,
				AgentEffect.Rally,
				AgentEffect.Fade,
				AgentEffect.Feedback,
				AgentEffect.su
			};

			return ret;
		}
		public static NodeEffect[] GetBadNodeEffects() {
			var ret = new NodeEffect[]{
				NodeEffect.Bitrot,
				NodeEffect.Fragment,
				NodeEffect.MarketForces,
				NodeEffect.Struts,
				NodeEffect.Virus
			};

			return ret;
		}
		public static NodeEffect[] GetNeutralNodeEffects() {
			var ret = new NodeEffect[]{
				NodeEffect.Scaffold
			};

			return ret;
		}
	}
}
