using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    enum ButtonTypes { enter = 0, options, load, credits, quit }
    private int index = 0;
    private int max_index = 4;

    [SerializeField] private SaveSO save;
    [SerializeField] AudioClip ui_sound;
    [SerializeField] TextMeshProUGUI[] buttonText;

    // Start is called before the first frame update
    void Start()
    {
        ButtonHover(0);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            ButtonPressed(index);
        }

        bool up = Input.GetButtonDown("Up");
        bool down = Input.GetButtonDown("Down");

        if (up || down) {

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
        switch(_index)
        {
            case (int)ButtonTypes.enter:
                save._isLoaded = false;
                gameManager.instance.stateUnpaused();
                SceneManager.LoadScene(1);
                break;

            case (int)ButtonTypes.options:
                gameManager.instance.statePaused();
                gameManager.instance.OptionsMenuCurrent();
                break;

            case (int)ButtonTypes.load:
                save._isLoaded = true;
                gameManager.instance.stateUnpaused();
                SceneManager.LoadScene(1);
                break;

            case (int)ButtonTypes.credits:
                gameManager.instance.statePaused();
                gameManager.instance.CreditsMenuCurrent();
                break;

            case (int)ButtonTypes.quit:
                Application.Quit();
                break;
        }
    }

    public void ButtonHover(int _index)
    {
        AudioManager.instance.CreateOneDimensionalSound(ui_sound, 1, "ui");

        for (int i = 0; i < buttonText.Length; i++)
        {
            if (i == _index)
            {
                buttonText[i].color = Color.white;
            } else {
                buttonText[i].color = Color.red;
            }
        }
    }
    //-------------------------------------------
}
