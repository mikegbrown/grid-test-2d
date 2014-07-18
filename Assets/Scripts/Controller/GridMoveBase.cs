using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GridMoveBase : MonoBehaviour 
{
	public float					m_MoveSpeed = 0.5f;
	public float					m_Gravity = 0.25f;
	public float					m_HopSpeed = 0.6f;
	
	protected bool					m_move;
	protected bool					m_enabled = true;
	protected bool					m_isTurnBased;

	protected GridSelector 			m_gridSelector;
	protected Cell					m_currentCell;

	protected List<Cell>			m_currentPath;

	protected float					m_currYOffset;
	protected Vector3 				m_currPos;
	
	protected void Initialize()
	{
		m_gridSelector = (GridSelector)transform.GetComponent<GridSelector>();
		m_currentCell = m_gridSelector.GetClosestCell( transform.position );
		if( m_currentCell == null )
		{
			Debug.LogError("Grid Movement Controller is not close enough to grid. Teleporting to grid cell (0,0)");
			m_currentCell = m_gridSelector.GetCellZero();
		}
		
		transform.position = m_currentCell.transform.position;
		
		//Register for Events
		Events.instance.AddListener<SetTurnBasedEvent>( SetTurnBased );
	}

	// Update is called once per frame
	public virtual void UpdateMovement()
	{
		if( m_enabled )
		{	
			if( m_move )
			{
				float dist = Vector3.Distance( m_currPos, m_currentCell.transform.position );
				if( dist < 0.1f )
				{
					transform.position = m_currentCell.transform.position;
					m_move = false;
					
					if( m_isTurnBased ) 
					{
						TurnCompletedEvent turnEvent = new TurnCompletedEvent( gameObject );
						Events.instance.Raise( turnEvent );
						m_enabled = false;
					}
				}
				else
				{
					Vector3 newPosition = Vector3.Lerp( transform.position, m_currentCell.transform.position,  m_MoveSpeed );
					m_currPos = newPosition;

					//apply gravity and hopspeed
					newPosition.y += m_currYOffset;
					m_currYOffset = Mathf.Clamp( m_currYOffset - m_Gravity, 0f, m_HopSpeed );

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

						m_currYOffset = m_HopSpeed;

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
	
	public void SetTurnBased( SetTurnBasedEvent e )
	{
		m_isTurnBased = true;
	}
}
