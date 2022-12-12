using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Transform HeartsParent;
    public GameObject GameOverGameObject;

    private void Awake()
    {
        AssignSingleton();
    }

    private void AssignSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }
        
        Debug.LogError("More than one \"" + name + "\" in scene!");
    }
}
