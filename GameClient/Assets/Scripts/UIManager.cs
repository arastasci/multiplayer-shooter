
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
    [SerializeField] private GameObject mainMenu;

    public Slider slider;
    [SerializeField] TextMeshProUGUI healthPoint;
    [SerializeField] TextMeshProUGUI ammoInfo;
    [SerializeField] GameObject hud;
    [SerializeField] TextMeshProUGUI killerDisplay;
    [SerializeField] TextMeshProUGUI speed;
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
    public void ConnectToServer()
    {
        startMenu.SetActive(false);
        RotateWorldInMenu.instance.enabled = false;
        gameUI.SetActive(true);
        usernameField.interactable = false;
        ipField.interactable = false;
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

    public void ToggleMenu()
    {
        mainMenu.SetActive(!mainMenu.activeInHierarchy);
        PlayerController controller = GameManager.players[Client.instance.myId].playerController;
        controller.GetComponentInChildren<CameraController>().enabled =
            !controller.GetComponentInChildren<CameraController>().enabled;
        controller.enabled = !controller.enabled;

    }

    public void ExitGame()
    {
        Client.instance.Disconnect();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.ClearDictionaries();
    }
    
}
