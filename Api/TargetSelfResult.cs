using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gr1d.Api.Skill;

namespace TcKs.Gr1dRuntime.Api {
	public class TargetSelfResult : SkillResultBase, ITargetSelfResult {
		public SelfResultType Result { get; set; }
	}
}
