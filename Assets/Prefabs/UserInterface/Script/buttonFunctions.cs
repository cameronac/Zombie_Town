using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    [SerializeField] private SaveSO save;

    public void resume()
    {
        gameManager.instance.stateUnpaused();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpaused();
    }

    public void quit()
    {
        Application.Quit();
    }


    public void playerRespawn()
    {
        gameManager.instance.stateUnpaused();
        gameManager.instance.playerScript.Respawn();
    }

    public void playButton()
    {
        /*save._isLoaded = false;
        gameManager.instance.stateUnpaused();
        SceneManager.LoadScene(1);*/
    }
    public void LoadLevel()
    {
        /*
        gameManager.instance.stateUnpaused();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);*/
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
        gameManager.instance.statePaused();
    }

    public void BackButton()
    {
        gameManager.instance.MainMenuCurrent();
    }

    public void optionMenu()
    {
        gameManager.instance.statePaused();
        gameManager.instance.OptionsMenuCurrent();
    }

    public void creditMenu()
    {
        gameManager.instance.statePaused();
        SceneManager.LoadScene(2);
    }
   public void saveGame()
    {
        gameManager.instance.saveGame();
    }
    public void loadGame()
    {
        save._isLoaded = true;
        gameManager.instance.stateUnpaused();
        SceneManager.LoadScene(1);
    }

}
