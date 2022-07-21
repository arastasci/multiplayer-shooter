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
        }
    }
    public void SetHealth(int health)
    {
        this.health = health;
        if(id == Client.instance.myId)
        {

            UIManager.instance.UpdateHealth(health);
        }
        if(health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        model.enabled = false;
    }
    public void Respawn()
    {
        model.enabled = true;
        SetHealth(maxHealth);
    }

    public void SetActiveWeapon(int id)
    {
        weapons[activeWeapon].SetActive(false);
        activeWeapon = id;
        weapons[activeWeapon].SetActive(true);
    }
}
