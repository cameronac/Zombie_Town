using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerCamControls : MonoBehaviour
{
    [SerializeField] int sensitivity;

    //how far the camera can rotate - usually 90/90
    [SerializeField] int lockVertMin;
    [SerializeField] int lockVertMax;

    [SerializeField] bool invertY;

    float xRotation;

    void Start()
    {
        //turn cursor off and lock to where it won't move
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        //get input
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;

        if (invertY)
            xRotation += mouseY;
        else
            xRotation -= mouseX;

        //clamp the camera rotation on the x-axis
        xRotation = Mathf.Clamp(xRotation, lockVertMin, lockVertMax);

        //rotate the camera on the x-axis
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        //rotate the player on the y-axis
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}

