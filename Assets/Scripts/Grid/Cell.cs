using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Cell : MonoBehaviour 
{

	public enum HighlightPriority
	{
		None,
		Low, 
		High
	}

	public enum CellType
	{
		Normal,
		Blocking,
		Deadly
	}

	public CellType					m_CellType = CellType.Normal;
	public Color					m_ConnectionColor = Color.green;
	public Color					m_UnpassableColor = Color.red;
	public List<GameObject>			m_connectedCells;
	public int						m_GridX;
	public int						m_GridY;

	private SpriteRenderer 			m_spriteRenderer;
	private bool					m_isHighlighted;
	private HighlightPriority		m_currentHighlightPriority;
	private float					m_highlightTime;
	private Color					m_defaultColor;

	private Cell					m_pathfindParent;

	private float					m_pathfindGScore;
	private float					m_pathfindHScore;

	// Use this for initialization
	void Start () 
	{
		m_spriteRenderer = (SpriteRenderer)transform.GetComponent<SpriteRenderer>();
		m_defaultColor = m_spriteRenderer.color;

		m_currentHighlightPriority = HighlightPriority.None;
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( m_isHighlighted )
		{
			m_highlightTime -= Time.deltaTime;
			if( m_highlightTime <= 0.0f )
			{
				ResetColor();
			}
		}
	}

	void OnDrawGizmos()
	{
		foreach( GameObject cellObj in m_connectedCells )
		{
			Cell cell = (Cell)cellObj.transform.GetComponent<Cell>();
			if( m_CellType == CellType.Blocking || cell.m_CellType == CellType.Blocking )
			{
				Gizmos.color = m_UnpassableColor;
				Gizmos.DrawLine(transform.position, cellObj.transform.position);
			}
			else
			{
				Gizmos.color = m_ConnectionColor;
				Gizmos.DrawLine(transform.position, cellObj.transform.position);
			}
		}
	}

	public void HighlightCell( Color color, float time, HighlightPriority priority )
	{
		if( priority >= m_currentHighlightPriority )
		{
			m_spriteRenderer.color = color;
			m_highlightTime = time;
			m_isHighlighted = true;
			m_currentHighlightPriority = priority;
		}
	}

	public bool IsConnectedToCell( Cell testCell )
	{
		bool rtrn = false;

		foreach( GameObject cellObj in m_connectedCells )
		{
			Cell compareCell = (Cell)cellObj.transform.GetComponent<Cell>();
			if( compareCell == testCell )
			{
				rtrn = true;
			}
		}
		return rtrn;
	}

	public void UpdateConnectedCells( float distance )
	{
		if( m_connectedCells != null )
			m_connectedCells = new List<GameObject>();

		m_connectedCells.Clear();

		Object[] allObjects = FindObjectsOfType( typeof(Cell) );
		foreach( Object obj in allObjects )
		{
			Cell cell = (Cell)obj;
			GameObject gObj = cell.gameObject;

			float dist = ( transform.position - gObj.transform.position ).magnitude;

			if ( dist <= distance || Mathf.Approximately(dist, distance ) )
			{
				if(gObj != gameObject)
				{
					m_connectedCells.Add( gObj );
				}
			}

		}
	}

	public void RemoveConnectedCell( GameObject gameObj )
	{
		m_connectedCells.Remove( gameObj );
	}

	public void OnDestroy() 
	{
		foreach( GameObject cellObj in m_connectedCells )
		{
			Cell cell = cellObj.GetComponent<Cell>();
			cell.RemoveConnectedCell( gameObject );
		}
	}
	
	public int[] GridCoords
	{
		get { return new int[] {m_GridX, m_GridY }; }
		set { m_GridX = value[0]; m_GridY = value[1]; }
	}

	public List<GameObject> AdjacentCells
	{
		get { return m_connectedCells; }
	}

	public Cell PathfindParent
	{
		get { return m_pathfindParent; }
		set { m_pathfindParent = value; }
	}

	public float GScore
	{
		get { return m_pathfindGScore; }
		set { m_pathfindGScore = value; }
	}

	public float HScore
	{
		get { return m_pathfindHScore; }
		set { m_pathfindHScore = value; }
	}

	public float FScore
	{
		get { return m_pathfindGScore + m_pathfindHScore; }
	}

	private void ResetColor()
	{
		m_isHighlighted = false;
		m_spriteRenderer.color = m_defaultColor;
		m_currentHighlightPriority = HighlightPriority.None;
	}
}

public class CellCompare : IComparer<Cell>
{
	// Compares cells by FScore. 
	public int Compare(Cell x, Cell y)
	{
		return x.FScore.CompareTo(y.FScore);
	}
}
