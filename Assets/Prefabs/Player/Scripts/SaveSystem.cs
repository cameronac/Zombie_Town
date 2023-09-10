using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting.FullSerializer;

public static class SaveSystem
{
    public static void SavePlayer(playerState player, GameObject pos)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.dataucannottouch";

        FileStream stream = new FileStream(path, FileMode.Create);

        GameData data = new GameData(player, pos);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static GameData LoadPlayer()
    {
        string path = Application.persistentDataPath + "/player.dataucannottouch";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();

            return data;

        }
        else
        {
            Debug.LogError("File Path does not exist in " + path);
            return null;
        }
    }
}
