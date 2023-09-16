using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void Enter()
    {
        gameManager.instance.SetLoad(false);
        gameManager.instance.stateUnpaused();
        SceneManager.LoadScene(1);
    }

    public void Options()
    {
        gameManager.instance.statePaused();
        gameManager.instance.OptionsMenuCurrent();

    }

    public void Load()
    {
        gameManager.instance.SetLoad(true);
        gameManager.instance.stateUnpaused();
        SceneManager.LoadScene(1);
    }

    public void Respawn()
    {
        gameManager.instance.stateUnpaused();
        gameManager.instance.playerScript.Respawn();
    }

    public void Credits()
    {
        gameManager.instance.statePaused();
        gameManager.instance.CreditsMenuCurrent();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpaused();
    }

    public void MainMenu() {
        SceneManager.LoadScene(0);
        gameManager.instance.statePaused();
    }

    public void Resume()
    {
        gameManager.instance.stateUnpaused();
    }

    public void Back()
    {
        gameManager.instance.MainMenuCurrent();
    }
    
    public void Save() {
        gameManager.instance.saveGame();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
