using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuildManager : MonoBehaviour
{
    public GameObject buildMenuGameObject;
    public BuildMenu buildMenu;
    public LayerMask canBeBuiltOnLayer;
    public float deconstructTime = 5f;
    public Material previewMaterial;
    public Material deconstructMaterial;
    public RectTransform deconstructTransform;
    public GameObject deconstructParent;
    public string buildableTag;
    public Vector3 gridSize = Vector3.one;
    public Transform buildableParentTransform;
    public float reach = 10.0f;
    public bool deconstructBarReversed;
    public Vector3Int buildablesGridSize = Vector3Int.one * 20;

    [SerializeField]
    public static Color red = new Color32(255, 0, 0, 123);
    [SerializeField]
    public static Color green = new Color32(0, 255, 0, 123);
    public static bool canPlace = true;

    [HideInInspector]
    public static bool buildMode;
    [HideInInspector]
    public Buildable selected;
    [HideInInspector]
    public GameObject[,,] grid;

    bool buildMenuOpen;
    bool deconstructMode;
    Vector3 buildPoint;
    GameObject lookingAt;
    GameObject previewGameObject;
    FirstPersonController character;
    Buildable[] buildables;
    Material oldMaterial;
    float deconstructFrame;

    public Buildable GetBuildableWithName(string name)
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
        deconstructFrame = 1f;
        previewMaterial.color = green;
        character = gameObject.GetComponent<PlayerHealthManager>().character;
        buildables = buildMenu.GetBuildables();
        grid = new GameObject[buildablesGridSize.x, buildablesGridSize.y, buildablesGridSize.z];
    }

    void OnValidate()
    {
        buildMenu.buildManager = this;
    }

    void Update()
    {
        if (lookingAt != null && deconstructMode)
            ChangeAllMaterialsInGO(lookingAt, oldMaterial);
        if (Input.GetButtonDown("build"))
        {
            if (!buildMode)
            {
                if (deconstructMode)
                {
                    deconstructMode = false;
                    if (lookingAt != null && oldMaterial != null)
                        ChangeAllMaterialsInGO(lookingAt, oldMaterial);
                }
                ToggleBuildMenu();
            } else
            {
                buildMode = false;
                deconstructMode = false;
                if (previewGameObject != null)
                    Destroy(previewGameObject);
                if (lookingAt != null && oldMaterial != null)
                    ChangeAllMaterialsInGO(lookingAt, oldMaterial);
            }
        }
        if (Input.GetButton("Fire1"))
        {
            if (buildMode && Input.GetButtonDown("Fire1"))
            {
                Build();
            } else if (deconstructMode && lookingAt != null)
            {
                
                if (deconstructFrame >= 1)
                {
                    Deconstruct();
                    deconstructFrame = 0f;
                    deconstructParent.SetActive(false);
                }
                else
                {
                    deconstructParent.SetActive(true);

                    deconstructFrame = Mathf.Clamp01(deconstructFrame + Time.deltaTime * deconstructTime);
                    if (deconstructBarReversed)
                        deconstructTransform.offsetMax = new Vector2(-Mathf.Lerp(220, 0, deconstructFrame), deconstructTransform.offsetMax.y);
                    else
                        deconstructTransform.offsetMax = new Vector2(-Mathf.Lerp(0, 220, deconstructFrame), deconstructTransform.offsetMax.y);
                }
                
            }
        } else
        {
            deconstructFrame = 0f;
            deconstructParent.SetActive(false);
        }
        if (Input.GetButtonDown("Cancel") && buildMode)
        {
            buildMode = false;
            deconstructMode = false;
            if (previewGameObject != null)
                Destroy(previewGameObject);
            if (lookingAt != null && oldMaterial != null)
                ChangeAllMaterialsInGO(lookingAt, oldMaterial);
        }
        if (Input.GetButtonDown("deconstruct"))
        {
            buildMode = false;
            deconstructMode = true;
        }
        if (buildMode || deconstructMode)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, reach, canBeBuiltOnLayer)) // If it succeded getting object
            {
                buildPoint = new Vector3(Mathf.Round(hit.point.x / gridSize.x) * gridSize.x, Mathf.Round(hit.point.y / gridSize.y) * gridSize.y, Mathf.Round(hit.point.z / gridSize.z) * gridSize.z);
                if (selected != null)
                    buildPoint += selected.buildOffset;
                if (lookingAt != null)
                    if (lookingAt != hit.transform.gameObject && deconstructMode)
                        ChangeAllMaterialsInGO(lookingAt, oldMaterial);
                if (hit.transform.gameObject.CompareTag(buildableTag))
                {
                    lookingAt = hit.transform.gameObject;
                } else
                {
                    lookingAt = null;
                }
                Debug.DrawLine(ray.origin, hit.point, Color.blue);
                if (buildMode)
                {
                    Preview();
                } else
                {
                    if (previewGameObject != null)
                        Destroy(previewGameObject);
                    if (lookingAt != null)
                    {
                        oldMaterial = lookingAt.GetComponentInChildren<Renderer>().material;
                        ChangeAllMaterialsInGO(lookingAt, deconstructMaterial);
                    }
                }

                if (Input.GetButtonDown("pick") && lookingAt.GetComponent<BuiltBuildable>() != null)
                {
                    buildMode = true;
                    deconstructMode = false;
                    selected = lookingAt.GetComponent<BuiltBuildable>().buildable;
                    if (lookingAt != null && oldMaterial != null)
                        ChangeAllMaterialsInGO(lookingAt, oldMaterial);
                }

            } else
            {
                lookingAt = null;
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
    }

    void Build()
    {
        // Build Logic
        if (canPlace)
        {
            Vector3Int gridPosition = new Vector3Int(
                Mathf.RoundToInt(buildPoint.x / gridSize.x + buildablesGridSize.x / 2),
                Mathf.RoundToInt(buildPoint.y / gridSize.y + buildablesGridSize.y / 2), 
                Mathf.RoundToInt(buildPoint.z / gridSize.z + buildablesGridSize.z / 2));
            grid[gridPosition.x, gridPosition.y, gridPosition.z] = Instantiate(selected.buildPrefab, buildPoint, Quaternion.identity, buildableParentTransform);
            grid[gridPosition.x, gridPosition.y, gridPosition.z].GetComponent<BuiltBuildable>().gridIndicies = gridPosition;
            if (grid[gridPosition.x, gridPosition.y, gridPosition.z].GetComponent<Belt>() != null)
                grid[gridPosition.x, gridPosition.y, gridPosition.z].GetComponent<Belt>().grid = grid;
            Destroy(previewGameObject);
        }
    }

    void Deconstruct()
    {
        if (lookingAt != null)
        {
            DestroyImmediate(lookingAt);
        }
    }

    void Preview()
    {
        if (previewGameObject == null)
            previewGameObject = Instantiate(selected.previewPrefab, buildPoint, Quaternion.identity);

        previewGameObject.transform.position = buildPoint;
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

    void ChangeAllMaterialsInGO(GameObject go, Material material)
	{
        Transform transform = go.transform;
        Renderer renderer = go.GetComponent<Renderer>();

        if (renderer != null)
		{
            renderer.material = material;
		}

        if (transform.childCount > 0)
		{
            foreach (Transform child in transform)
            {
                renderer = child.GetComponent<Renderer>();

                if (renderer != null)
                    renderer.material = material;
            }
        }
	}
}
