using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnBasedGameManager : MonoBehaviour 
{
	public enum Team
	{
		TeamA,
		TeamB
	}

	public GameObject[] 			m_TeamA;
	public GameObject[] 			m_TeamB;

	public bool 					m_TeamAAIControlled = true;
	public bool 					m_TeamBAIControlled = true;

	public Team 					m_FirstTurnTeam = Team.TeamA;

	private Team 					m_currentTeam;
	private int 					m_roundNumber = 0;

	private List<GameObject> 		m_membersToAct;

	// Use this for initialization
	void Start () 
	{
		SetupTurnBased();

		DisableTeam( Team.TeamA );
		DisableTeam( Team.TeamB );

		m_currentTeam = m_FirstTurnTeam;
		m_roundNumber = 1;
		m_membersToAct = GenerateToActList();

		EnableTeam( m_currentTeam );

		//Register for events
		Events.instance.AddListener<TurnCompletedEvent>( MemberActed );
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( m_membersToAct.Count == 0 )
		{
			Debug.Log("Turn Over");
			NextTeam();
			m_roundNumber++;
			m_membersToAct = GenerateToActList();

			EnableTeam( m_currentTeam );
		}
	}

	public void MemberActed( TurnCompletedEvent e )
	{
		if( m_membersToAct.Contains( e.TurnCompletedBy ) )
		{
			m_membersToAct.Remove( e.TurnCompletedBy );
		}
		else
		{
			Debug.LogError( "Member not found in to-act list." );
		}
	}

	private void SetupTurnBased()
	{
		SetTurnBasedEvent e = new SetTurnBasedEvent( this );
		Events.instance.Raise( e );
	}

	private void NextTeam()
	{
		if( m_currentTeam == Team.TeamA )
		{
			m_currentTeam = Team.TeamB;
		}
		else
		{
			m_currentTeam = Team.TeamA;
		}
	}

	private void DisableTeam( Team toDisable )
	{
		SetTeamEnabled( toDisable, false );
	}

	private void SetTeamEnabled ( Team toChange, bool enable )
	{
		GameObject[] members = GetTeam( toChange );

		for ( int i = 0; i < members.Length; i++ )
		{
			members[i].SendMessage( "SetEnabled", enable, SendMessageOptions.RequireReceiver );
		}
	}

	private void EnableTeam( Team toEnable )
	{
		SetTeamEnabled( toEnable, true );
	}


	private List<GameObject> GenerateToActList()
	{
		GameObject[] toParse = GetTeam( m_currentTeam );
		List<GameObject> newToActList = new List<GameObject>();

		for( int i = 0; i < toParse.Length; i++ )
		{
			newToActList.Add( toParse[i] );
		}

		return newToActList;
	}

	private GameObject[] GetTeam( Team toRetrieve )
	{
		GameObject[] toReturn = null;
		
		switch( toRetrieve )
		{
		case Team.TeamA:
			toReturn = m_TeamA;
			break;
		case Team.TeamB:
			toReturn = m_TeamB;
			break;
		default:
			break;
		}
		
		return toReturn;
	}
}
