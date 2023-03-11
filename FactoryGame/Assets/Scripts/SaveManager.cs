using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveManager
{
	public static string CurrentSaveName = "";
	
    public static void Save(string saveName)
	{
		BinaryFormatter formatter = new BinaryFormatter();
		//string path = Application.persistentDataPath + "/" + saveName + ".factory";
		string path = Path.Combine(Application.persistentDataPath, "saves");
		Directory.CreateDirectory(path);

		path = Path.Combine(path, saveName + ".factory");
		FileStream stream = new FileStream(path, FileMode.OpenOrCreate);

		StoredLevel level = new StoredLevel(saveName);

		formatter.Serialize(stream, level);
		stream.Close();

		CurrentSaveName = saveName;
		PlayerPrefs.SetString("lastSaveName", saveName);
	}

	public static StoredLevel Load(string saveName)
	{
		string path = Path.Combine(Application.persistentDataPath, "saves", saveName + ".factory");
		if (File.Exists(path))
		{
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream = new FileStream(path, FileMode.Open);

			StoredLevel level = formatter.Deserialize(stream) as StoredLevel;
			stream.Close();

			return level;
		} else
		{
			Debug.LogError("Save file not found in " + path);
			return null;
		}
	}

}
