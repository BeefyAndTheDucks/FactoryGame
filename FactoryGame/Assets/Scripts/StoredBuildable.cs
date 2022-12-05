using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoredBuildable
{
    public string UUID;

    public float[] rotation;
    public float[] position;

    public StoredBuildable(Buildable buildable, Vector3 position, Quaternion rotation)
	{
        UUID = buildable.UUID;

        this.position = new float[3] { position.x, position.y, position.z };
        this.rotation = new float[4] { rotation.x, rotation.y, rotation.z, rotation.w };
	}
}
