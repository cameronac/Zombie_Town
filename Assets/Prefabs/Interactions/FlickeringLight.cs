using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{

    enum LightSpeed { slow, medium, fast }

    [SerializeField] LightSpeed lightOnSpeed = LightSpeed.medium;
    [SerializeField] LightSpeed lightOffSpeed = LightSpeed.medium;

    Vector2 slow_speed = new Vector2(0.3f, 2f);
    Vector2 medium_speed = new Vector2(0.2f, 1f);
    Vector2 fast_speed = new Vector2(0.1f, 0.5f);

    Light light;

    Vector2 flickerOnSpeed;
    Vector2 flickerOffSpeed;

    //Default Methods----------------------------
    void Start()
    {
        if (TryGetComponent<Light>(out light))
        {
            StartCoroutine(FlickerLight());
        } else
        {
            print("Couldn't Find Light Component in FlickerLight Script!");
        }

        flickerOnSpeed = GetFlickerSpeed(lightOnSpeed);
        flickerOffSpeed = GetFlickerSpeed(lightOffSpeed);
    }

    private Vector2 GetFlickerSpeed(LightSpeed _speed)
    {
        Vector2 new_speed = Vector2.zero;

        switch (_speed)
        {
            case LightSpeed.slow:
                new_speed = slow_speed;
                break;

            case LightSpeed.medium:
                new_speed = medium_speed;
                break;

            case LightSpeed.fast:
                new_speed = fast_speed;
                break;
        }

        return new_speed;
    }



    //-------------------------------------------

    IEnumerator FlickerLight()
    {
        //Off
        light.enabled = false;
        yield return new WaitForSeconds(Random.Range(flickerOnSpeed.x, flickerOnSpeed.y));

        //On
        light.enabled = true;
        yield return new WaitForSeconds(Random.Range(flickerOffSpeed.x, flickerOffSpeed.y));

        StartCoroutine(FlickerLight());
    }

}
