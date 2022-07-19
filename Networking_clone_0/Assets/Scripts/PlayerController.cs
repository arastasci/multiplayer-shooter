using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
    private void SendInputToServer()
    {
        bool[] inputs = new bool[]
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D),
            Input.GetKey(KeyCode.Space),
        };
        ClientSend.PlayerMovement(inputs);
    }
}
