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
			Vector3 position = builder.testPosition;

			if (builder.lastBuilt != null)
				position.y = (builder.lastBuilt.transform.position.y + 10);

			float testMultiplier = 10;

			position += new Vector3(Random.Range(-10, 10) / 10f * testMultiplier, 0, Random.Range(-10, 10) / 10f * testMultiplier);

			BuildProperties testBuildProperties = new BuildProperties(builder.testRotation, position, builder.testBuildable);
			builder.Build(testBuildProperties);
		}

		if (GUILayout.Button("Test Deconstruct"))
		{
			builder.Deconstruct(builder.lastBuilt);
		}

		if (GUILayout.Button("Test Save"))
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
