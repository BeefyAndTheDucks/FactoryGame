using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerBuilder : MonoBehaviour
{
	public Material deconstructMaterial;

    private Camera _camera;
    private bool _isNew = true;
    private Buildable _last;
    private GameObject _current;
    private Quaternion _rotation = Quaternion.identity;
    private Vector3 _buildPos = Vector3.zero;
    private GameObject _lookingAt;
    private float _deconstructDeltaLeft;
    private float _deconstructPercentage;

    private bool LookingAtIsNull => _lookingAt == null;

    [SerializeField] private float maxDistance = Mathf.Infinity;
    [SerializeField] private Transform previewParent;
    [SerializeField] private Quaternion rotationStep;
    [SerializeField] private float deconstructTime;
    [SerializeField] private GameObject deconstructBar;
    [SerializeField] private RectTransform deconstructBarProgress;
    [SerializeField] private float deconstructBarEmpty = 975;
    [SerializeField] private float deconstructBarFull = 50;

    [HideInInspector] public bool deconstructMode;
    [HideInInspector] public GameObject lookingAtDeconstruct;
    [HideInInspector] public Buildable building;

    public static PlayerBuilder instance;

    private void Awake()
	{
        _camera = Camera.main;
        _deconstructDeltaLeft = deconstructTime;
        building = null;
        AssignSingleton();
	}
    
    private void AssignSingleton()
    {
	    if (instance != null)
	    {
		    Debug.LogError("More than one Builder script in scene.");
		    return;
	    }

	    instance = this;
    }

    private void FixedUpdate()
    {
        if (building != _last)
            _isNew = true;
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance))
		{
            Debug.DrawLine(ray.origin, hit.point, Color.green);
            
            _lookingAt = hit.transform.gameObject;

            if (building != null && building.previewPrefab != null)
			{
                _buildPos = hit.point + building.BuildOffset;
                if (_isNew)
				{
                    if (_current != null)
                        Destroy(_current);
                    _current = Instantiate(building.previewPrefab, previewParent);
                    _rotation = building.BaseRotation;
                    _isNew = false;
                }
            }

            if (_current != null)
			{
                _current.transform.SetPositionAndRotation(_buildPos, _rotation);
                _current.SetActive(true);
            }
        } else
		{
            if (_current != null)
                _current.SetActive(false);
            _lookingAt = null;
		}

        lookingAtDeconstruct = deconstructMode ? _lookingAt : null;
        
        _last = building;
    }

    private void Update()
	{
		if (Input.GetButtonDown("Place"))
		{
            if (building != null && building.prefab != null && !deconstructMode)
			{
                BuildProperties properties = new BuildProperties(_rotation, _buildPos, building);
                Builder.instance.Build(properties);
			}
		}

		var builtBuildable = (LookingAtIsNull) ? null : _lookingAt.GetComponent<BuiltBuildable>();

		if (Input.GetButton("Place"))
		{
			if (deconstructMode && !LookingAtIsNull && builtBuildable != null && !builtBuildable.isDeconstructing)
			{
				if (_deconstructDeltaLeft == 0)
				{
					if (builtBuildable != null) builtBuildable.isDeconstructing = true;
					Builder.instance.Deconstruct(lookingAtDeconstruct);
					_deconstructDeltaLeft = Mathf.Infinity;
				}
				else
				{
					_deconstructDeltaLeft -= Time.deltaTime;
					_deconstructDeltaLeft = Mathf.Clamp(_deconstructDeltaLeft, 0, deconstructTime);
					_deconstructPercentage = Map(_deconstructDeltaLeft, 0, deconstructTime, deconstructBarEmpty,
						deconstructBarFull);

					deconstructBarProgress.offsetMax =
						new Vector2(-_deconstructPercentage, deconstructBarProgress.offsetMax.y);
				}
			}
		}
		else
		{
			_deconstructDeltaLeft = deconstructTime;
			_deconstructPercentage = 0;
		}
		
		if (builtBuildable != null) builtBuildable.showMaterial = !Input.GetButton("Place");

		if (Input.GetButtonDown("Exit Build Mode"))
		{
            RemovePreview();
            deconstructMode = false;
		}

        if (Input.GetButtonDown("Deconstruct"))
		{
            deconstructMode = !deconstructMode;
            RemovePreview();
        }

        if (Input.GetButtonDown("Pick"))
        {
	        if (_lookingAt != null)
	        {
		        var buildable = _lookingAt.GetComponent<BuiltBuildable>();
		        if (buildable != null)
		        {
			        building = buildable.buildable;
		        }
	        }

	        deconstructMode = false;
        }
        
        deconstructBar.SetActive(deconstructMode && Input.GetButton("Place") && !LookingAtIsNull && builtBuildable != null && !builtBuildable.isDeconstructing);

        _rotation.eulerAngles += (rotationStep.eulerAngles * Input.GetAxis("Rotate"));
	}

    public void RemovePreview()
	{
        building = null;

        if (_current != null)
            Destroy(_current);
    }
    
    private static float Map(float x, float x1, float x2, float y1,  float y2)
    {
	    var m = (y2 - y1) / (x2 - x1);
	    var c = y1 - m * x1; // point of interest: c is also equal to y2 - m * x2, though float math might lead to slightly different results.
 
	    return m * x + c;
    }
}
