using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
	public GameObject jointGO;

	public override void OnNetworkSpawn()
	{
		jointGO.SetActive(IsOwner);
		if (IsOwner)
		{
			BuildMenu.Singleton.Controller = GetComponent<FirstPersonController>();
			BuildMenu.Singleton.playerBuilder = GetComponent<PlayerBuilder_2_0>();
		}
	}
}
