using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnerManager : MonoBehaviour
{
    [SerializeField] private GameObject zombieprefab;

    [SerializeField] private float zombieInterval;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(spawnerEnemies(zombieInterval, zombieprefab));
    }

  private IEnumerator spawnerEnemies(float interval, GameObject zombie)
    {
        yield return new WaitForSeconds(interval);
        GameObject newZombie = Instantiate(zombie, new Vector3(Random.Range(-10f, 15), Random.Range(5f, 5f), 0), Quaternion.identity);
        StartCoroutine(spawnerEnemies(interval, zombie));
    }
}
