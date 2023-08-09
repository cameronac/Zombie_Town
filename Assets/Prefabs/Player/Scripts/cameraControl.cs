using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControl : MonoBehaviour
{
    int sensitivity = 2;
    int lockVertMin = -90;
    int lockVertMax = 90;

    [SerializeField] bool invertY = false;

    float xRotation;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        bool isPaused = false;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;

        if (invertY)
        {
            xRotation += mouseY;
        }
        else
        {
            xRotation -= mouseY;
        }

        xRotation = Mathf.Clamp(xRotation, lockVertMin, lockVertMax);


        if (gameManager.instance != null)
        {
            if (gameManager.instance.isGamePaused())
            {
                isPaused = true;
            }
        }

        if (!isPaused)
        {
            transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            transform.parent.Rotate(Vector3.up * mouseX);
        }
    }
}
