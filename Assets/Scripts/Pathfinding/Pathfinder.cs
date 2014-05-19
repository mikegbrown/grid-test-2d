using UnityEngine;
using System.Collections.Generic;

public class Pathfinder : MonoBehaviour {

	private List<Cell>			m_closedList;
	private List<Cell>			m_openList;

	private GridManager 		m_GridManager;

	private Cell				m_startPoint;
	private Cell				m_destPoint;

	// Use this for initialization
	void Start () 
	{
		if(m_GridManager == null )
		{
			InitiateGridManger();
		}
	}

	public List<Cell> GetPath( Cell from, Cell to )
	{
		int loop1Count = 0;
		int loop2Count = 0;

		m_startPoint = from;
		m_destPoint = to;

		m_openList = new List<Cell>();
		m_closedList = new List<Cell>();

		m_openList.Add( m_startPoint );

		while( m_openList.Count > 0 )
		{
			loop1Count++;
			if(loop1Count > 100)
			{
				Debug.Log("Getting up there loop 1");
			}

			Cell currCell = m_openList[0];

			m_openList.Remove( currCell );
			m_closedList.Add( currCell );

			if( m_closedList.Contains( m_destPoint ) )
			{
				break;
			}

			foreach( GameObject adjObj in currCell.AdjacentCells )
			{
				Cell adjCell = (Cell)adjObj.transform.GetComponent<Cell>();

				if( adjCell == currCell)
				{
					Debug.LogError("Shit goin' wrong!");
				}

				if( !m_openList.Contains(adjCell) && !m_closedList.Contains(adjCell) )
				{
					if( adjCell.m_CellType == Cell.CellType.Normal )
					{
						m_openList.Add( adjCell );
						adjCell.PathfindParent = currCell;
						CalculateGHScores( adjCell, m_GridManager.m_GridType );
					}
				}
				else
				{
					float tempGScore = Vector3.Distance( adjCell.transform.position, currCell.transform.position );
					if( tempGScore < adjCell.GScore && !m_closedList.Contains(adjCell) )
					{
						if( adjCell.m_CellType == Cell.CellType.Normal )
						{
							m_openList.Add( adjCell );
							adjCell.PathfindParent = currCell;
							CalculateGHScores( adjCell, m_GridManager.m_GridType );
						}
					}
				}
				
				m_openList.Sort(new CellCompare());
			}
		}

		List<Cell> path = new List<Cell>();

		if( m_closedList.Contains (m_destPoint) )
		{
			Cell currPathCell = m_destPoint;

			while(!path.Contains(m_startPoint))
			{
				loop2Count++;
				if(loop2Count > 100)
				{
					Debug.Log("Getting up there loop 2");
				}

				path.Add(currPathCell);
				currPathCell = currPathCell.PathfindParent;

			}
		}

		return path;

	}

	private void CalculateGHScores( Cell calcCell, GridManager.GridType type )
	{
		float gScore =  Vector3.Distance( calcCell.transform.position, calcCell.PathfindParent.transform.position );
		calcCell.GScore = gScore;

		float hScore = CalculateHScore( calcCell, type );
		calcCell.HScore = hScore;

	}

	private float CalculateHScore( Cell calcCell, GridManager.GridType type )
	{
		float hScore = 0f;

		int yCells = Mathf.Abs ( calcCell.GridCoords[1] - m_destPoint.GridCoords[1] );
		int xCells = Mathf.Abs ( calcCell.GridCoords[0] - m_destPoint.GridCoords[0] );

		hScore = ( yCells + xCells ) * m_GridManager.m_GridSpacing;

		return hScore;
	}

	private float GetFScore( Cell calcCell )
	{
		return calcCell.GScore + calcCell.HScore;
	}

	private void InitiateGridManger()
	{
		m_GridManager = (GridManager)FindObjectOfType<GridManager>();
	}
}
