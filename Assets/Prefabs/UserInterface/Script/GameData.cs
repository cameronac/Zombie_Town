using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class GameData
{
    public float health;

    public bool shotgun;
    public bool pistol;
    public int checkpoint;
    public int numCarParts;
    public float[] playerPosition;
    public List<string> thingsToDestroy = new List<string>(0);

    public GameData(playerState player, GameObject pos)
    {
        health = player.health;
        shotgun = player.has_shotgun;
        pistol = player.has_pistol;
        numCarParts = player.numCarParts;
        playerPosition = new float[3];

        playerPosition[0] = pos.transform.position.x;
        playerPosition[1] = pos.transform.position.y;
        playerPosition[2] = pos.transform.position.z;

        foreach(string item in player.destroyItems)
        {
            thingsToDestroy.Add(item);
        }
    }
}
