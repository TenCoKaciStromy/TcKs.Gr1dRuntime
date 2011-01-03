using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gr1d.Api.Skill;

namespace TcKs.Gr1dRuntime.Api {
	public abstract class SkillResultBase : ISkillResult {
		public int Cooldown { get; set; }

		public int LastsFor { get; set; }

		public string Message { get; set; }

		public int Warmup { get; set; }
	}
}
