using UnityEngine;
using System.Collections;

public class GridSelector : MonoBehaviour 
{
	private GridManager 			m_GridManager;

	public delegate void 			CellSelectedDelegate( Cell selectedCell );
	public CellSelectedDelegate 	cellSelectedDelegate; 

	public delegate void 			CellHoverDelegate( Cell hoverCell );
	public CellHoverDelegate 		cellHoverDelegate;

	// Use this for initialization
	void Start () 
	{
		if(m_GridManager == null )
		{
			InitiateGridManger();
		}
	}

	// Update is called once per frame
	void Update () 
	{
		Cell closestCell = m_GridManager.GetClosestCellToMouse();
		if( closestCell != null )
		{
			if(cellHoverDelegate != null)
				cellHoverDelegate( closestCell );

			if( Input.GetButtonDown( "Fire1" ))
		   	{
				if(cellSelectedDelegate != null)
					cellSelectedDelegate( closestCell );
			}
		}
	}

	private void InitiateGridManger()
	{
		m_GridManager = (GridManager)FindObjectOfType<GridManager>();
	}

	public Cell GetClosestCell( Vector3 position )
	{
		if(m_GridManager == null )
		{
			InitiateGridManger();
		}

		return m_GridManager.GetClosestCell( position );
	}

	public Cell GetCellZero()
	{
		if(m_GridManager == null )
		{
			InitiateGridManger();
		}

		return m_GridManager.GetCellByName("Cell(0,0)");
	}



}
