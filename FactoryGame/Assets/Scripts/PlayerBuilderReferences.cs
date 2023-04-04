using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerBuilderReferences : NetworkBehaviour
{
	[Header("Main")]
	public GameObject deconstructBar;
	public RectTransform deconstructBarProgress;
	public Transform previewParent;
	[Header("Network Development Help")]
	[SerializeField] private NetworkDevelopmentHelperMode mode;

	public static PlayerBuilderReferences Singleton;

	void Awake() {
		AssignSingleton();
	}

	private void AssignSingleton() {
		if (Singleton != null) {
			Debug.LogError("More than one " + name + " script in scene.");
			return;
		}

		Singleton = this;
	}

	void Start() {
		if ((Application.isEditor && mode == NetworkDevelopmentHelperMode.EditorIsHost) || ((Debug.isDebugBuild && !Application.isEditor) && mode == NetworkDevelopmentHelperMode.BuildIsHost)) {
			NetworkManager.Singleton.StartHost();
		} else {
			NetworkManager.Singleton.StartClient();
		}
	}

	private enum NetworkDevelopmentHelperMode {
		BuildIsHost,
		EditorIsHost,
		NoneAreHost
	}
}
