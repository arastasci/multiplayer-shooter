using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject startMenu;
    public InputField usernameField;
    public GameObject gameUI;
    public Slider slider;
    [SerializeField] TextMeshProUGUI healthPoint;
    [SerializeField] TextMeshProUGUI ammoInfo;
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
    private void Start()
    {
        
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
}
