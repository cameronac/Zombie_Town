using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveData 
{
  public static void PlayerSave(playerState player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.zombietown";
        FileStream stream = new FileStream(path, FileMode.Create);

        GameData data = new GameData(player);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static GameData LoadPlayer()
    {
        string path = Application.persistentDataPath + "/player.zombietown";
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData data =formatter.Deserialize(stream) as GameData;
            stream.Close();

            return data;
        }
        else
        {
            return null;
        }


    }
}
