using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Scanner))]
public class ScannerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		Scanner scanner = (Scanner)target;

		base.OnInspectorGUI();

		if (GUILayout.Button("Animate"))
		{
			scanner.StartScan(scanner.testPosition, scanner.testMaxSize);
		}
	}
}
