using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;
    public int health;
    public int maxHealth;
    public MeshRenderer model;
    [SerializeField] GameObject[] weapons = new GameObject[2];
    PlayerController playerController;
    CamFollowKiller camFollowKiller;
    public ScoreData score;

    int activeWeapon = 1;
    public void Initialize(int id, string userName)
    {
        this.id = id;
        username = userName;
        health = maxHealth;
        if (id == Client.instance.myId) 
        {
            UIManager.instance.slider.maxValue = maxHealth;
            UIManager.instance.UpdateHealth(maxHealth);
            playerController = GetComponent<PlayerController>();
            camFollowKiller = GetComponent<CamFollowKiller>();
        }
        score = new ScoreData(id, 0, 0);
    }
    public void SetHealth(int health, int byPlayer)
    {
        this.health = health;
        if(id == Client.instance.myId)
        {
            UIManager.instance.UpdateHealth(health);
        }
        if(health <= 0)
        {
            Die(byPlayer);
        }
    }

    public void Die(int byPlayer)
    {
        model.enabled = false;
        weapons[activeWeapon].SetActive(false);
        playerController.enabled = false;
        camFollowKiller.enabled = true;
        // make cam follow killer CM
        UI.instance. GameManager.players[byPlayer].username

    }
    public void Respawn()
    {
        model.enabled = true;
        weapons[activeWeapon].SetActive(true);
        playerController.enabled = true;
        camFollowKiller.enabled = false;
        SetHealth(maxHealth, id); // id totally useless here, should refactor the code later
    }

    public void SetActiveWeapon(int id)
    {
        weapons[activeWeapon].SetActive(false);
        activeWeapon = id;
        weapons[activeWeapon].SetActive(true);
    }
}
