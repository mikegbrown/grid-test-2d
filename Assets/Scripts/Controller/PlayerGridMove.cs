using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof ( GridSelector ))]
[RequireComponent (typeof ( Pathfinder   ))]
public class PlayerGridMove : GridMoveBase 
{
	private Pathfinder				m_pathfinder;

	// Use this for initialization
	void Start () 
	{
		base.Initialize();
		m_pathfinder = (Pathfinder)transform.GetComponent<Pathfinder>();
		m_gridSelector.cellSelectedDelegate = CellSelected;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if( m_enabled )
		{
			base.UpdateMovement();
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

			m_currentPath = path;

			foreach( Cell pathCell in path )
			{
				pathCell.HighlightCell( Color.green, 0.5f, Cell.HighlightPriority.High);
			}
		}

	}
}
