using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TcKs.Runtime.CmdTest.Agents.Extensions;

namespace TcKs.Runtime.CmdTest {
	class Program {
		/// <summary>
		/// Do nothing.
		/// </summary>
		static void Nop() { }
		static void Main(string[] args) {
			DoSimpleTestWorld();
		}

		static void DoSimpleTestWorld() {
			var world = TcKs.Runtime.CmdTest.Scenarios.SimpleTest.SimpleTestWorld.CreateInitialized();
			HookAgentSkills(world);

			for (int i = 0; i < 100; i++) {
				world.ProcessTick();
			}
			Nop();
		}

		static void HookAgentSkills(TcKs.Gr1dRuntime.Mechanics.World world) {
			MyAgentExtensions		.AttackDelegate	= world.MockAttack;
			MyAgentExtensions		.MoveDelegate	= world.MockMove;
			MyAgentExtensions		.ClaimDelegate	= world.MockClaim;
			MyAgentExtensions		.WaitDelegate	= world.MockWait;

			MyPirateExtensions	.CopyDelegate	= world.MockCopy;
			MyPirateExtensions	.FeintDelegate	= world.MockFeint;
			MyPirateExtensions	.Init3Delegate	= world.MockInit3;
			MyPirateExtensions	.SummonDelegate	= world.MockSummon;
			MyPirateExtensions	.VirusDelegate	= world.MockVirus;
		}
		static void UnhookAgentSkills() {
			MyAgentExtensions.AttackDelegate = null;
			MyAgentExtensions.MoveDelegate = null;
			MyAgentExtensions.ClaimDelegate = null;
			MyAgentExtensions.WaitDelegate = null;

			MyPirateExtensions.CopyDelegate = null;
			MyPirateExtensions.FeintDelegate = null;
			MyPirateExtensions.Init3Delegate = null;
			MyPirateExtensions.SummonDelegate = null;
			MyPirateExtensions.VirusDelegate = null;
		}
	}
}
