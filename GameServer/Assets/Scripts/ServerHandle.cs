using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();

        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected and is now player {_fromClient}.");
        if (_fromClient != _clientIdCheck)
        {
            Debug.Log($"Player {_username}(ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})");
        }
        Server.clients[_fromClient].SendIntoGame(_username);
        // send player into game
    }
    public static void PlayerGetInput(int _fromClient, Packet packet)
    {
        PlayerInput playerInput = new PlayerInput();
        playerInput.z = packet.ReadFloat();
        playerInput.x = packet.ReadFloat();
        playerInput.isJumping = packet.ReadBool();
        playerInput.isCrouching = packet.ReadBool();
        Quaternion rotation = packet.ReadQuaternion();
        Server.clients[_fromClient].player.SetInput(playerInput, rotation);
    }
    public static void PlayerShoot(int _fromClient, Packet packet)
    {
        Vector3 direction = packet.ReadVector3();

        Server.clients[_fromClient].player.Shoot(direction);
    }

    public static void PlayerLaunchProjectile(int _fromClient, Packet packet)
    {
        Vector3 direction = packet.ReadVector3();
        Server.clients[_fromClient].player.LaunchProjectile(direction);
    }
    
    public static void PlayerFire(int fromClient, Packet packet)
    {
        Vector3 direction = packet.ReadVector3();
        Server.clients[fromClient].player.Fire(direction);   
    }
    public static void PlayerReload(int fromClient, Packet packet)
    {
        Server.clients[fromClient].player.Reload();
    }
    public static void PlayerChangedWeapon(int fromClient, Packet packet)
    {
        int newWeaponID = packet.ReadInt();
        Server.clients[fromClient].player.SetActiveWeapon(newWeaponID);
    }
    

}
