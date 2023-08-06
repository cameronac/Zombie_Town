using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponSway : MonoBehaviour
{
    float hIntensity = 8.0f;
    float vIntensity = 15.0f;
    float smoothing = 15f;
    Quaternion restRotation;

    public void Start()
    {
        restRotation = transform.localRotation;
    }

    public void Update()
    {
        float mouseX = -Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        Quaternion xAxis = Quaternion.AngleAxis(hIntensity * mouseX, Vector3.up);
        Quaternion yAxis = Quaternion.AngleAxis(vIntensity * mouseY, Vector3.right);
        Quaternion newRotation = restRotation * xAxis * yAxis;

        transform.localRotation = Quaternion.Lerp(transform.localRotation, newRotation, Time.deltaTime * smoothing);
    }
}
