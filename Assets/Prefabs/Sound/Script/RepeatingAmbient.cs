using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatingAmbient : MonoBehaviour
{
    private AudioSource s;
    private float starting_volume;
    private float custom_volume;

    void Start()
    {
        s = GetComponent<AudioSource>();
        custom_volume = 0;
        s.volume = 0;
        StartCoroutine(play(true, s, 2f, .1f));
        StartCoroutine(play(false, s, 2f, 0f));
    }

    private void Update()
    {
        if(!s.isPlaying)
        {
            s.Play();
            StartCoroutine(play(true, s, 2f, .1f));
            StartCoroutine(play(false, s, 2f, 0f));
        }

        s.volume = AudioManager.music_volume * custom_volume;
    }

    public IEnumerator play(bool fade, AudioSource s, float dur, float volume)
    {
        if(!fade)
        {
            double len = (double)s.clip.samples / s.clip.frequency;
            yield return new WaitForSecondsRealtime((float)len - dur);
        }

        float time = 0f;
        float startVol = s.volume;

        while(time < dur)
        {
            time += Time.deltaTime;
            custom_volume = Mathf.Lerp(startVol, volume, time / dur);
            yield return null;
        }

        yield break;
    }
}
