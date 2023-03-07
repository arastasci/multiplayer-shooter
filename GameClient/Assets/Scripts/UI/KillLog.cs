using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KillLog : MonoBehaviour
{
    private string killer;
    private string victim;
    private Sprite weapon;
    public string Killer
    {
        get => killer;
        set
        {
            killer = value;
            transform.GetChild(0).GetComponent<Text>().text = killer;
        }
    }

    public string Victim
    {
        get => victim;
        set
        {
            victim = value;
            transform.GetChild(2).GetComponent<Text>().text = victim;

        }
    }

    public Sprite Weapon
    {
        get => weapon;
        set
        {
            weapon = value;
            transform.GetChild(1).GetComponent<Image>().sprite = weapon;
        }
    }

    private void Start()
    {
        Destroy(gameObject, 5f);
    }
}
