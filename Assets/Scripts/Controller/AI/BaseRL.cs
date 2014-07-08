using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof ( GridSelector ))]
[RequireComponent (typeof ( Pathfinder   ))]
public class BaseRL : GridMoveBase 
{
	public GameObject 				m_target;

	private Pathfinder				m_pathfinder;
	private Cell					m_targetCell;

	// Use this for initialization
	void Start () 
	{
		m_pathfinder = (Pathfinder)transform.GetComponent<Pathfinder>();

		base.Initialize();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{

		if( m_target != null && m_enabled )
		{
			m_targetCell = m_gridSelector.GetClosestCell( m_target.transform.position );

			List<Cell> path = m_pathfinder.GetPath( m_currentCell, m_targetCell );
			m_currentPath = path;

			//we don't want the AI trying to move onto the target.
			m_currentPath.Remove( m_currentCell );
			m_currentPath.Remove( m_targetCell );

			base.UpdateMovement();
		}
	}
}
