using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GridManager))]
public class GridEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		GridManager gridManager = (GridManager)target;
		if(GUILayout.Button("Create Grid"))
		{
			gridManager.CreateGrid();
		}

		if(GUILayout.Button("Create Grid with Connections"))
		{
			gridManager.CreateGrid(true);
		}

		if(GUILayout.Button("Update Connections"))
		{
			gridManager.UpdateAllConnections();
		}

		if(GUILayout.Button("Delete Grid"))
		{
			gridManager.DeleteGrid();
		}
	}
}
