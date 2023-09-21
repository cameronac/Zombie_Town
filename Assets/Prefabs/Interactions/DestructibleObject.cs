using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour, IDamage
{
    [SerializeField] float hp;
    [SerializeField] AudioClip hit_sound;
    [SerializeField] AudioClip destroy_sound;
    [SerializeField] Transform[] allChildren;
    int index;

    public void TakeDamage(float damage)
    {
        allChildren = GetComponentsInChildren<Transform>();
        List<GameObject> childObjects = new List<GameObject>();
        for(int i = 1; i < allChildren.Length; i++)
        {
            childObjects.Add(allChildren[i].gameObject);
        }
        hp -= damage;

        if (hp <= 0)
        {
            playerState.instance.destroyItems.Add(gameObject.name);
            AudioManager.instance.CreateSoundAtPosition(destroy_sound, transform.position);
            Destroy(gameObject);
        }
        else
        {
            AudioManager.instance.CreateSoundAtPosition(hit_sound, transform.position);
            index = Random.Range(0, childObjects.Count);
            Destroy(childObjects[index]);
        }
    }
}
