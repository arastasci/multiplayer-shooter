using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerManager player;
    public float sensitivity = 100f;
    public float clampAngle = 90f;
    private float verticalRotation;
    private float horizontalRotation;
    private bool cursorLocked = false;
    Vector3 basePosition;
    PlayerManager killer;
    [SerializeField] Vector3 killCamOffset = new Vector3(0,1f,2f);
    [SerializeField] Quaternion killCamAngle = Quaternion.Euler(-30, 0, 0);
    private void Start()
    {
        verticalRotation = transform.localEulerAngles.x;
        horizontalRotation = player.transform.eulerAngles.y;
        basePosition = transform.localPosition;
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ToggleCursorMode();
        }
        if (cursorLocked)
        {
            Look();
        }

        
        Debug.DrawRay(transform.position, transform.forward * 2, Color.red);
    }

    public void LockToKiller(int playerID)
    {
        killer = GameManager.players[playerID];
        transform.parent = killer.transform;
        transform.localRotation = killCamAngle;
        transform.localPosition = killCamOffset;
    }
    public void GoBackToPlayer()
    {
        killer = null;
        transform.parent = player.transform;
        transform.localRotation = Quaternion.identity;
        transform.localPosition = basePosition;
    }

    private void Look()
    {
        float _mouseVertical = -Input.GetAxis("Mouse Y");
        float _mouseHorizontal = Input.GetAxis("Mouse X");

        verticalRotation += _mouseVertical * sensitivity * Time.deltaTime;
        horizontalRotation += _mouseHorizontal * sensitivity * Time.deltaTime;

        verticalRotation = Mathf.Clamp(verticalRotation, -clampAngle, clampAngle);

        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        player.transform.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);
    }
    private void ToggleCursorMode(bool flag = true)
    {
        
        Cursor.visible = !Cursor.visible;
        if (Cursor.lockState == CursorLockMode.None && flag)
        {
            Cursor.lockState = CursorLockMode.Locked;
            cursorLocked = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            cursorLocked = false;
        }
    }

    private void OnDisable()
    {
        ToggleCursorMode(false);
    }
}
