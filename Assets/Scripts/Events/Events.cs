using UnityEngine;
using System.Collections;

public class CellEnteredEvent : GameEvent
{
	public Cell CellEntered;
	public GameObject ObjEntered;

	public CellEnteredEvent( Cell entered, GameObject objEntered )
	{
		ObjEntered = objEntered;
		CellEntered = entered;
	}
}

public class TurnCompletedEvent: GameEvent
{
	public GameObject TurnCompletedBy;

	public TurnCompletedEvent( GameObject obj )
	{
		TurnCompletedBy = obj;
	}
}

public class SetTurnBasedEvent: GameEvent
{
	public TurnBasedGameManager Manager;

	public SetTurnBasedEvent( TurnBasedGameManager manager )
	{
		Manager = manager;
	}
}
