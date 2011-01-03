using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TcKs.Runtime.CmdTest.Agents.Extensions;

namespace TcKs.Runtime.CmdTest.Agents.Pirates {
	public class Pirate01Scout : Pirate01Dummy {
		public override void Tick(Gr1d.Api.Agent.IAgentUpdateInfo agentUpdate) {
			base.Tick(agentUpdate);
			this.TickSkillMove(agentUpdate);
		}

		protected virtual Gr1d.Api.Skill.ITargetNodeResult TickSkillMove(Gr1d.Api.Agent.IAgentUpdateInfo agentUpdate) {
			Gr1d.Api.Skill.ITargetNodeResult ret = null;

			var rnd = new Random();
			var nodes = (from n in agentUpdate.Node.Exits.Values
						 orderby rnd.Next()
						 select n).ToList();

			var targetNode = nodes.FirstOrDefault();
			if (null != targetNode) {
				ret = this.Move(targetNode);
				string msg = string.Format("Current node=[L:{0} R:{1} C:{2}]", targetNode.Layer, targetNode.Row, targetNode.Column);
				this.Deck.Trace(msg, Gr1d.Api.Deck.TraceType.Information);
			}

			return ret;
		}
	}
}
