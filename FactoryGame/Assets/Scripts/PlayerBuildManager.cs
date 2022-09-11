using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

public class PlayerBuildManager : MonoBehaviour
{
    public GameObject buildMenuGameObject;
    public BuildMenu buildMenu;
    public int canBeBuiltOnLayer;
    public float deconstructTime = 5f;
    public Material previewMaterial;
    public Material deconstructMaterial;
    public RectTransform deconstructTransform;
    public GameObject deconstructParent;
    public string buildableTag;

    [SerializeField]
    public static Color red = new Color32(255, 0, 0, 123);
    [SerializeField]
    public static Color green = new Color32(0, 255, 0, 123);
    public static bool canPlace = true;

    [HideInInspector]
    public static bool buildMode;
    [HideInInspector]
    public Buildable selected;

    bool buildMenuOpen;
    bool deconstructMode;
    Vector3 buildPoint;
    GameObject lookingAt;
    GameObject previewGameObject;
    FirstPersonController character;
    Buildable[] buildables;
    Material oldMaterial;
    float deconstructFrame;

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
        deconstructFrame = 1f;
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
        if (lookingAt != null && deconstructMode)
            lookingAt.GetComponent<Renderer>().material = oldMaterial;
        if (Input.GetButtonDown("build"))
        {
            if (!buildMode)
            {
                if (deconstructMode)
                {
                    deconstructMode = false;
                    if (lookingAt != null && oldMaterial != null)
                        lookingAt.GetComponent<Renderer>().material = oldMaterial;
                }
                ToggleBuildMenu();
            } else
            {
                buildMode = false;
                deconstructMode = false;
                if (previewGameObject != null)
                    Destroy(previewGameObject);
                if (lookingAt != null && oldMaterial != null)
                    lookingAt.GetComponent<Renderer>().material = oldMaterial;
            }
        }
        if (Input.GetButton("Fire1"))
        {
            if (buildMode && Input.GetButtonDown("Fire1"))
            {
                Build();
            } else if (deconstructMode && lookingAt != null)
            {
                if (deconstructFrame <= 0)
                {
                    Deconstruct();
                    deconstructFrame = 1f;
                    deconstructParent.SetActive(false);
                } else
                {
                    deconstructParent.SetActive(true);
                    deconstructFrame -= Time.deltaTime * deconstructTime;
                    deconstructTransform.offsetMax = new Vector2(-(deconstructFrame * 220), deconstructTransform.offsetMax.y);
                }
            }
        } else
        {
            deconstructFrame = 1f;
            deconstructParent.SetActive(false);
        }
        if (Input.GetButtonDown("Cancel") && buildMode)
        {
            buildMode = false;
            deconstructMode = false;
            if (previewGameObject != null)
                Destroy(previewGameObject);
            if (lookingAt != null && oldMaterial != null)
                lookingAt.GetComponent<Renderer>().material = oldMaterial;
        }
        if (Input.GetButtonDown("deconstruct"))
        {
            buildMode = false;
            deconstructMode = true;
        }
        if (buildMode || deconstructMode)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, canBeBuiltOnLayer)) // If it succeded getting object
            {
                buildPoint = hit.point;
                if (selected != null)
                    buildPoint = hit.point + selected.buildOffset;
                if (lookingAt != null)
                    if (lookingAt != hit.transform.gameObject && deconstructMode)
                        lookingAt.GetComponent<Renderer>().material = oldMaterial;
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
                        oldMaterial = lookingAt.GetComponent<Renderer>().material;
                        lookingAt.GetComponent<Renderer>().material = deconstructMaterial;
                    }
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
            Instantiate(selected.buildPrefab, buildPoint, Quaternion.identity);
            Destroy(previewGameObject);
        }
    }

    void Deconstruct()
    {
        if (lookingAt != null)
        {
            Destroy(lookingAt);
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
}
