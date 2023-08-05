using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizePlacement : MonoBehaviour
{
    [SerializeField] GameObject spawn;
	[SerializeField] GameObject[] points;
	//needs to be between 1 - 100
	[SerializeField] int spawnWeight;
	
	void Start()
	{
		for(int i = 0; i < points.Length; i++)
		{
			//random number 1-100
			int check = Random.Range(1,101);
			//if we fall within range, then spawn the object. Higher number == higher chance of spawning
			if(check <= spawnWeight) {
				Instantiate(spawn, points[i].transform.position,Quaternion.identity);
			}
		}
		
	}
}
