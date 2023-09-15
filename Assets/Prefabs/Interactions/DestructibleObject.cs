using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour, IDamage
{
    [SerializeField] float hp;
    [SerializeField] AudioClip hit_sound;
    [SerializeField] AudioClip destroy_sound;

    public void TakeDamage(float damage)
    {
        
        hp -= damage;

        if (hp <= 0)
        {
            playerState.instance.destroyItems.Add(gameObject.name);
            AudioManager.instance.CreateSoundAtPosition(destroy_sound, transform.position);
            Destroy(gameObject);
        } else
        {
            AudioManager.instance.CreateSoundAtPosition(hit_sound, transform.position);
        }
    }
}
