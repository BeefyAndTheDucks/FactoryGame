using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();

		if (!IsOwner)
		{
			Destroy(GetComponent<FirstPersonController>());
			Destroy(GetComponent<Camera>());
			Destroy(this);
		} else {
			BuildMenu.instance.Controller = GetComponent<FirstPersonController>();
		}
	}
}
