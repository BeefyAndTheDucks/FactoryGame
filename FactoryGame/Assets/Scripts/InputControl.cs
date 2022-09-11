using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputControl : MonoBehaviour
{
    public event Action<Item> onItemEnter;

    private void OnTriggerEnter(Collider other)
    {
        onItemEnter(other.GetComponent<ConveyorItem>().type);
    }
}
