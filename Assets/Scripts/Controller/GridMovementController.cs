using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (GridSelector))]
[RequireComponent (typeof (Pathfinder))]
public class GridMovementController : MonoBehaviour 
{
	public float					m_MoveSpeed = 0.5f;

	private GridSelector 			m_gridSelector;
	private Pathfinder				m_pathfinder;
	private Cell					m_currentCell;
	
	private bool					m_move = false;
	private List<Cell>				m_currentPath;

	// Use this for initialization
	void Start () 
	{
		m_pathfinder = (Pathfinder)transform.GetComponent<Pathfinder>();

		m_gridSelector = (GridSelector)transform.GetComponent<GridSelector>();
		m_gridSelector.cellSelectedDelegate = CellSelected;

		m_currentCell = m_gridSelector.GetClosestCell( transform.position );
		if( m_currentCell == null )
		{
			Debug.LogError("Grid Movement Controller is not close enough to grid. Teleporting to grid cell (0,0)");
			m_currentCell = m_gridSelector.GetCellZero();
		}

		transform.position = m_currentCell.transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if( m_move )
		{
			float dist = Vector3.Distance( transform.position, m_currentCell.transform.position );
			if( dist < 0.1f )
			{
				transform.position = m_currentCell.transform.position;
				m_move = false;
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

	private void CellSelected( Cell selectedCell )
	{
		if( m_currentCell.IsConnectedToCell( selectedCell ) )
		{
			//transform.position = selectedCell.transform.position;
			if( selectedCell.m_CellType == Cell.CellType.Normal )
			{
				m_currentCell = selectedCell;
				m_move = true;
			}
		}
		else
		{
			List<Cell> path = m_pathfinder.GetPath( m_currentCell, selectedCell );
			path.Reverse();

			m_currentPath = path;

			foreach( Cell pathCell in path )
			{
				pathCell.HighlightCell( Color.green, 0.5f, Cell.HighlightPriority.High);
			}
		}

	}
}
