using UnityEngine;
using UnityEngine.UI;

public class BuildMenu : MonoBehaviour
{
    private Builder _builder;
    private Buildable[] _buildables;
    
    [SerializeField] private Button BuildMenuItemPrefab;
    [SerializeField] private Category[] Categories;
    [SerializeField] private Transform CategoryParent;
    [SerializeField] private Transform BuildableParent;
    [SerializeField] private PlayerBuilder PlayerBuilder;
    [HideInInspector] public FirstPersonController Controller;
    [SerializeField] private GameObject Menu;

    [HideInInspector] public static BuildMenu instance;

    private void AssignSingleton()
    {
        if (instance != null)
        {
            Debug.LogError("More than one BuildMenu script in scene.");
            return;
        }

        instance = this;
    }

	void Awake()
	{
        AssignSingleton();
    }

	private void Start()
    {
        _builder = Builder.instance;
        _buildables = _builder.buildables;
        _generateCategories();
    }

    private void _generateCategories()
    {
        // Generate the categories
        foreach (var category in Categories)
        {
            var newCategory = Instantiate(BuildMenuItemPrefab, CategoryParent);
            newCategory.image.sprite = category.icon;
            newCategory.onClick.AddListener(() => _onCategoryButtonClick(category));
        }
        
        _onCategoryButtonClick(Categories[0]);
    }

    private void _onCategoryButtonClick(Category category)
    {
        if (category == null)
            return;
        
        foreach (Transform child in BuildableParent)
            Destroy(child.gameObject);
        
        foreach (var buildable in _buildables)
        {
            if (buildable.Category != category)
                continue;
            
            var newCategory = Instantiate(BuildMenuItemPrefab, BuildableParent);
            newCategory.image.sprite = buildable.Icon;
            newCategory.onClick.AddListener(() => _onBuildableButtonClick(buildable));
        }
    }

    private void _onBuildableButtonClick(Buildable buildable)
    {
        PlayerBuilder.building = buildable;
        _toggleBuildMenu();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Build"))
            if ((PlayerBuilder.building == null || PlayerBuilder.building.prefab == null))
                _toggleBuildMenu();
            else if (PlayerBuilder.deconstructMode)
            {
                PlayerBuilder.ExitBuildMode();
                _toggleBuildMenu();
            }
            else
                PlayerBuilder.ExitBuildMode();

        if (Input.GetButtonDown("Exit Build Mode"))
        {
            _switchBuildMenuTo(false);
        }
    }

    private void _toggleBuildMenu()
    {
        _switchBuildMenuTo(!Menu.activeSelf);
    }

    private void _switchBuildMenuTo(bool to)
    {
        Menu.SetActive(to);

        Cursor.lockState = (to) ? CursorLockMode.None : CursorLockMode.Locked;
        Controller.playerCanMove = !to;
        Controller.enableHeadBob = !to;
        Controller.cameraCanMove = !to;
    }
}
