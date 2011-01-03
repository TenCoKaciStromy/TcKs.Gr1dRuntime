using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Gr1d.Api;
using Gr1d.Api.Agent;
using Gr1d.Api.Deck;
using Gr1d.Api.Node;
using Gr1d.Api.Player;
using Gr1d.Api.Skill;

using TcKs.Gr1dRuntime.Api;
using TcKs.Gr1dRuntime.Console;

namespace TcKs.Gr1dRuntime.Mechanics {
	public class World {
		#region Nested types
		/// <summary>
		/// Contains informations about agents thread.
		/// </summary>
		public class AgentThreadInfo {
			/// <summary>
			/// Thread of agent.
			/// </summary>
			public System.Threading.Thread Thread { get; private set; }
			/// <summary>
			/// Reset event for main loop.
			/// </summary>
			public System.Threading.ManualResetEvent MainReset { get; private set; }
			/// <summary>
			/// Reset event for agent loop.
			/// </summary>
			public System.Threading.ManualResetEvent AgentReset { get; private set; }

			public AgentThreadInfo(System.Threading.Thread thread) {
				if (object.ReferenceEquals(thread, null)) { throw new ArgumentNullException("thread"); }

				this.Thread = thread;
				this.MainReset = new System.Threading.ManualResetEvent(false);
				this.AgentReset = new System.Threading.ManualResetEvent(false);
			}
		}

		protected class AgentWorkerStartInfo {
			public IAgent Agent { get; set; }
			public IAgentUpdateInfo AgentUpdateInfo { get; set; }
			public AgentThreadInfo Thread { get; set; }

			public AgentWorkerStartInfo() { }
			public AgentWorkerStartInfo( IAgent agent, IAgentUpdateInfo agentUpdateInfo, AgentThreadInfo thread ) {
				this.Agent = agent;
				this.AgentUpdateInfo = agentUpdateInfo;
				this.Thread = thread;
			}
		}
		#endregion Nested types

		#region World properties
		public Dictionary<IPlayer, IDeck> Players { get; private set; }
		public Dictionary<IAgent, IAgentUpdateInfo> Agents { get; private set; }
		public LinkedList<INodeInformation> Nodes { get; set; }
		public Dictionary<IAgent, AgentThreadInfo> Processes { get; private set; }
		public int TickNumber { get; set; }
		#endregion

		public World() {
			this.Players = new Dictionary<IPlayer, IDeck>();
			this.Agents = new Dictionary<IAgent, IAgentUpdateInfo>();
			this.Processes = new Dictionary<IAgent, AgentThreadInfo>();
		}

