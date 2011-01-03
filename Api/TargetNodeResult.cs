using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gr1d.Api.Skill;
using Gr1d.Api.Node;

namespace TcKs.Gr1dRuntime.Api {
	public class TargetNodeResult : SkillResultBase, ITargetNodeResult {
		public INodeInformation AffectedNode { get; set; }

		public NodeResultType Result { get; set; }
	}
}
