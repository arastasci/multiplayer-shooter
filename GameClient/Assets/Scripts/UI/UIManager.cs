
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject startMenu;
    public InputField usernameField;
    public InputField ipField;
    public GameObject gameUI;
    public GameObject scoreBoard;
    [SerializeField] public GameObject mainMenu;
    [SerializeField] private Transform killLog;
    [SerializeField] private GameObject killPrefab;
    public Slider slider;
    [SerializeField] TextMeshProUGUI healthPoint;
    [SerializeField] TextMeshProUGUI ammoInfo;
    [SerializeField] GameObject hud;
    [SerializeField] TextMeshProUGUI killerDisplay;
    [SerializeField] TextMeshProUGUI speed;

    [SerializeField] private Sprite rocketImage;
    [SerializeField] private Sprite pistolImage;
    [SerializeField] private Sprite skullImage;
    
    string[] killStrings = { "pwned you","rizzed you","fragged you" };
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists.");
            Destroy(this);
        }
        
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            instance.ToggleMenu();
        }
    }

    public void UpdateHealth(int health)
    {
        healthPoint.text = health.ToString();
        slider.value = health;

    }
    public void UpdateAmmoInfo(int ammoInMag, int ammoTotal)
    {
        ammoInfo.text = ammoInMag.ToString() + " / " + ammoTotal.ToString();
    }
    /// <summary>
    /// Connected to a Unity Event that connects the player to the server with the IP:Port in the input field.
    /// </summary>
    public void ConnectToServer()
    {
        startMenu.SetActive(false);
        RotateWorldInMenu.instance.enabled = false;
        gameUI.SetActive(true);
        usernameField.interactable = false;
        ipField.interactable = false;
        if (ipField.text == "t") // for testing locally
        {
            Client.instance.ConnectToServer("127.0.0.1:26950");
        }else
        Client.instance.ConnectToServer(ipField.text);
    }
    public void UpdateSpeed(float speed)
    {
        this.speed.text = String.Format("{0:0.##}", speed);
   //     Debug.Log(speed);
    }
    public void ShowScoreBoard()
    {
        scoreBoard.SetActive(true);
    }
    public void HideScoreBoard()
    {
        scoreBoard.SetActive(false);
    }
    
    public void DisplayKiller(string killerName)
    {
        hud.SetActive(false);
        killerDisplay.enabled = true;
        killerDisplay.text = String.Format("{0} {1}!", killerName,killStrings[UnityEngine.Random.Range(0,killStrings.Length)]);
        
    }

    public void DisplaySelfKill()
    {
        hud.SetActive(false);
    }
    public void HideKiller()
    {
        hud.SetActive(true);
        killerDisplay.text = "";
        killerDisplay.enabled = false;
        
    }

    public void ToggleMenu(bool gameStarts = false)
    {
     
        mainMenu.SetActive(!mainMenu.activeInHierarchy);
        if (PlayerManager.isDead) return;
        PlayerController controller = GameManager.players[Client.instance.myId].playerController;
        controller.GetComponentInChildren<CameraController>().enabled =
            !controller.GetComponentInChildren<CameraController>().enabled;
        controller.enabled = !controller.enabled;

    }

    public void LogKill(int killer, int victim, int weapon)
    {
        KillLog killRow = Instantiate(killPrefab, killLog).GetComponent<KillLog>();
        killRow.Killer = 
            GameManager.players[killer]
            .username;
        killRow.Victim = GameManager.players[victim].username;
        if(killer == victim)
        {
            killRow.Weapon = skullImage;
            return;
        }
        switch (weapon)
        {
            case 0:
                killRow.Weapon = pistolImage;
                break;
            case 1:
                killRow.Weapon = rocketImage;
                break;
        }
    }

    public void ExitGame()
    {
        Client.instance.Disconnect();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.ClearDictionaries();
    }
    
}
