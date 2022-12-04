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
        Material oldMaterial = renderer.material;

        // Relocate object
        lastBuilt.transform.rotation = properties.rotation;
        lastBuilt.transform.position = properties.position;

        // Assign dissolving properties
        buildEffectMaterial.SetFloat("_Smoothness", oldMaterial.GetFloat("_Smoothness"));
        buildEffectMaterial.SetFloat("_Metallicness", oldMaterial.GetFloat("_Metallic"));
        buildEffectMaterial.SetColor("_Color", oldMaterial.GetColor("_Color"));
        buildEffectMaterial.SetTexture("_Texture", oldMaterial.GetTexture("_MainTex"));
        buildEffectMaterial.SetTexture("_Ambient_Occlusion", oldMaterial.GetTexture("_OcclusionMap"));
        buildEffectMaterial.SetTexture("_Emission_Map", oldMaterial.GetTexture("_EmissionMap"));

        // Start dissolving
        renderer.material = buildEffectMaterial;
        buildEffectMaterial.SetFloat("_Dissolve", 1f);
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

        renderer.material = oldMaterial;
	}

    public void Deconstruct(GameObject obj)
    {
        if (obj == null)
            return;

        // Get old material
        Renderer renderer = obj.GetComponent<Renderer>();
        Material oldMaterial = renderer.material;

        // Assign dissolving properties
        deconstructEffectMaterial.SetFloat("_Smoothness", oldMaterial.GetFloat("_Smoothness"));
        deconstructEffectMaterial.SetFloat("_Metallicness", oldMaterial.GetFloat("_Metallic"));
        deconstructEffectMaterial.SetColor("_Color", oldMaterial.GetColor("_Color"));
        deconstructEffectMaterial.SetTexture("_Texture", oldMaterial.GetTexture("_MainTex"));
        deconstructEffectMaterial.SetTexture("_Ambient_Occlusion", oldMaterial.GetTexture("_OcclusionMap"));
        deconstructEffectMaterial.SetTexture("_Emission_Map", oldMaterial.GetTexture("_EmissionMap"));
        deconstructEffectMaterial.SetFloat("_Dissolve", 0f);

        // Start dissolving
        renderer.material = deconstructEffectMaterial;
        StartCoroutine(deconstructEffect(obj));
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
