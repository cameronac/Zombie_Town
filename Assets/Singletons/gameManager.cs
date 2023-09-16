using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System.IO;
using UnityEngine.Rendering.UI;
using Unity.VisualScripting;

public class gameManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject creditsMenu;
    public GameObject cred;

    [SerializeField] AudioClip ui_sound;
    [SerializeField] AudioClip lose_sound;

    private FileHandler dataHandler;
    public static gameManager instance;
    public GameObject playerSpawnPos;

    public GameObject player;
    public playerState playerScript;
    public playerShoot p_playerShoot;


    public GameObject activeMenu;
    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject loseMenu;

    public TextMeshProUGUI ammoTextMesh;
    public Image healthImage;
    public Image staminaImage;
    public TextMeshProUGUI interactText;
    public GameObject crosshair;
    [SerializeField] TextMeshProUGUI objectiveText;
    [SerializeField] GameObject playerDamageFlash;
    [SerializeField] private SaveSO save;
    [SerializeField] Image deathAreaImage;

    bool isPaused;
    bool fadeInObjective = false;
    public bool inDeathArea = false;
    private GameData data;

    // Start is called before the first frame update
    void Awake()
    {   
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerScript = player.GetComponent<playerState>();
            p_playerShoot = player.GetComponent<playerShoot>();
        }

        playerSpawnPos = GameObject.FindGameObjectWithTag("Player Spawn Pos");
    }

    private void Start()
    {
        deathAreaImage.color = new Color(0, 0, 0, 0);
        inDeathArea = false;

        if (save._isLoaded == true && File.Exists(Application.persistentDataPath + "/player.dataucannottouch"))
        {
            loadGame();
            save._isLoaded = false;
        }

        StartCoroutine(ObjectiveFadeInFadeOut(8));
    }
    // Update is called once per frame
    void Update()
    {
        if (isPaused)
        {
            interactText.enabled = false;
        }

        FadeInFadeOutObjective();
        UpdateDeathArea();

        if (SceneManager.GetSceneByBuildIndex(1).name == SceneManager.GetActiveScene().name) {
            if (Input.GetButtonDown("Cancel") && activeMenu == null)
            {
                statePaused();
                activeMenu = pauseMenu;
                pauseMenu.SetActive(isPaused);

            } else if (Input.GetButtonDown("Cancel") && activeMenu == pauseMenu) {
                stateUnpaused();
            }
        }
    }

    //Main Menu Methods--------------------------
    public void MainMenuCurrent()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(false);
    }

    public void OptionsMenuCurrent()
    {
        optionsMenu.SetActive(true);
        mainMenu.SetActive(false);
        creditsMenu.SetActive(false);
    }

    public void CreditsMenuCurrent()
    {
        optionsMenu.SetActive(false);
        mainMenu.SetActive(false);
        creditsMenu.SetActive(true);
        //StartCoroutine(credsRoll());
    }
    //-------------------------------------------

    public void SetLoad(bool load)
    {
        save._isLoaded = load;
    }

    public void UpdateDeathArea() {
        deathAreaImage.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);

        float alpha = deathAreaImage.color.a;

        if (inDeathArea)
        {
            alpha = Mathf.MoveTowards(alpha, 1, Time.deltaTime * 0.25f);
            deathAreaImage.color = new Color(0, 0, 0, alpha);  
        } else {
            alpha = Mathf.MoveTowards(alpha, 0, Time.deltaTime * 0.25f);
            deathAreaImage.color = new Color(0, 0, 0, alpha);
        }

        if (inDeathArea) {
            if (deathAreaImage.color == new Color(0, 0, 0, 1.0f) && activeMenu != loseMenu)
            {
                inDeathArea = false;
                youLose();
            }
        }
    }

    public void statePaused()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        isPaused = !isPaused;
        crosshair.gameObject.SetActive(false);
    }

    public void stateUnpaused()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = !isPaused;

        if (activeMenu != null)
        {
            activeMenu.SetActive(false);
        }

        AudioManager.instance.DeleteSoundWithTag("lose");

        activeMenu = null;
        crosshair.gameObject.SetActive(true);
    }

    public void youWin()
    {
        statePaused();
        activeMenu = winMenu;
        activeMenu.SetActive(true);
    }

    public void youLose()
    {
        AudioManager.instance.CreateOneDimensionalSound(lose_sound, 1f, "lose");
        statePaused();
        activeMenu = loseMenu;
        activeMenu.SetActive(true);
    }

    public void loadGame()
    {
        if (SceneManager.GetSceneByBuildIndex(0).name != SceneManager.GetActiveScene().name)
        {
            data = SaveSystem.LoadPlayer();
            if (data != null)
            {
                playerScript.health = data.health;
                playerScript.has_shotgun = data.shotgun;
                playerScript.has_pistol = data.pistol;

                Vector3 position;

                position.x = data.playerPosition[0];
                position.y = data.playerPosition[1];
                position.z = data.playerPosition[2];

                player.transform.position = position;

                playerScript.medCount = data.medCount;

                for (int i = 0; i < data.thingsToDestroy.Count; i++)
                {
                    GameObject killMe = GameObject.Find(data.thingsToDestroy[i]);
                    if (killMe)
                    {
                        playerScript.destroyItems.Add(data.thingsToDestroy[i]);
                        Destroy(killMe);
                    }
                }
            }
        }

    }

    public void saveGame()
    {
        SaveSystem.SavePlayer(playerScript, player);
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
        switch(playerScript.currItem)
        {
            case playerState.heldItems.pistol:
                ammoTextMesh.SetText(magazine.ToString() + " | " + ammo.ToString());
                break;
            case playerState.heldItems.shotgun:
                ammoTextMesh.SetText(magazine.ToString() + " | " + ammo.ToString());
                break;
            case playerState.heldItems.meds:
                ammoTextMesh.SetText(ammo.ToString());
                break;
            case playerState.heldItems.knife:
                ammoTextMesh.SetText("∞");
                break;
        }
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
        if (!isPaused) {
            interactText.enabled = toggle;
        } 
    }
    public void updateObjective(string txt)
    {
        StopCoroutine("ObjectiveFadeInFadeOut");
        objectiveText.SetText("Objective: " + txt);
        StartCoroutine(ObjectiveFadeInFadeOut(3));
    }
    private List<IData> FindAllDataPersistenceObjects()
    {
        IEnumerable<IData> dataPresistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IData>();

        return new List<IData>(dataPresistenceObjects);
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

    IEnumerator credsRoll()
    {
        yield return new WaitForSeconds(0.5f);
        //cred.SetActive(true);
    }
    //-------------------------------------------

}