		#region World methods
		public virtual IDeck AddPlayer(IPlayer player) {
			if (object.ReferenceEquals(player, null)) { throw new ArgumentNullException("player"); }

			var ret = new ConsoleDeck(); //TODO: factory method
			ret.AgentsProvider = () => {
				var _ret = this.Agents
							.Where(kp => kp.Value.Owner.Id == player.Id)
							.Select(kp => new KeyValuePair<IAgent, IAgentInfo>(kp.Key, kp.Value))
							.ToList();
				return _ret;
			};
			this.Players.Add(player, ret);
			return ret;
		}
		public virtual IAgentUpdateInfo AddAgent(IAgent agent, int level, IPlayer owner) {
			var deck = this.Players[owner];
			agent.Initialise(deck);

			var firstNode = this.Nodes.First.Value;
			var ret = new AgentUpdateInfo(agent) {
				Action = AgentAction.None,
				Id = Guid.NewGuid(),
				Level = level,
				Owner = owner,
				Stack = 27
			};
			this.Agents.Add(agent, ret);


			return ret;
		}
		public virtual void LaunchAgent(IAgent agent) {
			var agentUpdateInfo = this.Agents[agent];
			var mockAgentUpdateInfo = (AgentInfo)agentUpdateInfo;

			var currentNode = agentUpdateInfo.Node;
			if (null != currentNode) {
				throw new InvalidOperationException("Agent is ALREADY launched.");
			}

			var firstNode = this.Nodes.First.Value;
			var mockFirstNode = (NodeInformation)firstNode;
			mockAgentUpdateInfo.Node = firstNode;
			mockFirstNode.AllAgentsList.Add(agentUpdateInfo);
		}
		public virtual void RecallAgent(IAgent agent) {
			var agentUpdateInfo = this.Agents[agent];
			var mockAgentUpdateInfo = (AgentInfo)agentUpdateInfo;

			var currentNode = agentUpdateInfo.Node;
			if (null == currentNode) {
				throw new InvalidOperationException("Agent is NOT launched.");
			}

			var mockCurrentNode = (NodeInformation)currentNode;
			mockCurrentNode.AllAgentsList.Remove(agentUpdateInfo);
			mockAgentUpdateInfo.Node = null;
			this.Agents.Remove(agent);
			this.Processes.Remove(agent);
		}
		public virtual void CrashAgent(IAgent agent) {
			this.RecallAgent(agent);
		}
		public virtual void ProcessTick() {
			this.TickNumber++;
			foreach (DeckBase deck in this.Players.Values) {
				deck.TickNumber = this.TickNumber;
			}

			var newlyLaunchedAgents = this.Agents.Keys.Except(this.Processes.Keys).ToList();
			foreach (var __newlyLaunchedAgent in newlyLaunchedAgents) {
				var newlyLaunchedAgent = __newlyLaunchedAgent;
				var agentInfo = this.Agents[newlyLaunchedAgent];

				var thread = new System.Threading.Thread(this.AgentWorker);
				thread.IsBackground = true;
				thread.Priority = System.Threading.ThreadPriority.Lowest;
				thread.Name = string.Format("{0}({1}({2}))"
					, agentInfo.Owner.DisplayHandle
					, newlyLaunchedAgent.GetType().FullName
					, agentInfo.Level);

				var threadEx = new AgentThreadInfo(thread);
				this.Processes.Add(newlyLaunchedAgent, threadEx);
			}

			var rnd = new Random();
			foreach (var kpProc in this.Processes.OrderBy(p => rnd.Next())) {
				var agent = kpProc.Key;
				var thread = kpProc.Value;
				IAgentUpdateInfo agentInfo;
				this.Agents.TryGetValue(agent, out agentInfo);
				if (null == agentInfo) { continue; /* agent crashed or was recalled */ }

				var mainReset = thread.MainReset;
				var workerReset = thread.AgentReset;

				if (false == thread.Thread.IsAlive) {
					workerReset.Reset();
					var startInfo = new AgentWorkerStartInfo(agent, agentInfo, thread);
					thread.Thread.Start(startInfo);
				}

				mainReset.Reset();
				workerReset.Set();
				mainReset.WaitOne();
			}
		}
		#endregion World methods

		#region Trace methods
		public virtual void Trace(params object[] values) {
			this.Trace(string.Empty, values);
		}
		public virtual void Trace(string category, params object[] values) {
			var text = new StringBuilder();
			if (false == string.IsNullOrEmpty(category)) {
				text.Append(category).Append(":");
			}
			foreach (var value in values) {
				string strValue;
				if (object.ReferenceEquals(value, null)) {
					strValue = "NULL";
				}
				else {
					strValue = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}", value);
				}

				text.Append(" ").Append(value);
			}

			System.Console.Write(text.ToString());
		}

		public virtual void Trace(IAgent agent, params object[] values) {
			this.Trace(agent, string.Empty, values);
		}
		public virtual void Trace(IAgent agent, TraceType traceType, params object[] values) {
			this.Trace(agent, traceType.ToString(), values);
		}
		public virtual void Trace(IAgent agent, string category, params object[] values) {
			var agentInfo = this.Agents[agent];
			this.Trace(agentInfo, category, values);
		}

		public virtual void Trace(IAgentInfo agentInfo, params object[] values) {
			this.Trace(agentInfo, string.Empty, values);
		}
		public virtual void Trace(IAgentInfo agentInfo, TraceType traceType, params object[] values) {
			this.Trace(agentInfo, traceType.ToString(), values);
		}
		public virtual void Trace(IAgentInfo agentInfo, string category, params object[] values) {
			var deck = this.Players[agentInfo.Owner];

			var text = new StringBuilder();
			foreach (var value in values) {
				string strValue;
				if (object.ReferenceEquals(value, null)) {
					strValue = "NULL";
				}
				else {
					strValue = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}", value);
				}

				text.Append(" ").Append(value);
			}

