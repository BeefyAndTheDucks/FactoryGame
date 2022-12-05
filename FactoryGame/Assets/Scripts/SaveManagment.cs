using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveManagment
{
    public static void Save(string saveName)
	{
		BinaryFormatter formatter = new BinaryFormatter();
		string path = Application.persistentDataPath + "/" + saveName + ".factory";
		FileStream stream = new FileStream(path, FileMode.OpenOrCreate);

		StoredLevel level = new StoredLevel();

		formatter.Serialize(stream, level);
		stream.Close();
	}

	public static StoredLevel Load(string saveName)
	{
		string path = Application.persistentDataPath + "/" + saveName + ".factory";
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
