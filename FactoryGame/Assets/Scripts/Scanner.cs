using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
	[Header("Testing")]
	public Vector3 testPosition;
	public float testMaxSize = 100;

	[Header("Main")]
	public float growSpeed = 5;

	public void StartScan(Vector3 position, float maxSize)
	{
		transform.position = position;

		gameObject.SetActive(true);

		StartCoroutine(scanAnimation(maxSize));
	}

	IEnumerator scanAnimation(float maxSize)
	{
		while ((transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3 <= maxSize)
		{
			transform.localScale += Vector3.one * Time.deltaTime * growSpeed * (((transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3) / 10 + 1);
			yield return null;
		}

		gameObject.SetActive(false);
		transform.localScale = Vector3.zero;
	}
}
