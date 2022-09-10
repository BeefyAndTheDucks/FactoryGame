using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuildManager : MonoBehaviour
{
    public GameObject buildMenuGameObject;

    public BuildMenu buildMenu;

    public int canBeBuiltOnLayer;

    FirstPersonController character;
    Buildable[] buildables;

    public static bool buildMode;
    bool buildMenuOpen;
    Vector3 buildPoint;
    GameObject lookingAt;
    [HideInInspector]
    public Buildable selected;
    GameObject previewGameObject;

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

    void Start()
    {
        character = gameObject.GetComponent<PlayerHealthManager>().character;
        buildables = buildMenu.GetBuildables();
    }

    void OnValidate()
    {
        buildMenu.buildManager = this;
    }

    void Update()
    {
        if (buildMode)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, canBeBuiltOnLayer)) // If it succeded getting object
            {
                buildPoint = hit.point;
                lookingAt = hit.transform.gameObject;
                Debug.DrawLine(ray.origin, hit.point, Color.blue);
                Preview();
            }
        }
        if (Input.GetButtonDown("build"))
        {
            if (!buildMode)
            {
                ToggleBuildMenu();
            } else
            {
                buildMode = false;
            }
        }
        if (Input.GetButtonDown("Fire1"))
        {
            if (buildMode)
            {
                Build();
            }
        }
    }

    void ToggleBuildMenu()
    {
        buildMenuOpen = !buildMenuOpen;
        buildMenuGameObject.SetActive(buildMenuOpen);

        Cursor.lockState = (buildMenuOpen) ? CursorLockMode.None : CursorLockMode.Locked;
        character.playerCanMove = !buildMenuOpen;
        character.cameraCanMove = !buildMenuOpen;
        character.enableHeadBob = !buildMenuOpen;

        Debug.Log("Toggle Build menu to: " + buildMenuOpen);
    }

    void Build()
    {
        // Build Logic
    }

    void Preview()
    {
        if (previewGameObject != null)
            Destroy(previewGameObject);
        previewGameObject = Instantiate(selected.previewPrefab, buildPoint, Quaternion.identity);
    }
    
    public void SelectBuildable(string name)
    {
        Debug.Log("Selecting Buildable");
        Buildable tmp = GetBuildableWithName(name);
        if (tmp != null)
        {
            ToggleBuildMenu();
            Debug.Log("Found buildable!");
            selected = tmp;
            buildMode = true;
        } else
        {
            Debug.LogError("Buildable \"" + name + "\" does not exist.");
        }
    }
}
