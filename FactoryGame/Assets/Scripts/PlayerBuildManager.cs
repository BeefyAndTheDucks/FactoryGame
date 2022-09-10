using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuildManager : MonoBehaviour
{
    public GameObject buildMenuGameObject;
    public BuildMenu buildMenu;
    public int canBeBuiltOnLayer;
    public Material previewMaterial;

    [SerializeField]
    public static Color red = new Color32(255, 0, 0, 123);
    [SerializeField]
    public static Color green = new Color32(0, 255, 0, 123);

    [HideInInspector]
    public static bool buildMode;
    [HideInInspector]
    public Buildable selected;

    bool buildMenuOpen;
    Vector3 buildPoint;
    GameObject lookingAt;
    GameObject previewGameObject;
    FirstPersonController character;
    Buildable[] buildables;

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
        previewMaterial.color = green;
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
                buildPoint = hit.point + selected.buildOffset;
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
                if (previewGameObject != null)
                    Destroy(previewGameObject);
            }
        }
        if (Input.GetButtonDown("Fire1"))
        {
            if (buildMode)
            {
                Build();
            }
        }
        if (Input.GetButtonDown("Cancel") && buildMode)
        {
            buildMode = false;
            if (previewGameObject != null)
                Destroy(previewGameObject);
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
        Instantiate(selected.buildPrefab, buildPoint, Quaternion.identity);
    }

    void Preview()
    {
        if (previewGameObject != null)
            Destroy(previewGameObject);
        previewGameObject = Instantiate(selected.previewPrefab, buildPoint, Quaternion.identity);
    }
    
    public void SelectBuildable(string name)
    {
        Buildable tmp = GetBuildableWithName(name);
        if (tmp != null)
        {
            ToggleBuildMenu();
            selected = tmp;
            buildMode = true;
        } else
        {
            Debug.LogError("Buildable \"" + name + "\" does not exist.");
        }
    }
}
