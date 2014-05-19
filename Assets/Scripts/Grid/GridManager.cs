using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridManager : MonoBehaviour 
{
	public enum GridType
	{
		Square4way,
		Square8way,
		Hex
	}

	public bool					m_UpdateConnectionsOnStart = false;
	public bool					m_CreateGridOnStart = false;
	public GameObject			m_CellObject;
	public GridType 			m_GridType;
	public float				m_GridSpacing = 10.0f;
	public float 				m_HexYModifier = 2.3f;
	public int					m_HorizontalCells = 10;
	public int					m_VerticalCells = 10;

	private bool				m_hasUpdated = false;
	private Vector3				m_origin;
	private List<GameObject>	m_cells;

	private Cell				m_closestToMouse;

	// Use this for initialization
	void Start () 
	{
		m_origin = transform.position;
		m_cells = new List<GameObject>();
		if( m_CreateGridOnStart )
		{
			CreateGrid();
		}
		else
		{
			//in case m_cells is empty, but GridManager has cell child Objects
			foreach( Transform child in transform )
			{
				Cell cell = (Cell)child.GetComponent<Cell>();
				if(cell != null )
				{
					m_cells.Add( child.gameObject );
				}
			}
		}
	}
	// Update is called once per frame
	void Update () 
	{
		if( !m_hasUpdated && m_UpdateConnectionsOnStart )
		{
			UpdateAllConnections();
			m_hasUpdated = true;
		}

		UpdateClosestCellToMouse();
	}

	public void UpdateAllConnections()
	{
		float dist = m_GridSpacing;
		switch( m_GridType )
		{
			case GridType.Square8way:
				dist = Mathf.Sqrt( 2.0f * Mathf.Pow(m_GridSpacing, 2.0f ) ) ;
				break;
			default:
				break;
		}
		
		foreach( GameObject cellObj in m_cells )
		{
			Cell cell = (Cell)cellObj.GetComponent<Cell>();
			cell.UpdateConnectedCells( dist );
		}
	}

	public void DeleteGrid()
	{
		List<GameObject> toDelete = new List<GameObject>();
		//in case m_cells is empty, but GridManager has cell child Objects
		foreach( Transform child in transform )
		{
			if(child.GetComponent<Cell>() != null )
				toDelete.Add ( child.gameObject );
		}
		foreach( GameObject obj in toDelete )
		{
			DestroyImmediate(obj);
		}

		if(m_cells != null)
			m_cells.Clear();
	}

	public void CreateGrid()
	{
		CreateGrid(false);
	}

	public void CreateGrid(bool withUpdate)
	{
		DeleteGrid();

		if(m_cells == null)
			m_cells = new List<GameObject>();

		m_origin = transform.position;

		switch( m_GridType )
		{
			case GridType.Hex:
				CreateHexGrid();
				break;
			default:
				CreateSquareGrid();
				break;
		}

		if(withUpdate)
			UpdateAllConnections();
	}

	public Cell GetCellByName( string name )
	{
		GameObject cellObj = GameObject.Find(name);
		return (Cell)cellObj.transform.GetComponent<Cell>();
	}

	public Cell GetClosestCellToMouse()
	{
		return m_closestToMouse;
	}

	public Cell GetClosestCell( Vector3 position )
	{
		Cell closestCell = null;
		float closestCellDistance = m_GridSpacing;

		foreach( GameObject cell in m_cells )
		{
			float tempdist = Vector3.Distance( position, cell.transform.position );
			if ( tempdist < closestCellDistance )
			{
				Cell tempCell = cell.transform.GetComponent<Cell>();
				if( tempCell != null )
				{
					closestCell = tempCell;
					closestCellDistance = tempdist;
				}
			}
		}

		return closestCell;
	}

	private void CreateSquareGrid()
	{
		for(int x = 0; x < m_HorizontalCells; x++ )
		{
			for( int y = 0; y < m_VerticalCells; y++ )
			{
				string cellName = string.Format( "Cell({0},{1})", x, y );
				GameObject newCell = (GameObject)Instantiate( m_CellObject, m_origin + new Vector3((x * 1.0f) * m_GridSpacing, (y * 1.0f) * m_GridSpacing ), transform.rotation );

				newCell.name = cellName;
				newCell.transform.parent = transform;
				m_cells.Add( newCell );

				Cell cellObj = (Cell)newCell.transform.GetComponent<Cell>();
				cellObj.GridCoords = new int[] {x,y};
			}
		}
	}
	
	private void UpdateClosestCellToMouse()
	{
		Vector3 mousePos = Input.mousePosition;
		mousePos.z = 50.0f;
		mousePos = Camera.main.ScreenToWorldPoint(mousePos);
		
		m_closestToMouse = GetClosestCell( mousePos );
	}

	private float CalculateHexMod ( float spacing )
	{
		float rtrn = m_HexYModifier;

		//rtrn = Mathf.Sqrt( Mathf.Pow( m_GridSpacing/2f, 2f ) / 0.75f );
		//rtrn = Mathf

		return rtrn;
	}

	private void CreateHexGrid()
	{
		float m_hexMod = CalculateHexMod( m_GridSpacing );

		for (int y = 0; y < m_VerticalCells; y++ )
		{
			float m_modifier = 0f;

			if( y != 0 && y%2 != 0 )
			{
				m_modifier = m_GridSpacing/2f;
			}

			for (int x = 0; x < m_HorizontalCells; x++ )
			{
				//Debug.Log( m_GridSpacing * Mathf.Sin(60f) );

				Vector3 newPosition = m_origin + new Vector3( ((x * 1.0f) * m_GridSpacing) + m_modifier, (y * 1.0f) * m_hexMod, 0f );

				string cellName = string.Format( "Cell({0},{1})", x, y );
				GameObject newCell = (GameObject)Instantiate( m_CellObject, newPosition, transform.rotation );
				
				newCell.name = cellName;
				newCell.transform.parent = transform;
				m_cells.Add( newCell );

				Cell cellObj = (Cell)newCell.transform.GetComponent<Cell>();
				cellObj.GridCoords = new int[] { x, y };
			}
		}
	}
}
