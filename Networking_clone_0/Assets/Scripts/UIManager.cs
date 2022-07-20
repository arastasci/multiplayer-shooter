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

    Stack<GameObject> ScoreCards; // store scorecards in this remove add them here
    private int lastPlayerCountInScoreboard;
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
    // when initializing a new client and when a client disconnects, call this method
    public void UpdateScoreboard(List<Utilities.ScoreboardInfo> scoreboardInfos, int playerCount)
    {
        if (playerCount > lastPlayerCountInScoreboard)
        {
            for(int i = 0; i < playerCount - lastPlayerCountInScoreboard; i++)
            CreateScoreItem();
        }
        else if (playerCount < lastPlayerCountInScoreboard)
        {
            for (int i = 0; i < lastPlayerCountInScoreboard - playerCount; i++)
                DeleteScoreItem();
        }
        foreach (Utilities.ScoreboardInfo scoreboardInfo in scoreboardInfos)
        {
            // fill info of scorecards yes okay
            //
            
        }
    }
    void CreateScoreItem()
    {
        //instantiate and add to ScoreCards
        GameObject scoreCard = null;
        ScoreCards.Push(scoreCard);
    }
    void DeleteScoreItem()
    {
        // remove from scorecards and destroy

        Destroy(ScoreCards.Pop());

    }
}
