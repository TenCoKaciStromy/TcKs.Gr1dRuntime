using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TcKs.Runtime.CmdTest.Agents.Extensions;

namespace TcKs.Runtime.CmdTest.Agents.Pirates {
	public class Pirate01Defender : Pirate01Dummy {
		public override void Tick(Gr1d.Api.Agent.IAgentUpdateInfo agentUpdate) {
			base.Tick(agentUpdate);
			this.TickSkillAttack(agentUpdate);
		}

		protected virtual Gr1d.Api.Skill.ITargetAgentResult TickSkillAttack(Gr1d.Api.Agent.IAgentUpdateInfo agentUpdate) {
			Gr1d.Api.Skill.ITargetAgentResult ret = null;

			var opponents = (from a in agentUpdate.Node.AllAgents
							 where a.Owner.Id != agentUpdate.Owner.Id
							 orderby a.Stack, a.Level
							 select a).ToList();

			var targetOpponent = opponents.FirstOrDefault();
			if (null != targetOpponent) {
				ret = this.Attack(targetOpponent);
			}

			return ret;
		}
	}
}
