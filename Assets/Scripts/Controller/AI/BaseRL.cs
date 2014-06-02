using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof ( GridSelector ))]
[RequireComponent (typeof ( Pathfinder   ))]
public class BaseRL : MonoBehaviour 
{

	public GameObject 				m_target;
	public float					m_MoveSpeed = 0.5f;

	private bool					m_move;
	private bool					m_enabled = true;

	private Pathfinder				m_pathfinder;
	private GridSelector 			m_gridSelector;
	private Cell					m_currentCell;
	private Cell					m_targetCell;

	private bool					m_isTurnBased;
	private TurnBasedGameManager	m_turnBasedManager;

	private List<Cell>				m_currentPath;

	// Use this for initialization
	void Start () 
	{
		m_pathfinder = (Pathfinder)transform.GetComponent<Pathfinder>();

		m_gridSelector = (GridSelector)transform.GetComponent<GridSelector>();

		m_currentCell = m_gridSelector.GetClosestCell( transform.position );
		if( m_currentCell == null )
		{
			Debug.LogError("Grid Movement Controller is not close enough to grid. Teleporting to grid cell (0,0)");
			m_currentCell = m_gridSelector.GetCellZero();
		}
		
		transform.position = m_currentCell.transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if( m_target != null && m_enabled )
		{
			//TODO: Consider making "current cell" public knowledge so this can be more cheaply determined.
			m_targetCell = m_gridSelector.GetClosestCell( m_target.transform.position );

			List<Cell> path = m_pathfinder.GetPath( m_currentCell, m_targetCell );
			m_currentPath = path;

			//we don't want the AI trying to move onto the target.
			m_currentPath.Remove( m_currentCell );
			m_currentPath.Remove( m_targetCell );

//			//TODO: Remove - as this is only for debugging.
//			foreach( Cell pathCell in path )
//			{
//				pathCell.HighlightCell( Color.red, 0.5f, Cell.HighlightPriority.High);
//			}

			if( m_move )
			{
				float dist = Vector3.Distance( transform.position, m_currentCell.transform.position );
				if( dist < 0.1f )
				{
					transform.position = m_currentCell.transform.position;
					m_move = false;

					if( m_isTurnBased ) 
					{
						m_turnBasedManager.MemberActed(gameObject);
						m_enabled = false;
					}

				}
				else
				{
					Vector3 newPosition = Vector3.Lerp( transform.position, m_currentCell.transform.position,  m_MoveSpeed );
					transform.position = newPosition;
				}
			}
			else
			{
				if( m_currentPath != null )
				{
					if( m_currentPath.Count > 0 )
					{
						m_move = true;
						m_currentCell = m_currentPath[0];
						m_currentPath.Remove( m_currentPath[0] );
					}
				}
			}
		}
	}

	public void SetEnabled( bool enabled )
	{
		m_enabled = enabled;
	}

	public void SetTurnBased( TurnBasedGameManager turnBasedManager )
	{
		m_isTurnBased = true;
		m_turnBasedManager = turnBasedManager;
	}
}
