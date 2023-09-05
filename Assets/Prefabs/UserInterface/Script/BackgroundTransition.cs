using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundTransition : MonoBehaviour
{
    [SerializeField] List<Texture> images;
    RawImage backgroundImage;
    int previousIndex = 0;
    bool fadeIn = false;

    // Start is called before the first frame update
    void Start()
    {
        backgroundImage = GetComponent<RawImage>();
        StartCoroutine(FadeInFadeOut());
    }

    private void Update()
    {
        if (fadeIn)
        {
            backgroundImage.color = Color.Lerp(backgroundImage.color, new Color(0, 0, 0, 1), Time.deltaTime * 2);
        }
        else
        {
            backgroundImage.color = Color.Lerp(backgroundImage.color, new Color(1, 1, 1, 1), Time.deltaTime * 2);
        }
    }

    IEnumerator FadeInFadeOut()
    {
        yield return new WaitForSeconds(5);
        fadeIn = true;

        yield return new WaitForSeconds(3);
        fadeIn = false;

        if (images.Count > 0) {
            int new_index = previousIndex;

            do
            {
                new_index = Random.Range(0, images.Count);
            } while (previousIndex == new_index);

            previousIndex = new_index;
            backgroundImage.texture = images[previousIndex];
        }

        StartCoroutine(FadeInFadeOut());
    }


}
