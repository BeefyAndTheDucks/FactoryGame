using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;

public class BuildMenu : MonoBehaviour
{

    public BuildMenuCategory[] categories;
    public GameObject categoryPrefab;
    public GameObject buildablePrefab;
    public Transform categoryContentTransform;
    public Transform buildableContentTransform;

    [HideInInspector]
    public PlayerBuildManager buildManager;

    void InitializeInstantiated(GameObject instantiated, string name, Sprite icon)
    {
        instantiated.name = name;
        instantiated.GetComponent<UnityEngine.UI.Image>().sprite = icon;
        instantiated.GetComponentInChildren<TextMeshProUGUI>().text = name;
    }

    void Start()
    {
        foreach (BuildMenuCategory category in categories)
        {
            GameObject instantiated = Instantiate(categoryPrefab, categoryContentTransform);
            InitializeInstantiated(instantiated, category.name, category.icon);
            instantiated.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { OnCategoryButtonClick(instantiated.name); });
        }

        // Start by selecting first category
        OnCategoryButtonClick(categories[0].name);
    }

    BuildMenuCategory GetCategoryWithName(string name)
    {
        foreach (BuildMenuCategory category in categories)
        {
            if (category.name == name)
            {
                return category;
            }
        }

        return null;
    }

    void OnCategoryButtonClick(string name)
    {

        BuildMenuCategory category = GetCategoryWithName(name);

        if (category != null)
        {
            foreach (Transform child in buildableContentTransform)
            {
                Destroy(child.gameObject);
            }

            foreach (Buildable buildable in category.buildables)
            {
                GameObject instantiated = Instantiate(buildablePrefab, buildableContentTransform);
                InitializeInstantiated(instantiated, buildable.name, buildable.icon);
                instantiated.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { OnBuildableButtonClick(buildable.name); });
            }
        } else
        {
            Debug.LogError("No category with name \"" + name + "\"!");
        }

    }

    void OnBuildableButtonClick(string name)
    {
        buildManager.SelectBuildable(name);
    }

    public Buildable[] GetBuildables()
    {
        List<Buildable> buildables = new List<Buildable>();

        foreach (BuildMenuCategory category in categories)
        {
            foreach (Buildable buildable in category.buildables)
            {
                buildables.Add(buildable);
            }
        }

        return buildables.ToArray();
    }

}
