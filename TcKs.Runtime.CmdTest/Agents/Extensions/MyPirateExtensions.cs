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
	public static class MyPirateExtensions {
		public static Func<IPirate1, ITargetSelfResult> CopyDelegate { get; set; }
		public static ITargetSelfResult Copy(this IPirate1 self) {
			var dlg = CopyDelegate;
			if (null != dlg) { return dlg(self); }

			return IPirate1SkillExtensions.Copy(self);
		}

		public static Func<IPirate2, IAgentInfo, ITargetAgentResult> Init3Delegate { get; set; }
		public static ITargetAgentResult Init3(this IPirate2 self, IAgentInfo target) {
			var dlg = Init3Delegate;
			if (null != dlg) { return dlg(self, target); }

			return IPirate2SkillExtensions.Init3(self,target);
		}

		public static Func<IPirate3, IAgentInfo, ITargetAgentResult> FeintDelegate { get; set; }
		public static ITargetAgentResult Feint(this IPirate3 self, IAgentInfo target) {
			var dlg = FeintDelegate;
			if (null != dlg) { return dlg(self, target); }

			return IPirate3SkillExtensions.Feint(self, target);
		}

		public static Func<IPirate4, IAgentInfo, ITargetAgentResult> SummonDelegate { get; set; }
		public static ITargetAgentResult Summon(this IPirate4 self, IAgentInfo target) {
			var dlg = SummonDelegate;
			if (null != dlg) { return dlg(self, target); }

			return IPirate4SkillExtensions.Summon(self, target);
		}

		public static Func<IPirate5, INodeInformation, ITargetNodeResult> VirusDelegate { get; set; }
		public static ITargetNodeResult Virus(this IPirate5 self, INodeInformation target) {
			var dlg = VirusDelegate;
			if (null != dlg) { return dlg(self, target); }

			return IPirate5SkillExtensions.Virus(self, target);
		}
	}
}
