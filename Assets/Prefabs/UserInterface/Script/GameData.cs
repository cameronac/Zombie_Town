using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class GameData
{
    public float health;

    public bool shotgun;
    public bool pistol;
    public int checkpoint;
    public int numCarParts;
    public float[] playerPosition;

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
    }
}
