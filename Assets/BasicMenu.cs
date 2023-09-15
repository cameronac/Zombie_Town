using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BasicMenu : MonoBehaviour
{
    private int index = 0;
    public int max_index = 0;

    [SerializeField] AudioClip ui_sound;
    [SerializeField] TextMeshProUGUI[] buttonText;

    // Start is called before the first frame update
    void Start()
    {
        ButtonHover(0);
    }

    private void Update()
    {
        bool up = Input.GetButtonDown("Up");
        bool down = Input.GetButtonDown("Down");

        if (up || down)
        {
            if (up)
            {
                index -= 1;
            }

            if (down)
            {
                index += 1;
            }

            if (index < 0)
            {
                index = max_index;
            }

            if (index > max_index)
            {
                index = 0;
            }

            ButtonHover(index);
        }
    }

    //Events-------------------------------------
    public void ButtonHover(int _index)
    {
        index = _index;

        for (int i = 0; i < buttonText.Length; i++)
        {
            if (i == _index)
            {
                if (buttonText[i].color != Color.white)
                {
                    AudioManager.instance.CreateOneDimensionalSound(ui_sound, 1, "ui");
                }

                buttonText[i].color = Color.white;
            }
            else
            {
                buttonText[i].color = Color.red;
            }
        }
    }
    //-------------------------------------------
}
