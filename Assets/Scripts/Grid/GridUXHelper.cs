using UnityEngine;
using System.Collections;

[RequireComponent (typeof (GridSelector))]
public class GridUXHelper : MonoBehaviour 
{

	public Color					m_SelectedColor = Color.black;
	public float					m_SelectedHighlightTime = 0.1f;
	public Color					m_HoverColor = Color.gray;
	public float					m_HoverHighlightTime = 0.02f;

	private GridSelector 			m_gridSelector;

	// Use this for initialization
	void Start () 
	{
		m_gridSelector = (GridSelector)transform.GetComponent<GridSelector>();

		m_gridSelector.cellSelectedDelegate = CellSelected;
		m_gridSelector.cellHoverDelegate = HoverCell;
	}

	private void HoverCell( Cell hoverCell )
	{
		hoverCell.HighlightCell( m_HoverColor, m_HoverHighlightTime, Cell.HighlightPriority.Low );
	}

	private void CellSelected( Cell selectedCell )
	{
		selectedCell.HighlightCell( m_SelectedColor, m_SelectedHighlightTime, Cell.HighlightPriority.High );
	}

}
