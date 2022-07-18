using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    Transform camTransform;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ClientSend.PlayerShoot(camTransform.forward);
        }
        if (Input.GetMouseButtonDown(1))
        {
            ClientSend.PlayerLaunchProjectile(camTransform.forward);
        }
    }

    private void FixedUpdate()
    {
        SendInputToServer();
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
