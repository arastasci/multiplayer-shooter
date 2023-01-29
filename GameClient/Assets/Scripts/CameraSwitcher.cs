using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public static CameraSwitcher instance;
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

    private Camera SceneCamera;
    [SerializeField] private Camera playerCamera;
    private void Start()
    {
        SceneCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        SceneCamera.enabled = false;

    }

    public void SwitchCameras()
    {
        playerCamera.enabled = !playerCamera.enabled;
        SceneCamera.enabled = !SceneCamera.enabled;
    }
}
