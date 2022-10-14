using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BuiltBuildable))]
public class CornerBelt : Belt
{
	BuiltBuildable builtBuildable;

	public new void NextBelt()
	{
		if (transform.eulerAngles.y == -90)
		{
			GameObject gridItem = grid[builtBuildable.gridIndicies.x, builtBuildable.gridIndicies.y, builtBuildable.gridIndicies.z + 1];

			if (gridItem != null && gridItem.GetComponent<Belt>() != null)
			{
				gridItem.GetComponent<Belt>().AddToBelt(currentItem);
				currentItem = null;
			}
		} else if (transform.eulerAngles.y == 0)
		{
			GameObject gridItem = grid[builtBuildable.gridIndicies.x + 1, builtBuildable.gridIndicies.y, builtBuildable.gridIndicies.z];

			if (gridItem != null && gridItem.GetComponent<Belt>() != null)
			{
				gridItem.GetComponent<Belt>().AddToBelt(currentItem);
				currentItem = null;
			}
		} else if (transform.eulerAngles.y == 90)
		{
			GameObject gridItem = grid[builtBuildable.gridIndicies.x, builtBuildable.gridIndicies.y, builtBuildable.gridIndicies.z - 1];

			if (gridItem != null && gridItem.GetComponent<Belt>() != null)
			{
				gridItem.GetComponent<Belt>().AddToBelt(currentItem);
				currentItem = null;
			}
		} else if (transform.eulerAngles.y == 180)
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
