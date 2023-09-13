using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemSway : MonoBehaviour
{
    [SerializeField] float smoothing;
    [SerializeField] public float intesity;

    void Update()
    {
        // getting raw data
        float x = Input.GetAxisRaw("Mouse X") * intesity;
        float y = Input.GetAxisRaw("Mouse Y") * intesity;
        
        //get the position of where the object should go
        Quaternion moveX = Quaternion.AngleAxis(-y, Vector3.right);
        Quaternion moveY = Quaternion.AngleAxis(x, Vector3.up);
        
        //put both x and y movement together
        Quaternion moveBoth = moveX * moveY;

        //give the trasform actual movement
        Quaternion movement = Quaternion.Slerp(transform.localRotation, moveBoth, smoothing * Time.deltaTime);

        transform.localRotation = movement;
    }
}
