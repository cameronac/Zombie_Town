using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] enemies;
    [SerializeField] GameObject[] spawnPositions;

    private void SpawnEnemies()
    {
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            Instantiate(enemies[Random.Range(0, enemies.Length)], spawnPositions[i].transform.position, spawnPositions[i].transform.rotation);
        }

        Destroy(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SpawnEnemies();
        }
    }
}
