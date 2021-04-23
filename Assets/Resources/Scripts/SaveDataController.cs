using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveDataController
{
    public static void Save(params ISaveable[] saveData)
    {
        foreach (ISaveable data in saveData)
        {
            string path = Application.persistentDataPath + data.GetDataPath();

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);

            SaveData fileData = new SaveData(data);

            formatter.Serialize(stream, fileData);
            stream.Close();

            Debug.Log("Successfully saved to " + path);
        }
    }

    public static SaveData[] Load(params ISaveable[] saveData)
    {
        SaveData[] saveDataArray = new SaveData[saveData.Length];

        for (int i = 0; i < saveData.Length;)
        {
            string path = Application.persistentDataPath + saveData[i].GetDataPath();

            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                SaveData fileData = formatter.Deserialize(stream) as SaveData;
                stream.Close();

                saveDataArray[i] = fileData;

                Debug.Log("Successfully loaded from " + path);
            }
            else
            {
                Debug.LogError("Save file not found in path.");
            }
        }

        return saveDataArray;
    }
}
