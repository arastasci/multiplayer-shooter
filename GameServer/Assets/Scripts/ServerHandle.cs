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
    public static void PlayerMovement(int _fromClient, Packet packet)
    {
        bool[] inputs = new bool[packet.ReadInt()];
        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = packet.ReadBool();
        }
        Quaternion rotation = packet.ReadQuaternion();
        Server.clients[_fromClient].player.SetInput(inputs, rotation);
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
}
