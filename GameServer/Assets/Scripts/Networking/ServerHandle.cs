using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class consists of <seealso cref="Server.PacketHandler"/> methods.
/// </summary>
public class ServerHandle
{
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();

        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} " +
            $"connected and is now player {_fromClient}.");
        if (_fromClient != _clientIdCheck)
        {
            Debug.Log($"Player {_username}(ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})");
        }
        Server.clients[_fromClient].SendIntoGame(_username);
    }
    public static void PlayerGetInput(int _fromClient, Packet packet)
    {
        PlayerInput playerInput = new PlayerInput();
        playerInput.z = packet.ReadFloat();
        playerInput.x = packet.ReadFloat();
        playerInput.isJumping = packet.ReadBool();
        playerInput.isCrouching = packet.ReadBool();
        Quaternion rotation = packet.ReadQuaternion();
        Quaternion weaponRotation = packet.ReadQuaternion();
        Server.clients[_fromClient].player.SetInput(playerInput, rotation, weaponRotation);
        
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

    public static void Ping(int fromClient, Packet packet){
        ServerSend.Ping(fromClient);
    }
    /// <summary>
    /// Gets the ping measured by the client and sends it to all clients for it to be displayed in the scoreboard.
    /// </summary>
    /// <param name="fromClient"></param>
    /// <param name="packet"></param>
    public static void GetAndSendPing(int fromClient, Packet packet)
    {
        int ping = packet.ReadInt();
        ServerSend.OthersPing(fromClient, ping);
    }

}
