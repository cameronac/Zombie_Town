using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizePlacement : MonoBehaviour
{
    [SerializeField] GameObject spawn;
	[SerializeField] GameObject[] points;
	//needs to be between 1 - 100
	[SerializeField] int spawnWeight;

	[SerializeField] int minAmount;
	
	void Start()
	{
		bool[] isThere = new bool[points.Length];
		int actualAmount = 0;
		do 
		{
			for(int i = 0; i < points.Length; i++)
			{
				//random number 1-100
				int check = Random.Range(1,101);
				//if we fall within range, then spawn the object. Higher number == higher chance of spawning
				if(check <= spawnWeight && isThere[i] != true) {
					Instantiate(spawn, points[i].transform.position,Quaternion.identity);
					isThere[i] = true;
					actualAmount++;
				}
			}
		}  while(actualAmount <= minAmount && minAmount != -1);
	}
}
