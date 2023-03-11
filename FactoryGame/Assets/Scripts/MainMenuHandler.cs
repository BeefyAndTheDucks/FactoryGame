using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

public class MainMenuHandler : MonoBehaviour
{
    [Header("Buttons")]
    public Button continueButton;

    [Header("Misc")]
    public Transform loadSaveGameParent;

    public TMP_InputField createSaveInputField;

    public TextMeshProUGUI continueButtonText;
    public SaveGameUIInfo saveGameUIPrefab;

    public CrossSceneDataKeeper dataKeeper;

    [Header("UI Windows")]
    public GameObject buttons;
    public GameObject load;
    public GameObject newGame;

    public void PointerEnter()
    {
        continueButtonText.text = string.Concat("Save: ", PlayerPrefs.GetString("lastSaveName", "No Save"));
    }

    public void PointerExit()
    {
        continueButtonText.text = "CONTINUE";
    }

    public void ContinueButtonClick()
	{
        if (PlayerPrefs.HasKey("lastSaveName"))
		{
            OnSaveGameClicked(SaveManager.Load(PlayerPrefs.GetString("lastSaveName")));
        }
	}

    public void OnLoadButtonClicked()
	{
        buttons.SetActive(false);
        newGame.SetActive(false);
        load.SetActive(true);
	}

    public void OnBackButtonClicked()
	{
        buttons.SetActive(true);
        newGame.SetActive(false);
        load.SetActive(false);
    }

    public void OnNewGameButtonClicked()
	{
        buttons.SetActive(false);
        newGame.SetActive(true);
        load.SetActive(false);
    }

    public void OnCreateSaveButtonClicked()
	{
        if (createSaveInputField.text != "")
            OnSaveGameClicked(new(createSaveInputField.text, true));
	}

    private void Start()
    {
        continueButton.interactable = PlayerPrefs.HasKey("lastSaveName");
        Application.quitting += _SaveOnExit;

		foreach (StoredLevel save in GetSaves())
		{
            SaveGameUIInfo instantiatedSave = Instantiate(saveGameUIPrefab, loadSaveGameParent);
            instantiatedSave.nameText.text = save.fileName;

            StringBuilder builder = new();
            builder.Append(save.playtime.hours); builder.Append("h ");
            builder.Append(save.playtime.minutes); builder.Append("m ");
            builder.Append(save.playtime.seconds); builder.Append("s");
            instantiatedSave.playtimeText.text = builder.ToString();

            builder = new();
            builder.Append(save.lastplayed.day); builder.Append("/");
            builder.Append(save.lastplayed.month); builder.Append("/");
            builder.Append(save.lastplayed.year);

            instantiatedSave.lastPlayedText.text = builder.ToString();

            instantiatedSave.button.onClick.AddListener(() => OnSaveGameClicked(save));
        }
    }

    public void Exit() => Application.Quit();

    private void _SaveOnExit()
    {
        PlayerPrefs.Save();
        if (SaveManager.CurrentSaveName == "")
            return;
        SaveManager.Save(SaveManager.CurrentSaveName);
    }

    List<StoredLevel> GetSaves()
	{
        var result = new List<StoredLevel>();

        var path = Path.Combine(Application.persistentDataPath, "saves");
        var info = new DirectoryInfo(path);

        BinaryFormatter formatter = new BinaryFormatter();

        foreach (FileInfo file in info.GetFiles("*.factory"))
		{
            if (!file.Exists) continue;
            var filePath = file.FullName;
            
            FileStream stream = new FileStream(filePath, FileMode.Open);

            StoredLevel level = formatter.Deserialize(stream) as StoredLevel;
            stream.Close();

            result.Add(level);
        }

        return result;
    }

    public void OnSaveGameClicked(StoredLevel save)
    {
        print("SAVEGAME CLICKED!");
        dataKeeper.GoToMainScene(save);
    }
}
