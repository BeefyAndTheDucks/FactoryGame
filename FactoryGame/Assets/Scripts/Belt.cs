using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BuiltBuildable))]
public class Belt : MonoBehaviour
{
    public int itemsPerSecond = 1;
	public Transform itemParent;

	[HideInInspector]
	public GameObject[,,] grid;
	//[HideInInspector]
	public Transform currentItem;
	[HideInInspector]
	public BuiltBuildable builtBuildable;

	void Start()
	{
		builtBuildable = GetComponent<BuiltBuildable>();
	}

	public void AddToBelt(Transform item)
	{
		currentItem = item;
		currentItem.SetParent(itemParent);
		currentItem.localPosition = new Vector3(0, 0, -.5f);
	}

	void Update()
	{
		grid = PlayerBuildManager.instance.grid;
		if (currentItem != null && currentItem.localPosition.z < .5f)
			currentItem.localPosition = new Vector3(0, 0, currentItem.localPosition.z - Time.deltaTime * Application.targetFrameRate * itemsPerSecond);
		else if (currentItem != null && currentItem.localPosition.z >= .5f)
		{
			currentItem.localPosition = new Vector3(0, 0, .5f);
			NextBelt();
		}
	}

	public virtual void NextBelt()
	{
		if (transform.eulerAngles.y == 0)
		{
			GameObject gridItem = grid[builtBuildable.gridIndicies.x, builtBuildable.gridIndicies.y, builtBuildable.gridIndicies.z + 1];

			if (gridItem != null && gridItem.GetComponent<Belt>() != null)
			{
				gridItem.GetComponent<Belt>().AddToBelt(currentItem);
				currentItem = null;
			}
		}
		else if (transform.eulerAngles.y == 90)
		{
			GameObject gridItem = grid[builtBuildable.gridIndicies.x + 1, builtBuildable.gridIndicies.y, builtBuildable.gridIndicies.z];

			if (gridItem != null && gridItem.GetComponent<Belt>() != null)
			{
				gridItem.GetComponent<Belt>().AddToBelt(currentItem);
				currentItem = null;
			}
		}
		else if (transform.eulerAngles.y == 180)
		{
			GameObject gridItem = grid[builtBuildable.gridIndicies.x, builtBuildable.gridIndicies.y, builtBuildable.gridIndicies.z - 1];

			if (gridItem != null && gridItem.GetComponent<Belt>() != null)
			{
				gridItem.GetComponent<Belt>().AddToBelt(currentItem);
				currentItem = null;
			}
		}
		else if (transform.eulerAngles.y == -90)
		{
			GameObject gridItem = grid[builtBuildable.gridIndicies.x - 1, builtBuildable.gridIndicies.y, builtBuildable.gridIndicies.z];

			if (gridItem != null && gridItem.GetComponent<Belt>() != null)
			{
				gridItem.GetComponent<Belt>().AddToBelt(currentItem);
				currentItem = null;
			}
		}
	}
}
