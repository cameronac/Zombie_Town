using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class gameManager : MonoBehaviour
{

    private FileHandler dataHandler;
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
    public GameObject crosshair;
    [SerializeField] TextMeshProUGUI objectiveText;
    [SerializeField] GameObject playerDamageFlash;
    [SerializeField] private SaveSO save;

    bool isPaused;
    bool fadeInObjective = false;
    private GameData data;

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
        if (save._isLoaded == true)
        {
            loadGame();
        }

        StartCoroutine(ObjectiveFadeInFadeOut(8));
    }

    //private void OnEnable()
    //{
    //    SceneManager.sceneLoaded += OnSceneLoaded;
    //    SceneManager.sceneUnloaded += OnSceneUnloaded;
    //}

    //private void OnDisable()
    //{
    //    SceneManager.sceneLoaded -= OnSceneLoaded;
    //    SceneManager.sceneUnloaded -= OnSceneUnloaded;
    //}

    //public void OnSceneLoaded(Scene sc, LoadSceneMode mode)
    //{

    //}

    //public void OnSceneUnloaded(Scene sc)
    //{

    //}

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
            crosshair.gameObject.SetActive(false);

        } else {
            mainMenu.SetActive(false);
            ammoTextMesh.gameObject.SetActive(true);
            healthImage.gameObject.SetActive(true);
            staminaImage.gameObject.SetActive(true);
            objectiveText.gameObject.SetActive(true);
            interactText.gameObject.SetActive(true);
            crosshair.gameObject.SetActive(true);
        }

        if (Input.GetButtonDown("Cancel") && activeMenu == null && SceneManager.GetSceneByBuildIndex(0).name != SceneManager.GetActiveScene().name)
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

    public void loadGame()
    {
        if(SceneManager.GetSceneByBuildIndex(0).name != SceneManager.GetActiveScene().name)
        {
            data = SaveSystem.LoadPlayer();

            playerScript.health = data.health;
            playerScript.has_shotgun = data.shotgun;
            playerScript.has_pistol = data.pistol;

            Vector3 position;

            position.x = data.playerPosition[0];
            position.y = data.playerPosition[1];
            position.z = data.playerPosition[2];

            player.transform.position = position;
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
        interactText.enabled = toggle;
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
    //-------------------------------------------
}
