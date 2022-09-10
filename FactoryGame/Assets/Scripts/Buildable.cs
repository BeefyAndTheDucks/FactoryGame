using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Buildable : ScriptableObject
{

    public GameObject buildPrefab;
    public GameObject previewPrefab;
    public Sprite icon;
    public Vector3 buildOffset;

}