			deck.Trace(text.ToString(), category);
		}
		#endregion Trace methods

		#region Agent methods
		protected virtual void AgentWorker( object startInfo ) {
			var typedStartInfo = (AgentWorkerStartInfo)startInfo;
			this.AgentWorker(typedStartInfo);
		}
		protected virtual void AgentWorker( AgentWorkerStartInfo startInfo ) {
			if (object.ReferenceEquals(startInfo, null)) { throw new ArgumentNullException("startInfo"); }

			var agent = startInfo.Agent;
			if (object.ReferenceEquals(agent, null)) { throw new ArgumentNullException("startInfo.Agent"); }
			var agentInfo = startInfo.AgentUpdateInfo;
			if (object.ReferenceEquals(agent, null)) { throw new ArgumentNullException("startInfo.AgentUpdateInfo"); }
			var thread = startInfo.Thread;
			if (object.ReferenceEquals(agent, null)) { throw new ArgumentNullException("startInfo.Thread"); }

			var mainReset = thread.MainReset;
			var workerReset = thread.AgentReset;

			var currThreadName = System.Threading.Thread.CurrentThread.Name;

			Debug.WriteLine(string.Format("THREAD {0} STARTED", currThreadName));
			while (agentInfo.Stack > 0) {
				Debug.WriteLine(string.Format("THREAD {0} WAITS for reset event", currThreadName));
				workerReset.WaitOne();
				workerReset.Reset();

				System.Diagnostics.Debug.WriteLine(string.Format("Tick ({0}:{1}) >>>", agentInfo.Owner.DisplayHandle, agent.GetType().FullName));
				agent.Tick(agentInfo);
				System.Diagnostics.Debug.WriteLine(string.Format("<<< Tick ({0}:{1})", agentInfo.Owner.DisplayHandle, agent.GetType().FullName));

				workerReset.Reset();
				mainReset.Set();
			}
			this.CrashAgent(agent);

			System.Diagnostics.Debug.WriteLine(string.Format("THREAD {0} FINISHED", currThreadName));
		}
		#endregion Agent methods

		#region Agent skill methods
		protected virtual void WaitToNextTick(IAgent self) {
			var thread = this.Processes[self];
			var mainReset = thread.MainReset;
			var workerReset = thread.AgentReset;

			mainReset.Set();
			workerReset.WaitOne();
			workerReset.Reset();
		}
		public virtual ITargetNodeResult MockMove(IAgent self, INodeInformation target) {
			ITargetNodeResult ret;

			var okTarget = (NodeInformation)target;
			var selfAgentUpdateInfo = this.Agents[self];
			var okSelfAgentUpdateInfo = (AgentUpdateInfo)selfAgentUpdateInfo;

			var currentNode = (from n in target.Exits.Values
							   let a = n.AllAgents.Where(ag => ag.Id == selfAgentUpdateInfo.Id).FirstOrDefault()
							   where null != a
							   select n).FirstOrDefault();

			this.WaitToNextTick(self);
			if (null == currentNode) {
				ret = new TargetNodeResult {
					AffectedNode = null,
					Result = NodeResultType.FailedOutOfRange,
					Cooldown = 1,
					LastsFor = 1,
					Warmup = 1,
					Message = "MOCK: Target node is not nesting with current node of agent."
				};
			}
			else {
				var okCurrentNode = (NodeInformation)currentNode;
				okCurrentNode.AllAgentsList.Remove(selfAgentUpdateInfo);
				okTarget.AllAgentsList.Add(selfAgentUpdateInfo);
				okSelfAgentUpdateInfo.NodePrevious = currentNode;
				okSelfAgentUpdateInfo.Node = target;

				ret = new TargetNodeResult {
					AffectedNode = null,
					Result = NodeResultType.Success,
					Cooldown = 1,
					LastsFor = 1,
					Warmup = 1,
					Message = "Success mock move."
				};
			}

			this.Trace(self, TraceType.Information
					, "MOCK-Move!"
					, "Self={", self, "}"
					, "Target={", target, "}"
					, "Result={", ret, "}");

			return ret;
		}

		public virtual ITargetAgentResult MockAttack(IAgent self, IAgentInfo target) {
			ITargetAgentResult ret;

			var selfAgentInfo = this.Agents[self];
			if (selfAgentInfo.Node == target.Node) {
				ret = new TargetAgentResult {
					AffectedAgent = target,
					Result = AgentResultType.Success,
					Cooldown = 1,
					LastsFor = 1,
					Warmup = 1,
					Message = "Success mock attack."
				};
			}
			else {
				ret = new TargetAgentResult {
					AffectedAgent = target,
					Result = AgentResultType.FailedOutOfRange,
					Cooldown = 1,
					LastsFor = 1,
					Warmup = 1,
					Message = "Failed (out-of-range) mock attack."
				};
			}
			
			this.WaitToNextTick(self);
			if (ret.Result == AgentResultType.Success) {
				var okTarget = (AgentInfo)target;
				okTarget.Stack--;
			}
			
			this.Trace(self, TraceType.Information
				, "MOCK-Attack!"
				, "Self={", self, "}"
				, "Target={", target, "}"
				, "Result={", ret, "}");

			return ret;
		}

		public virtual ITargetNodeResult MockClaim(IAgent self, INodeInformation target) {
			ITargetNodeResult ret;

			var selfAgentInfo = (AgentInfo)self;
			if (selfAgentInfo.Node == target) {
				if (target.Owner.Id == selfAgentInfo.Owner.Id) {
					ret = new TargetNodeResult {
						AffectedNode = target,
						Result = NodeResultType.FailedInvalidTarget,
						Cooldown = 1,
						LastsFor = 1,
						Warmup = 1,
						Message = "Failed (invalid-target: claim own node) mock claim."
					};
				}
				else {
					ret = new TargetNodeResult {
						AffectedNode = target,
						Result = NodeResultType.Success,
						Cooldown = 1,
						LastsFor = 1,
						Warmup = 1,
						Message = "Success mock claim."
					};
				}
			}
			else {
				ret = new TargetNodeResult {
					AffectedNode = target,
					Result = NodeResultType.FailedOutOfRange,
					Cooldown = 1,
					LastsFor = 1,
					Warmup = 1,
					Message = "Failed (out-of-range) mock claim."
				};
			}
			this.WaitToNextTick(self);

			if (ret.Result == NodeResultType.Success) {
				var mockTarget = (NodeInformation)target;
				mockTarget.Owner = selfAgentInfo.Owner;
			}
			this.Trace(self, TraceType.Information
				, "MOCK-Claim!"
				, "Self={", self, "}"
				, "Target={", target, "}"
				, "Result={", ret, "}");

			return ret;
		}

		public virtual ITargetSelfResult MockWait(IAgent self) {
			this.WaitToNextTick(self);
			var ret = new TargetSelfResult {
				Result = SelfResultType.Success,
				Cooldown = 1,
				LastsFor = 1,
				Warmup = 1,
				Message = "Success mock wait."
			};
			this.Trace(self, TraceType.Information
				, "MOCK-Claim!"
				, "Self={", self, "}"
				, "Result={", ret, "}");

			return ret;
		}
		#endregion Agent skill methods

		#region Pirate skill methods
		public virtual ITargetSelfResult MockCopy(IAgent self) {
			ITargetSelfResult ret;

			var selfAgentInfo = this.Agents[self];
			var okSelfAgentUpdateInfo = (AgentUpdateInfo)selfAgentInfo;
			var selfPlayer = selfAgentInfo.Owner;
			var okSelfPlayer = (Player)selfPlayer;
			var selfDeck = this.Players[selfPlayer];

			this.WaitToNextTick(self);
			var copiesCount = (from a in selfDeck.GetInitialisedAgents<IAgent>()
							   where a.Effects.Contains(AgentEffect.Copy)
							   select a).Count() / 2;
			if (copiesCount < okSelfPlayer.Level) {
				var objCopiedAgent = Activator.CreateInstance(selfPlayer.GetType());
				var copiedAgent = (IAgent)objCopiedAgent;
				var copiedAgentInfo = this.AddAgent(copiedAgent, selfAgentInfo.Level, selfPlayer);

				var okCopiedAgentInfo = (AgentUpdateInfo)copiedAgentInfo;
				okCopiedAgentInfo.Action = selfAgentInfo.Action;
				okCopiedAgentInfo.Clan = selfAgentInfo.Clan;
				okCopiedAgentInfo.EffectsList.Add(AgentEffect.Copy);
				okCopiedAgentInfo.Group = selfAgentInfo.Group;
				okCopiedAgentInfo.Heap = 0;
				okCopiedAgentInfo.Id = Guid.NewGuid();
				okCopiedAgentInfo.Node = selfAgentInfo.Node;
				okCopiedAgentInfo.Stack = 1;

				okSelfAgentUpdateInfo.EffectsList.Add(AgentEffect.Copy);
				this.LaunchAgent(copiedAgent);

				ret = new TargetSelfResult {
					Result = SelfResultType.Success,
					Cooldown = 1,
					LastsFor = 1,
					Warmup = 1,
					Message = "Success mock copy."
				};
			}
			else {
				ret = new TargetSelfResult {
					Result = SelfResultType.FailedInvalid,
					Cooldown = 1,
					LastsFor = 1,
					Warmup = 1,
					Message = "Failed (too-much-copies) mock copy."
				};
			}

			this.Trace(self, TraceType.Information
				, "MOCK-Claim!"
				, "Self={", self, "}"
				, "Result={", ret, "}");

			return ret;
		}

		public virtual ITargetAgentResult MockInit3(IAgent self, IAgentInfo target) {
			ITargetAgentResult ret;

			var selfAgentInfo = this.Agents[self];
			if (selfAgentInfo.Node == target.Node) {
				ret = new TargetAgentResult {
					AffectedAgent = target,
					Result = AgentResultType.Success,
					Cooldown = 1,
					LastsFor = 1,
					Warmup = 1,
					Message = "Success mock Init3."
				};
			}
			else {
				ret = new TargetAgentResult {
					AffectedAgent = target,
					Result = AgentResultType.FailedOutOfRange,
					Cooldown = 1,
					LastsFor = 1,
					Warmup = 1,
					Message = "Failed (out-of-range) mock Init3."
				};
			}

			this.WaitToNextTick(self);
			if (ret.Result == AgentResultType.Success) {
				var okTarget = (AgentUpdateInfo)target;
				var badEffects = BasicInfo.GetBadAgentEffects();
				okTarget.EffectsList.RemoveAll(e => badEffects.Contains(e));
			}
			
			this.Trace(self, TraceType.Information
				, "MOCK-Attack!"
				, "Self={", self, "}"
				, "Target={", target, "}"
				, "Result={", ret, "}");

			return ret;
		}

		public virtual ITargetAgentResult MockFeint(IAgent self, IAgentInfo target) {
			ITargetAgentResult ret;

			var selfAgentInfo = this.Agents[self];
			if (selfAgentInfo.Node == target.Node) {
				ret = new TargetAgentResult {
					AffectedAgent = null,
					Result = AgentResultType.Success,
					Cooldown = 1,
					LastsFor = 1,
					Warmup = 1,
					Message = "Success mock Feint."
				};
			}
			else {
				ret = new TargetAgentResult {
					AffectedAgent = null,
					Result = AgentResultType.FailedOutOfRange,
					Cooldown = 1,
					LastsFor = 1,
					Warmup = 1,
					Message = "Failed (out-of-range) mock Feint."
				};
			}

			this.WaitToNextTick(self);
			if (ret.Result == Gr1d.Api.Skill.AgentResultType.Success) {
				var okTarget = (AgentInfo)target;
				okTarget.EffectsList.Add(AgentEffect.Feint);
			}

			this.Trace(self, TraceType.Information
				, "MOCK-Attack!"
				, "Self={", self, "}"
				, "Target={", target, "}"
				, "Result={", ret, "}");

			return ret;
		}

		public virtual ITargetAgentResult MockSummon(IAgent self, IAgentInfo target) {
			ITargetAgentResult ret;
			var selfAgentInfo = this.Agents[self];

			if (selfAgentInfo.Owner.Id == target.Owner.Id) {
				ret = new TargetAgentResult {
					AffectedAgent = target,
					Result = AgentResultType.Success,
					Cooldown = 1,
					LastsFor = 1,
					Warmup = 1,
					Message = "Success mock summon."
				};
			}
			else {
				ret = new TargetAgentResult {
					AffectedAgent = target,
					Result = AgentResultType.FailedInvalidTarget,
					Cooldown = 1,
					LastsFor = 1,
					Warmup = 1,
					Message = "Failed (invalid-target) mock summon."
				};
			}

			this.WaitToNextTick(self);
			if (ret.Result == Gr1d.Api.Skill.AgentResultType.Success) {
				var okTarget = (AgentInfo)target;
				okTarget.Node = selfAgentInfo.Node;
			}

			this.Trace(self, TraceType.Information
				, "MOCK-Attack!"
				, "Self={", self, "}"
				, "Target={", target, "}"
				, "Result={", ret, "}");

			return ret;
		}

		public virtual ITargetNodeResult MockVirus(IAgent self, INodeInformation target) {
			ITargetNodeResult ret;

			ret = new TargetNodeResult {
				AffectedNode = target,
				Cooldown = 1,
				LastsFor = 1,
				Warmup = 1,
				Result = NodeResultType.FailedDifficulty,
				Message = "Failed (not-implemented-by-runtime) mock virus."
			};
			this.WaitToNextTick(self);

			return ret;
		}
		#endregion Pirate skill methods
	}
}
