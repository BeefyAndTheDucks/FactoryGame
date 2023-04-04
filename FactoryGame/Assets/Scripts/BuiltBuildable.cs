using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BuiltBuildable : NetworkBehaviour
{
    public Buildable buildable;

    [HideInInspector] public bool showMaterial = true;
    [HideInInspector] public bool isDeconstructing;

    private Material _originalMaterial;
    private Renderer _renderer;
    private List<PlayerBuilder_2_0> _plBuilders;

    private void Update()
    {
        if (!IsSpawned) return;
        if (_plBuilders == null) return;
        if (_plBuilders.Count < 1) return;
        
        if (_renderer.sharedMaterial != _originalMaterial && _renderer.sharedMaterial != _plBuilders[0].deconstructMaterial)
            _originalMaterial = _renderer.sharedMaterial;
        _renderer.sharedMaterial = (_plBuilders.Any(player => player.LookingAtDeconstruct == gameObject) && showMaterial && !isDeconstructing) ? _plBuilders[0].deconstructMaterial : _originalMaterial;
    }

    public override void OnNetworkSpawn()
	{
        _plBuilders = GameManager.Singleton.AllPlayerBuilders;
        GameManager.Singleton.OnNewPlayerAdded.Value += OnNewPlayerBuilderConnectedClientRpc;
        _renderer = GetComponent<Renderer>();
        _originalMaterial = _renderer.sharedMaterial;
    }
    
    [ClientRpc]
    private void OnNewPlayerBuilderConnectedClientRpc() => _plBuilders = GameManager.Singleton.AllPlayerBuilders;
}
