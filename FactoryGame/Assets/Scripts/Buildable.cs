using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// You cannot store this with a binary formatter!
/// </summary>
[System.Serializable]
public class Buildable
{
    [Header("Info")]
    public string name = "New Buildable";
    public Category Category;
    public GameObject prefab;
    public GameObject previewPrefab;
    public Vector3 BuildOffset;
    public Quaternion BaseRotation;
    public Sprite Icon;

    [Header("UUID Stuff")]
    public string UUID = "";

    // Other properties

    static string UUIDChars = "1234567890abcdefghijklmnopqrstuvwxyz";
    static int UUIDLength = 16;

    public void GenerateUUID()
	{
        Builder.usedBuildableUUIDs.Remove(UUID);

        char[] UUIDCharArray = new char[UUIDLength];

        while (true)
		{
			for (int i = 0; i < UUIDLength; i++)
			{
                UUIDCharArray[i] = UUIDChars[Random.Range(0, UUIDChars.Length - 1)];
			}

            UUID = new string(UUIDCharArray);

            if (!Builder.usedBuildableUUIDs.Contains(UUIDCharArray.ToString()))
			{
                Builder.usedBuildableUUIDs.Add(UUID);
                break;
			}
		}
	}

    public Buildable()
	{
        UUIDChars = "1234567890abcdefghijklmnopqrstuvwxyz";
        UUID = "";

        UUIDLength = 32;
    }
}

