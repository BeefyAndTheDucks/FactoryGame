using System;
using System.Linq;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Singleton { get; private set; }

    public NetworkVariable<Action> OnNewPlayerAdded = new(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);

    private readonly NetworkVariable<List<NetworkObjectReference>> _allPlayerBuilders =
        new(readPerm: NetworkVariableReadPermission.Owner, writePerm: NetworkVariableWritePermission.Server);

    public List<PlayerBuilder_2_0> AllPlayerBuilders
    {
        get
        {
            var change = _allPlayerBuilders.Value;
            List<PlayerBuilder_2_0> result = new();
            change.ForEach(val =>
            {
                val.TryGet(out var netObj);
                result.Add(netObj.GetComponent<PlayerBuilder_2_0>());
            });
            return result;
        }
    }

    private void Awake()
    {
        AssignSingleton();
    }
    
    [ServerRpc]
    public void OnPlayerAddedServerRpc(NetworkObjectReference player)
    {
        _allPlayerBuilders.Value.Add(player);
        OnNewPlayerAdded.Value?.Invoke();
    }

    private void AssignSingleton()
    {
        if (Singleton == null)
        {
            Singleton = this;
            return;
        }
        
        Debug.LogError("More than one \"" + name + "\" in scene!");
    }
}
