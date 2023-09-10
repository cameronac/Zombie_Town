using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    enum ButtonTypes { back = 0 }
    private int index = 0;
    private int max_index = 0;

    [SerializeField] Slider musicSlider;
    [SerializeField] Slider uiSlider;
    [SerializeField] Slider sfxSlider;

    [SerializeField] AudioClip ui_sound;
    [SerializeField] TextMeshProUGUI[] buttonText;

    // Start is called before the first frame update
    void Start()
    {
        ButtonHover(0);
    }

    private void Update()
    {
        musicSlider.value = AudioManager.music_volume;
        uiSlider.value = AudioManager.ui_volume;
        sfxSlider.value = AudioManager.sfx_volume;

        if (Input.GetButtonDown("Submit"))
        {
            ButtonPressed(index);
        }

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
    public void ButtonPressed(int _index)
    {
        switch (_index)
        {
            case (int)ButtonTypes.back:
                gameManager.instance.MainMenuCurrent();
                break;
        }
    }

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
