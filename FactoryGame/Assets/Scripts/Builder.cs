using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    [Header("Test Stuff")]
    public Buildable testBuildable;
    public Vector3 testPosition;
    public Quaternion testRotation;

    [Header("Main")]
    public Material buildEffectMaterial;
    public Material deconstructEffectMaterial;
    public float buildEffectSpeed = 1f;
    public float deconstructEffectSpeed = 1f;

    [HideInInspector] public GameObject lastBuilt;

    public void Build(BuildProperties properties)
	{
        // Instantiate and get old material
        lastBuilt = Instantiate(properties.buildable.prefab);
        Renderer renderer = lastBuilt.GetComponent<Renderer>();
        Material oldMaterial = renderer.sharedMaterial;

        // Relocate object
        lastBuilt.transform.rotation = properties.rotation;
        lastBuilt.transform.position = properties.position;

        // Assign dissolving properties
        assignProperties(oldMaterial, buildEffectMaterial);
        buildEffectMaterial.SetFloat("_Dissolve", 1f);

        // Start dissolving
        renderer.material = buildEffectMaterial;
        StartCoroutine(buidEffect(oldMaterial, renderer));
	}

    IEnumerator buidEffect(Material oldMaterial, Renderer renderer)
	{
        float dissolve = 1f;

        while (dissolve > 0)
		{
            dissolve -= Time.deltaTime * buildEffectSpeed;
            dissolve = Mathf.Clamp01(dissolve);
            buildEffectMaterial.SetFloat("_Dissolve", dissolve);
            yield return null;
		}

        renderer.sharedMaterial = oldMaterial;
	}

    public void Deconstruct(GameObject obj)
    {
        if (obj == null)
            return;

        // Get old material
        Renderer renderer = obj.GetComponent<Renderer>();
        Material oldMaterial = renderer.sharedMaterial;

        // Assign dissolving properties
        assignProperties(oldMaterial, deconstructEffectMaterial);
        deconstructEffectMaterial.SetFloat("_Dissolve", 0f);

        // Start dissolving
        renderer.material = deconstructEffectMaterial;
        StartCoroutine(deconstructEffect(obj));
    }

    void assignProperties(Material copyFrom, Material copyTo)
	{
        copyTo.SetFloat("_Smoothness", copyFrom.GetFloat("_Smoothness"));
        copyTo.SetFloat("_Metallicness", copyFrom.GetFloat("_Metallic"));
        copyTo.SetColor("_Color", copyFrom.GetColor("_Color"));
        copyTo.SetTexture("_Texture", copyFrom.GetTexture("_MainTex"));
        copyTo.SetTexture("_Ambient_Occlusion", copyFrom.GetTexture("_OcclusionMap"));
        copyTo.SetTexture("_Emission_Map", copyFrom.GetTexture("_EmissionMap"));
        copyTo.SetTexture("_Metallic_Map", copyFrom.GetTexture("_MetallicGlossMap"));
    }

    IEnumerator deconstructEffect(GameObject obj)
    {
        float dissolve = 0f;

        while (dissolve < 1)
        {
            dissolve += Time.deltaTime * buildEffectSpeed;
            dissolve = Mathf.Clamp01(dissolve);
            deconstructEffectMaterial.SetFloat("_Dissolve", dissolve);
            yield return null;
        }

        if (Application.isEditor)
            DestroyImmediate(obj);
        else
            Destroy(obj);
    }

}

public struct BuildProperties
{
    public Quaternion rotation;
    public Vector3 position;
    public Buildable buildable;

    public BuildProperties(Quaternion rotation, Vector3 position, Buildable buildable)
	{
        this.rotation = rotation;
        this.position = position;
        this.buildable = buildable;
    }
}
