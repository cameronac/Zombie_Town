using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door : MonoBehaviour, IInteract
{
    [SerializeField] AudioClip door_sound;
    [SerializeField] GameObject lock_object;
    [SerializeField] bool isLocked = false;
    BoxCollider boxCollider;

    bool isOpen = false;
    Quaternion startRotation;
    bool isTouching = false;
    bool rotateOther = false;

    public void pressed()
    {
        if (!isLocked) {
            isOpen = !isOpen;
            AudioManager.instance.CreateSoundAtPosition(door_sound, transform.position);

            if (isTouching)
            {
                rotateOther = true;
            } else
            {
                rotateOther = false;
            }
        }
    }

    //Built In Methods---------------------------
    void Start()
    {
        if (!isLocked)
        {
            Destroy(lock_object);
        }

        startRotation = transform.rotation;
        boxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (lock_object == null)
        {
            isLocked = false;
        }

        if (isOpen) {
            if (!rotateOther)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, startRotation * Quaternion.Euler(new Vector3(0, -90, 0)), Time.deltaTime * 5);
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, startRotation * Quaternion.Euler(new Vector3(0, 90, 0)), Time.deltaTime * 5);
            }
            
        } else {
            transform.rotation = Quaternion.Lerp(transform.rotation, startRotation, Time.deltaTime * 5);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            isTouching = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isTouching = false;
        }
    }
    //-------------------------------------------
}
