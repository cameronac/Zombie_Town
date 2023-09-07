using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class GameData
{

    public Vector3 playerPosition;

    public GameData()
    {
      playerPosition = Vector3.zero;
    }
}
