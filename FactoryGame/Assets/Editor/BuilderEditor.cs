using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Builder))]
public class BuilderEditor : Editor
{
    public override void OnInspectorGUI()
	{
		Builder builder = (Builder)target;

		base.OnInspectorGUI();

		if (GUILayout.Button("Test Building"))
		{
			BuildProperties testBuildProperties = new BuildProperties(builder.testRotation, builder.testPosition, builder.testBuildable);
			builder.Build(testBuildProperties);
		}

		if (GUILayout.Button("Test Deconstruct"))
		{
			builder.Deconstruct(builder.lastBuilt);
		}
	}
}
