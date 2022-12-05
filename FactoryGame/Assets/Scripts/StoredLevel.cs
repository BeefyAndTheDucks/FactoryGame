using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class StoredLevel
{
    List<StoredBuildable> buildables;

    public StoredLevel()
	{
		buildables = new List<StoredBuildable>();

		foreach (Transform child in Builder.instance.buildableParent)
		{
			Buildable buildable = child.GetComponent<BuiltBuildable>().buildable;
			StoredBuildable storedBuildable = new StoredBuildable(buildable, child.position, child.rotation);

			buildables.Add(storedBuildable);
		}
	}

	public void ToLevel()
	{
		Debug.Log("Converting to level...");

		Builder.instance.ClearLevel();

		foreach (StoredBuildable storedBuildable in buildables)
		{
			Buildable buildable = Builder.instance.GetBuildableByUUID(storedBuildable.UUID);
			if (buildable == null)
				continue;

			Quaternion rotation = new Quaternion(storedBuildable.rotation[0], storedBuildable.rotation[1], storedBuildable.rotation[2], storedBuildable.rotation[3]);
			Vector3 position = new Vector3(storedBuildable.position[0], storedBuildable.position[1], storedBuildable.position[2]);

			BuildProperties properties = new BuildProperties(rotation, position, buildable);

			Builder.instance.Build(properties, true);

			Debug.Log("Added new object.");
		}
	}
}
