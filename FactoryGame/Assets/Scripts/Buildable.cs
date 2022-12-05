using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Buildable : ScriptableObject
{
    [Header("UUID Stuff")]
    public string UUID = "0000000000000000";
    public string UUIDChars = "1234567890abcdefghijklmnopqrstuvwxyz";

    public int UUIDLength = 32;

    [Header("Others")]
    public GameObject prefab;
    // Other properties

	public void GenerateUUID()
	{
        Builder.usedUUIDs.Remove(UUID);

        char[] UUIDCharArray = new char[UUIDLength];

        while (true)
		{
			for (int i = 0; i < UUIDLength; i++)
			{
                UUIDCharArray[i] = UUIDChars[Random.Range(0, UUIDChars.Length - 1)];
			}

            UUID = new string(UUIDCharArray);

            if (!Builder.usedUUIDs.Contains(UUIDCharArray.ToString()))
			{
                Builder.usedUUIDs.Add(UUID);
                break;
			}
		}
	}
}

