using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class PlayerData
{
    public bool tutorialPassed;
    public int coinsTotal;
    public int stepsBest;

    public int[] inventoryIDs;

    public PlayerData()
    {
        inventoryIDs = new int[] { -1, -1, -1 };
    }

    public static void Save(PlayerData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.drs";

        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData Load()
    {
        string path = Application.persistentDataPath + "/player.drs";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;

            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError($"File not found in {path}");

            PlayerData data = new PlayerData();

            return data;
        }
    }

    public static void Clear()
    {
        PlayerData data = new PlayerData();
        Save(data);
    }
}