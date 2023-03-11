using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CrossSceneDataKeeper : MonoBehaviour
{
	public static string fileName;

    public void GoToMainScene(StoredLevel levelToLoad)
	{
		fileName = levelToLoad.fileName;

		DontDestroyOnLoad(gameObject);

		SceneManager.LoadScene(1);

		StartCoroutine(LoadNewLevel(levelToLoad));
	}

	IEnumerator LoadNewLevel(StoredLevel levelToLoad)
	{
		yield return new WaitForSeconds(0.1f);

		print("LOADING NEW LEVEL!");
		levelToLoad.ToLevel();
	}
}
