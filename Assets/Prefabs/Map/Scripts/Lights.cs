using System.Collections;
using UnityEngine;

public class Lights : MonoBehaviour
{
    [SerializeField] float flickerIntensity = 10;
    [SerializeField] float flickerRate = 1;

    private Light _light;
    private bool _flicker = true;

    private void Start()
    {
        _light = GetComponent<Light>();
        StartCoroutine(Flicker());
    }

    private IEnumerator Flicker()
    {
        while (true)
        {
            float flickerAmount = Random.Range(-flickerIntensity, flickerIntensity);
            float flickerTime = Random.Range(0, flickerRate);

            float newIntensity = Mathf.Clamp(_light.intensity + flickerAmount, 0, 10);
            _light.intensity = newIntensity;

            _flicker = !_flicker;

            yield return new WaitForSeconds(flickerTime);
        }
    }

    public void ToggleLight()
    {
        _light.enabled = !_light.enabled;
    }
}