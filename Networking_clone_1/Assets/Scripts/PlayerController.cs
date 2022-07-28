using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct PlayerInput
{
    public float z;
    public float x;
    public bool isJumping;
}
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    Transform camTransform;
    int weapon = 0;
    bool changedState = false;
    private void Update()
    {
        GetInput();

        if (changedState)
        {
            ClientSend.PlayerChangedWeapon(weapon);
            changedState = false;
        }
        if (Input.GetMouseButtonDown(0))
        {
            ClientSend.PlayerFire(camTransform.forward);
        }
        
    }

    private void FixedUpdate()
    {
        SendInputToServer();
    }
    


    void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && weapon != 0)
        {
            weapon = 0;
            changedState = true;   
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && weapon != 1)
        {
            weapon = 1;
            changedState = true;
        }

        // if prompted to reload
        if (Input.GetKeyDown(KeyCode.R))
        {
            ClientSend.PlayerReload(weapon);
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            UIManager.instance.ShowScoreBoard();
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            UIManager.instance.HideScoreBoard();
        }
    }
    private void SendInputToServer()
    {
        PlayerInput input = new PlayerInput();
        input.z = Input.GetAxisRaw("Vertical");
        input.x = Input.GetAxisRaw("Horizontal");
        input.isJumping = Input.GetKey(KeyCode.Space);
        ClientSend.PlayerGetInput(input);
    }
}
