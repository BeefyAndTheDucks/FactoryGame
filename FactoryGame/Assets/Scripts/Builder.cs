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
	public static List<string> usedBuildableUUIDs = new();
	public static List<string> usedItemUUIDs = new();

	public static Builder instance;
	
	private static readonly int Smoothness = Shader.PropertyToID("_Smoothness");
	private static readonly int Metallicness = Shader.PropertyToID("_Metallicness");
	private static readonly int Color1 = Shader.PropertyToID("_Color");
	private static readonly int Texture1 = Shader.PropertyToID("_Texture");
	private static readonly int AmbientOcclusion = Shader.PropertyToID("_Ambient_Occlusion");
	private static readonly int EmissionMap = Shader.PropertyToID("_Emission_Map");
	private static readonly int MetallicMap = Shader.PropertyToID("_Metallic_Map");
	private static readonly int Metallic = Shader.PropertyToID("_Metallic");
	private static readonly int MainTex = Shader.PropertyToID("_MainTex");
	private static readonly int OcclusionMap = Shader.PropertyToID("_OcclusionMap");
	private static readonly int Map = Shader.PropertyToID("_EmissionMap");
	private static readonly int MetallicGlossMap = Shader.PropertyToID("_MetallicGlossMap");
	private static readonly int Dissolve = Shader.PropertyToID("_Dissolve");

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

	public void RegenerateAllUUIDs()
	{
		foreach (Buildable buildable in buildables)
		{
			buildable.GenerateUUID();
		}
	}

	public void GenerateMissingUUIDs()
	{
		foreach (Buildable buildable in buildables)
		{
			if (buildable.UUID == "")
				buildable.GenerateUUID();
		}
	}

	public void Build(BuildProperties properties, bool isLoading = false)
	{
		// Instantiate
		lastBuilt = Instantiate(properties.buildable.prefab, buildableParent);

		// Relocate object
		lastBuilt.transform.rotation = properties.rotation;
		lastBuilt.transform.position = properties.position;

		// Add "BuiltBuildable" Component and assign the buildable field
		lastBuilt.AddComponent<BuiltBuildable>().buildable = properties.buildable;

		if (isLoading)
			return;

		// Get renderer and old material
		Renderer renderer = lastBuilt.GetComponent<Renderer>();
		Material oldMaterial = renderer.sharedMaterial;

		// Assign dissolving properties
		renderer.material = buildEffectMaterial;
		AssignProperties(oldMaterial, renderer.material);
		renderer.material.SetFloat(Dissolve, 1f);

		// Start dissolving
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

		level?.ToLevel();
	}

	IEnumerator buidEffect(Material oldMaterial, Renderer renderer)
	{
		var dissolve = 1f;

		while (dissolve > 0)
		{
			dissolve -= Time.deltaTime * buildEffectSpeed;
			dissolve = Mathf.Clamp01(dissolve);
			renderer.material.SetFloat(Dissolve, dissolve);
			yield return null;
		}
		
		renderer.GetComponent<Renderer>().material = oldMaterial;
	}

	IEnumerator deconstructEffect(GameObject obj, Renderer renderer)
	{
		float dissolve = 0f;

		while (dissolve < 1)
		{
			dissolve += Time.deltaTime * deconstructEffectSpeed;
			dissolve = Mathf.Clamp01(dissolve);
			renderer.material.SetFloat("_Dissolve", dissolve);
			yield return null;
		}

		Destroy(obj);
	}

	public void Deconstruct(GameObject obj)
	{
		if (obj == null)
			return;

		// Get old material
		Renderer renderer = obj.GetComponent<Renderer>();
		Material oldMaterial = renderer.material;

		// Assign dissolving properties
		renderer.material = deconstructEffectMaterial;
		AssignProperties(oldMaterial, renderer.material);
		renderer.material.SetFloat(Dissolve, 0f);

		// Start dissolving
		StartCoroutine(deconstructEffect(obj, renderer));
	}

	private static void AssignProperties(Material copyFrom, Material copyTo)
	{
		copyTo.SetFloat(Smoothness, copyFrom.GetFloat(Smoothness));
		copyTo.SetFloat(Metallicness, copyFrom.GetFloat(Metallic));
		copyTo.SetColor(Color1, copyFrom.GetColor(Color1));
		copyTo.SetTexture(Texture1, copyFrom.GetTexture(MainTex));
		copyTo.SetTexture(AmbientOcclusion, copyFrom.GetTexture(OcclusionMap));
		copyTo.SetTexture(EmissionMap, copyFrom.GetTexture(Map));
		copyTo.SetTexture(MetallicMap, copyFrom.GetTexture(MetallicGlossMap));
	}

	public void ClearLevel()
	{
		foreach (Transform child in buildableParent)
		{
			Destroy(child.gameObject);
		}
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
