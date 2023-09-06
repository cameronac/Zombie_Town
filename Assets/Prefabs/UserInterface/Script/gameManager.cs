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
    [SerializeField] GameObject playerDamageFlash;

    bool isPaused;
    bool fadeInObjective = false;

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


    private void Start()
    {
        StartCoroutine(ObjectiveFadeInFadeOut(8));
    }

    // Update is called once per frame
    void Update()
    {
        //print("Main Menu: " + (SceneManager.GetSceneByBuildIndex(0).name == SceneManager.GetActiveScene().name).ToString());
        
        FadeInFadeOutObjective();
        
        if (SceneManager.GetSceneByBuildIndex(0).name == SceneManager.GetActiveScene().name) {
            mainMenu.SetActive(true);
            ammoTextMesh.gameObject.SetActive(false);
            healthImage.gameObject.SetActive(false);
            staminaImage.gameObject.SetActive(false);
            objectiveText.gameObject.SetActive(false);
            interactText.gameObject.SetActive(false);

        } else {
            mainMenu.SetActive(false);
            ammoTextMesh.gameObject.SetActive(true);
            healthImage.gameObject.SetActive(true);
            staminaImage.gameObject.SetActive(true);
            objectiveText.gameObject.SetActive(true);
            interactText.gameObject.SetActive(true);
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
    public IEnumerator playerFlashDamage()
    {
        playerDamageFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        playerDamageFlash.SetActive(false);
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
        StopCoroutine("ObjectiveFadeInFadeOut");
        objectiveText.SetText("Objective: " + txt);
        StartCoroutine(ObjectiveFadeInFadeOut(3));
    }
 

    //Getters
    public bool isGamePaused()
    {
        return isPaused;
    }

    private void FadeInFadeOutObjective()
    {
        if (fadeInObjective) {
            objectiveText.color = Color.Lerp(objectiveText.color, new Color(1, 1, 1, 1), Time.deltaTime * 5);
        } else {
            objectiveText.color = Color.Lerp(objectiveText.color, new Color(1, 1, 1, 0), Time.deltaTime * 5);
        }
    }


    //Enumerators--------------------------------
    IEnumerator ObjectiveFadeInFadeOut(int time) {

        //Fade In
        fadeInObjective = true;
        
        yield return new WaitForSeconds(time);

        //Fade Out
        fadeInObjective = false;
    }
    //-------------------------------------------
}
