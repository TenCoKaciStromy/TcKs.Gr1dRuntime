using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gr1d.Api.Agent;
using Gr1d.Api.Skill;

namespace TcKs.Gr1dRuntime.Api {
	public class TargetAgentResult : SkillResultBase, ITargetAgentResult {
		public IAgentInfo AffectedAgent { get; set; }

		public AgentResultType Result { get; set; }
	}
}
