using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{

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
        SceneManager.LoadScene(1);
    }
    public void LoadLevel()
    {
        gameManager.instance.stateUnpaused();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
