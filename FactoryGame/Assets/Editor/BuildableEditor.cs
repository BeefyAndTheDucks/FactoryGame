using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Buildable))]
public class BuildableEditor : Editor
{
	public override void OnInspectorGUI()
	{
		Buildable buildable = (Buildable)target;

		base.OnInspectorGUI();

		if (GUILayout.Button("Generate UUID"))
		{
			buildable.GenerateUUID();
		}
	}
}
