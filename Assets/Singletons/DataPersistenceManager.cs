using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
 public static DataPersistenceManager Instance;
    private List<IData> dataPresistenceObjects;

    private GameData data;
    private void Awake()
    { 
        Instance = this;
    }
    public void Start()
    {
        this.dataPresistenceObjects = FindAllDataPersistenceObjects();
    }

    public void loadGame()
    {
        foreach (IData dataPresistenceObjects in dataPresistenceObjects)
        {
            dataPresistenceObjects.LoadData(data);
        }
    }

    public void saveGame()
    {
        foreach(IData dataPresistenceObjects in dataPresistenceObjects)
        {
            dataPresistenceObjects.SaveData(ref  data);
        }
    }
    private List<IData> FindAllDataPersistenceObjects()
    {
        IEnumerable<IData> dataPresistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IData>();

        return new List<IData>(dataPresistenceObjects);
    }
}

