using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuildManager : MonoBehaviour
{

    public Buildable[] buildables;

    public GameObject buildMenuGameObject;

    bool buildMode;
    bool buildMenuOpen;
    Vector3 buildPoint;
    Buildable selected;

    Buildable GetBuildableWithName(string name)
    {
        foreach (Buildable buildable in buildables)
        {
            if (buildable.name == name)
            {
                return buildable;
            }
        }

        return null;
    }

    void Update()
    {
        if (buildMode)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                buildPoint = hit.point;
                Debug.DrawLine(ray.origin, hit.point, Color.blue);
            }
        }
        if (Input.GetButtonDown("build"))
        {
            buildMenuOpen = !buildMenuOpen;
            buildMenuGameObject.SetActive(buildMenuOpen);
        }
    }
    
    public void SelectItem(string name)
    {
        Buildable tmp = GetBuildableWithName(name);
        if (tmp != null)
        {
            selected = tmp;
        } else
        {
            Debug.LogError("Buildable \"" + name + "\" does not exist.");
        }
    }
}
