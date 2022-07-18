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
    public void Initialize(int id, string userName)
    {
        this.id = id;
        username = userName;
        health = maxHealth;
        if (id == Client.instance.myId) 
        {
            UIManager.instance.textMeshPro.text = maxHealth.ToString(); 
        }
    }
    public void SetHealth(int health)
    {
        this.health = health;
        if(id == Client.instance.myId)
        {
            UIManager.instance.textMeshPro.text = health.ToString();
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
}
