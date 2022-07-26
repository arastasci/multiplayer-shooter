
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject startMenu;
    public InputField usernameField;
    public GameObject gameUI;
    public GameObject scoreBoard;
    public Slider slider;
    [SerializeField] TextMeshProUGUI healthPoint;
    [SerializeField] TextMeshProUGUI ammoInfo;
    [SerializeField] GameObject hud;
    [SerializeField] TextMeshProUGUI killerDisplay;

    string[] killStrings = { "pwned you!","chicago sunroof'ed you","married your mother"};
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
        Destroy(RotateWorldInMenu.instance);
        gameUI.SetActive(true);
        usernameField.interactable = false;
        Client.instance.ConnectToServer();
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
        killerDisplay.gameObject.SetActive(true);
        killerDisplay.text = String.Format("{0} {1}",killerName,)

    }
    
}
