using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings instance;
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
        PlayerPrefs.SetFloat("volume", 1f);

        AudioListener.volume = PlayerPrefs.GetFloat("volume");

    }

    public void VolumeValueChanged(Single volume)
    {
        PlayerPrefs.SetFloat("volume", volume);
        Debug.Log(PlayerPrefs.GetFloat("volume"));
        AudioListener.volume = PlayerPrefs.GetFloat("volume");
    }
}
