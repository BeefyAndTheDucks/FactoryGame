using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenu : MonoBehaviour
{

    public BuildMenuCategory[] categories;
    public GameObject categoryPrefab;
    public Transform contentTransform;

    void Start()
    {
        foreach (BuildMenuCategory category in categories)
        {
            GameObject instantiated = Instantiate(categoryPrefab, contentTransform);
            instantiated.name = category.name;
            instantiated.GetComponent<Image>().sprite = category.icon;
            category.button = instantiated.GetComponent<Button>();
        }
    }

}
