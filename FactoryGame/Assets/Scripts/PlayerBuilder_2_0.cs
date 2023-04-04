using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerBuilder_2_0 : NetworkBehaviour {
    public Material deconstructMaterial;

    private Camera _camera;
    private bool _buildingIsNew = true;
    private Buildable _last;
    private GameObject _current;
    private Quaternion _rotation;
    private Vector3 _buildPos;
    private GameObject _lookingAt;
    private bool _lookingAtIsNull = true;
    private float _deconstructDeltaLeft;
    private float _deconstructPercentage;
    private GameObject _deconstructBar;
    private RectTransform _deconstructBarProgress;
    private Transform _previewParent;
    private readonly NetworkVariable<NetworkObjectReference> _lookingAtDeconstruct = new(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Owner);
    private bool _lookingAtDeconstructIsNull;

    [SerializeField] private float maxRayDistance = Mathf.Infinity;
    [SerializeField] private Quaternion rotationStep;
    [SerializeField] private float deconstructTime;
    [SerializeField] private float deconstructBarEmpty = 975;
    [SerializeField] private float deconstructBarFull = 50;

    [HideInInspector] public bool deconstructMode;
    [HideInInspector] public Buildable building;

    public override void OnNetworkSpawn()
	{
        if (IsOwner)
            Init();
	}
    
    private GameObject LookingAt {
        get => _lookingAt;
        set {
            _lookingAtIsNull = value == null;
            _lookingAt = value;
        }
    }

    public GameObject LookingAtDeconstruct {
        get {
            if (_lookingAtDeconstructIsNull) {
                print("NULL");
                return null;
            } else {
                if (_lookingAtDeconstruct.Value.TryGet(out var obj)) {
                    print("NOT NULL");
                    return obj.gameObject;
                } else {
                    print("NULL");
                    return null;
                }
            }
        }
        private set {
            _lookingAtDeconstructIsNull = value == null;
            if (!_lookingAtDeconstructIsNull)
                _lookingAtDeconstruct.Value = new(value);
            else
                _lookingAtDeconstruct.Value = new();
        }
    }

	private void Init() {
		_camera = Camera.main;
        _deconstructDeltaLeft = deconstructTime;
        building = null;
        _deconstructBar = PlayerBuilderReferences.Singleton.deconstructBar;
        _deconstructBarProgress = PlayerBuilderReferences.Singleton.deconstructBarProgress;
        _previewParent = PlayerBuilderReferences.Singleton.previewParent;
        
        GameManager.Singleton.OnPlayerAddedServerRpc(new NetworkObjectReference(gameObject));
    }

	private void Update()
	{
		if (!IsOwner) return;
        if (!IsSpawned) return;

        if (building != _last)
            _buildingIsNew = true;

        var currentIsNull = _current == null;

        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, maxRayDistance)) {
            Debug.DrawLine(ray.origin, hit.point, Color.green);

            LookingAt = hit.transform.gameObject;

            if (building != null && building.previewPrefab != null) {
                _buildPos = hit.point + building.BuildOffset;
                if (_buildingIsNew) {
                    if (!currentIsNull)
                        Destroy(_current);
                    _current = Instantiate(building.previewPrefab, _previewParent);
                    _rotation = building.BaseRotation;
                    _buildingIsNew = false;
                }
            }

            if (!currentIsNull) {
                _current.transform.SetPositionAndRotation(_buildPos, _rotation);
                _current.SetActive(true);
            }
        } else {
            if (!currentIsNull)
                _current.SetActive(false);
            LookingAt = null;
        }

        LookingAtDeconstruct = deconstructMode && !_lookingAtIsNull ? LookingAt : null;

        _last = building;

        // Pressed
        if (Input.GetButtonDown("Place")) {
            if (building != null && building.prefab != null && !deconstructMode) {
                var properties = new BuildProperties(_rotation, _buildPos, building.UUID);
                Builder.Singleton.BuildServerRpc(properties);
			}
		}

        var builtBuildableComponent = _lookingAtIsNull ? null : LookingAt.GetComponent<BuiltBuildable>();

        // Held
        if (Input.GetButton("Place")) {
            if (deconstructMode && !_lookingAtIsNull && builtBuildableComponent != null && !builtBuildableComponent.isDeconstructing) {
                if (_deconstructDeltaLeft == 0) {
                    builtBuildableComponent.isDeconstructing = true;
                    Builder.Singleton.DeconstructServerRpc(LookingAtDeconstruct.GetComponent<NetworkBehaviourReferenceObject>(), default);
                    //lookingAt = null;
                    _deconstructDeltaLeft = Mathf.Infinity;
				} else {
                    _deconstructDeltaLeft -= Time.deltaTime;
                    _deconstructDeltaLeft = Mathf.Clamp(_deconstructDeltaLeft, 0, deconstructTime);
                    _deconstructPercentage = _deconstructDeltaLeft.Map(0, deconstructTime, deconstructBarEmpty, deconstructBarFull);

                    _deconstructBarProgress.offsetMax =
                        new Vector2(-_deconstructPercentage, _deconstructBarProgress.offsetMax.y);
				}
			}
		} else {
            _deconstructDeltaLeft = deconstructTime;
            _deconstructPercentage = 0;
		}

        if (builtBuildableComponent != null) builtBuildableComponent.showMaterial = !Input.GetButton("Place");

        if (Input.GetButtonDown("Exit Build Mode"))
            ExitBuildMode();

        if (Input.GetButtonDown("Deconstruct")) {
            deconstructMode = !deconstructMode;
            ExitBuildMode(false);
		}

        if (Input.GetButtonDown("Pick")) {
            if (!_lookingAtIsNull) {
                if (builtBuildableComponent != null) {
                    deconstructMode = false;
                    building = builtBuildableComponent.buildable;
				}
			}
		}

        _deconstructBar.SetActive(deconstructMode && Input.GetButton("Place") && !_lookingAtIsNull && builtBuildableComponent != null && !builtBuildableComponent.isDeconstructing);

        _rotation.eulerAngles += rotationStep.eulerAngles * Input.GetAxis("Rotate");
    }

    public void ExitBuildMode (bool exitDeconstructMode = true) {
        building = null;
        if (exitDeconstructMode)
            deconstructMode = false;

        if (_current != null)
            Destroy(_current);
	}
}
