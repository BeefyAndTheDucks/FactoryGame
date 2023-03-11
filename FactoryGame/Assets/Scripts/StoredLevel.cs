using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class StoredLevel
{
    List<StoredBuildable> buildables;

	public string fileName;
	public SavePlaytime playtime;
	public SaveLastplayed lastplayed;
    public StoredLevel(string fileName, bool shouldBeEmpty = false)
	{
		buildables = new List<StoredBuildable>();

		playtime = new(0, 0, 0);
		var oldLastPlayed = lastplayed;
		lastplayed = new(DateTime.Today, true);

		if (oldLastPlayed.valid)
		{
			var diff = lastplayed.GetDateTime() - oldLastPlayed.GetDateTime();
			playtime = new(diff.Hours, diff.Minutes, diff.Seconds);
		}
		this.fileName = fileName;

		if (shouldBeEmpty)
			return;

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

[Serializable]
public struct SavePlaytime
{
	public int hours;
	public int minutes;
	public int seconds;

	public SavePlaytime(int hours, int minutes, int seconds)
	{
		this.hours = hours;
		this.minutes = minutes;
		this.seconds = seconds;
	}
}

[Serializable]
public struct SaveLastplayed
{
	public int year;
	public int month;
	public int day;
	public bool valid;

	private DateTime time;

	public SaveLastplayed(DateTime now, bool valid = false)
	{
		this.valid = valid;
		year = now.Year;
		month = now.Month;
		day = now.Day;
		time = now;
	}

	public DateTime GetDateTime() => time;
}
