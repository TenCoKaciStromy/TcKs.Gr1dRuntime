using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TcKs.Runtime.CmdTest.Agents {
	public abstract class MockAgentBase : Gr1d.Api.Agent.IAgent {
	    protected Gr1d.Api.Deck.IDeck Deck { get; private set; }
	    public virtual void Initialise(Gr1d.Api.Deck.IDeck deck) {
	        this.Deck = deck;
	        this.Deck.Trace("Initialise ...", Gr1d.Api.Deck.TraceType.Information);
	    }

	    public virtual void OnArrived(Gr1d.Api.Agent.IAgentInfo arriver, Gr1d.Api.Agent.IAgentUpdateInfo agentUpdate) {
	        this.Deck.Trace("OnArrived ...", Gr1d.Api.Deck.TraceType.Information);
	    }

	    public virtual void OnAttacked(Gr1d.Api.Agent.IAgentInfo attacker, Gr1d.Api.Agent.IAgentUpdateInfo agentUpdate) {
	        this.Deck.Trace("OnAttacked ...", Gr1d.Api.Deck.TraceType.Information);
	    }

	    public virtual void Tick(Gr1d.Api.Agent.IAgentUpdateInfo agentUpdate) {
	        string msg = string.Format("Tick #{0} ...", this.Deck.TickNumber);
	        this.Deck.Trace(msg, Gr1d.Api.Deck.TraceType.Information);
	    }
	}
}
