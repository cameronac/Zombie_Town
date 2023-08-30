using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    public GameObject playerSpawnPos;

    public GameObject player;
    public playerState playerScript;

    public GameObject activeMenu;
    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject loseMenu;
    public GameObject mainMenu;
    public TextMeshProUGUI ammoTextMesh;
    public Image healthImage;
    public Image staminaImage;
    public TextMeshProUGUI interactText;
    [SerializeField] TextMeshProUGUI objectiveText;

    bool isPaused;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerScript = player.GetComponent<playerState>();
        }
        playerSpawnPos = GameObject.FindGameObjectWithTag("Player Spawn Pos");
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetSceneByBuildIndex(0).name == SceneManager.GetActiveScene().name) {
            mainMenu.SetActive(true);
            ammoTextMesh.enabled = false;
            healthImage.enabled = false;
            staminaImage.enabled = false;
            objectiveText.enabled = false;
        } else {
            mainMenu.SetActive(false);
            ammoTextMesh.enabled = true;
            healthImage.enabled = true;
            staminaImage.enabled = true;
            objectiveText.enabled = true;
        }

        if (Input.GetButtonDown("Cancel") && activeMenu == null)
        {
            statePaused();
            activeMenu = pauseMenu;
            pauseMenu.SetActive(isPaused);
        }
    }

    public void statePaused()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        isPaused = !isPaused;
    }

    public void stateUnpaused()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = !isPaused;
        if(activeMenu != null)
        {
            activeMenu.SetActive(false);
        }

        activeMenu = null;
    }

    public void youWin()
    {
        statePaused();
        activeMenu = winMenu;
        activeMenu.SetActive(true);
    }

    public void youLose()
    {
        statePaused();
        activeMenu = loseMenu;
        activeMenu.SetActive(true);
    }

    //Update User Interface
    public void SetAmmo(int magazine, int ammo)
    {
        ammoTextMesh.SetText(magazine.ToString() + " | " + ammo.ToString());
    }

    public void SetHealth(float health)
    {
        healthImage.fillAmount = health;
    }

    public void SetStamina(float stamina)
    {
        staminaImage.fillAmount = stamina;
    }

    public void ToggleInteract(bool toggle)
    {
        interactText.enabled = toggle;
    }
    public void updateObjective(string txt)
    {
        
            objectiveText.SetText("Objective: " + txt);
    }
       

    //Getters
    public bool isGamePaused()
    {
        return isPaused;
    }
}
