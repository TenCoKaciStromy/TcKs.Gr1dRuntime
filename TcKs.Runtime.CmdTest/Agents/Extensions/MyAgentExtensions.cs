using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gr1d.Api;
using Gr1d.Api.Agent;
using Gr1d.Api.Deck;
using Gr1d.Api.Node;
using Gr1d.Api.Skill;

namespace TcKs.Runtime.CmdTest.Agents.Extensions {
	public static class MyAgentExtensions {
		public static Func<IAgent, IAgentInfo, ITargetAgentResult> AttackDelegate { get; set; }
		public static ITargetAgentResult Attack(this IAgent self, IAgentInfo target) {
			var dlg = AttackDelegate;
			if (null != dlg) { return dlg(self, target); }

			return IAgentSkillExtensions.Attack(self, target);
		}

		public static Func<IAgent, INodeInformation, ITargetNodeResult> MoveDelegate { get; set; }
		public static ITargetNodeResult Move(this IAgent self, INodeInformation target) {
			var dlg = MoveDelegate;
			if (null != dlg) { return dlg(self, target); }

			return IAgentSkillExtensions.Move(self, target);
		}

		public static Func<IAgent, INodeInformation, ITargetNodeResult> ClaimDelegate { get; set; }
		public static ITargetNodeResult Claim(this IAgent self, INodeInformation target) {
			var dlg = ClaimDelegate;
			if (null != dlg) { return dlg(self, target); }

			return IAgentSkillExtensions.Claim(self, target);
		}

		public static Func<IAgent, ITargetSelfResult> WaitDelegate { get; set; }
		public static ITargetSelfResult Wait(this IAgent self) {
			var dlg = WaitDelegate;
			if (null != dlg) { return dlg(self); }

			return IAgentSkillExtensions.Wait(self);
		}
	}
}
