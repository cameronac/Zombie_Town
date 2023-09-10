using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawner : MonoBehaviour
{
    [SerializeField] GameObject zombiePrefab;
    [SerializeField] Transform[] spawnPos;
    [SerializeField] int numberToSpawn;
    [SerializeField] float timeBetweenSpawn;
    [SerializeField] List<GameObject> objectList = new List<GameObject>();

    int numberSpawned;
    bool isSpawning;
    bool startSpawning;

    // Update is called once per frame
    private void Update()
    {
        if (startSpawning && !isSpawning && numberSpawned < numberToSpawn)
        {
            StartCoroutine(spawn());
        }
    }

    public IEnumerator spawn()
    {
        isSpawning = true;

        GameObject objectSpawned = Instantiate(zombiePrefab, spawnPos[Random.Range(0, spawnPos.Length)].position, zombiePrefab.transform.rotation);

        if (objectSpawned.GetComponent<enemyAI>())
        {
            objectSpawned.GetComponent<enemyAI>().whereISpawned = this;
        }

        objectList.Add(objectSpawned);
        numberSpawned++;

        yield return new WaitForSeconds(timeBetweenSpawn);
        isSpawning = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startSpawning = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            for (int i = 0; i < objectList.Count; i++)
            {
                if (objectList[i] != null)
                {
                    Destroy(objectList[i]);
                }
            }

            objectList.Clear();
            numberSpawned = 0;
            startSpawning = false;
        }
    }

    public void enemiesDead()
    {
        numberToSpawn--;
    }
}
