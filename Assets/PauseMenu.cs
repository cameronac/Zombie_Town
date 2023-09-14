using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    enum ButtonTypes { resume = 0, restart, save, quit, mainmenu }
    private int index = 0;
    private int max_index = 4;

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
            case (int)ButtonTypes.resume:
                gameManager.instance.stateUnpaused();
                break;

            case (int)ButtonTypes.restart:
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                gameManager.instance.stateUnpaused();
                break;

            case (int)ButtonTypes.save:
                gameManager.instance.saveGame();
                break;

            case (int)ButtonTypes.quit:
                Application.Quit();
                break;

            case (int)ButtonTypes.mainmenu:
                SceneManager.LoadScene(0);
                gameManager.instance.statePaused();
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
