using System.Collections.Generic;


[System.Serializable]
public class State
{
	public System.Action begin, update, end;

}

public class FSM
{
	List<System.Action> states;

	public FSM ()
	{
		this.states = new List<System.Action> ();
	}

	public void DoState ()
	{
		var currentStateFunction = GetCurrentState ();
				
		if (currentStateFunction != null)
			currentStateFunction ();
	}

	public System.Action PopState ()
	{
		return states.Pop ();
	}

	public void PushState (System.Action state)
	{
		if (GetCurrentState () != state)
			states.Push (state);
	}

	public System.Action GetCurrentState ()
	{
		return states.Count > 0 ? states [states.Count - 1] : null;
	}
}
