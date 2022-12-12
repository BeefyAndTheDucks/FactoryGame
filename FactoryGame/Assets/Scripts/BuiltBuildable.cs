using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuiltBuildable : MonoBehaviour
{
    public Buildable buildable;

    [HideInInspector] public bool showMaterial = true;
    [HideInInspector] public bool isDeconstructing;

    private Material _originalMaterial;
    private Renderer _renderer;
    private PlayerBuilder _plBuilder;

    private void Update()
    {
        if (_renderer.sharedMaterial != _originalMaterial && _renderer.sharedMaterial != _plBuilder.deconstructMaterial)
            _originalMaterial = _renderer.sharedMaterial;
        _renderer.sharedMaterial = (_plBuilder.lookingAtDeconstruct == gameObject && showMaterial) ? _plBuilder.deconstructMaterial : _originalMaterial;
    }

    private void Start()
    {
        _plBuilder = PlayerBuilder.instance;
        _renderer = GetComponent<Renderer>();
        _originalMaterial = _renderer.sharedMaterial;
    }
}
