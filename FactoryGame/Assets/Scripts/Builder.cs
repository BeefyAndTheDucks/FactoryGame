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
	public Transform buildableParent;
	public Buildable[] buildables;

	[HideInInspector] public GameObject lastBuilt;
	public static List<string> usedUUIDs = new List<string>();

	public static Builder instance;

	void Awake()
	{
		AssignSingleton();
	}

	void AssignSingleton()
	{
		if (instance != null)
		{
			Debug.LogError("More than one Builder script in scene.");
			return;
		}

		instance = this;
	}

	public void Build(BuildProperties properties, bool isLoading = false)
	{
		// Instantiate
		lastBuilt = Instantiate(properties.buildable.prefab, buildableParent);

		// Relocate object
		lastBuilt.transform.rotation = properties.rotation;
		lastBuilt.transform.position = properties.position;

		if (isLoading)
			return;

		// Get renderer and old material
		Renderer renderer = lastBuilt.GetComponent<Renderer>();
		Material oldMaterial = renderer.sharedMaterial;

		// Assign dissolving properties
		assignProperties(oldMaterial, buildEffectMaterial);
		buildEffectMaterial.SetFloat("_Dissolve", 1f);

		// Start dissolving
		renderer.material = buildEffectMaterial;
		StartCoroutine(buidEffect(oldMaterial, renderer));
	}

	public Buildable GetBuildableByUUID(string UUID)
	{
		foreach (Buildable buildable in buildables)
		{
			if (buildable.UUID.Equals(UUID))
				return buildable;
		}

		Debug.LogError("No buildable with UUID " + UUID + "!");
		return null;
	}

	public void TestSaving()
	{
		SaveManagment.Save("hello");
	}

	public void TestLoading()
	{
		StoredLevel level = SaveManagment.Load("hello");

		if (level == null)
			return;

		level.ToLevel();
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

	public void ClearLevel()
	{
		foreach (Transform child in buildableParent)
		{
			Destroy(child.gameObject);
		}
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
