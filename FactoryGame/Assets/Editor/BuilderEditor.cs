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
		
		if (GUILayout.Button("Save"))
		{
			builder.TestSaving();
		}

		if (GUILayout.Button("Test Load"))
		{
			builder.TestLoading();
		}

		if (GUILayout.Button("Clear Level"))
		{
			builder.ClearLevel();
		}

		if (GUILayout.Button("Regenerate All UUIDs (May break saves)"))
		{
			builder.RegenerateAllUUIDs();
		}

		if (GUILayout.Button("Generate missing UUIDs"))
		{
			builder.GenerateMissingUUIDs();
		}
	}
}
