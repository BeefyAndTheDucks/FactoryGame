using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMan : MonoBehaviour
{
    public string saveName = "SceneChangeData";

    static SceneMan instance;

	void Awake()
	{
        instance = this;
	}

	void Start()
	{
        StartCoroutine(nameof(WaitForLoad));
	}

	public static void LoadScene(int buildIndex)
	{
        SceneManager.LoadScene(buildIndex);

        PlayerPrefs.SetString(instance.saveName, SaveData.Save());

        //if (SceneManager.GetActiveScene().buildIndex != buildIndex)
        //{
        //    instance.StartCoroutine(nameof(WaitForSceneLoad), buildIndex);
        //}
    }

    IEnumerator WaitForLoad()
	{
        yield return new WaitForSeconds(2);

        SaveData.Load(PlayerPrefs.GetString(saveName));
    }

    //IEnumerator WaitForSceneLoad(int buildIndex)
    //{
    //    while (SceneManager.GetActiveScene().buildIndex != buildIndex)
    //    {
    //        yield return null;
    //    }

    //    Debug.Log("Load!");

    //    SaveData.Load(PlayerPrefs.GetString(saveName));
    //}

}
