using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Recipe : ScriptableObject
{
    public Machine machineType;
    public Item[] inputs;
    public Item[] outputs;
    public int timeInSeconds;
}
