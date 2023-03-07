using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }

    #region Packets
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(UIManager.instance.usernameField.text);
            SendTCPData(_packet);
        }
    }

    public static void PlayerGetInput(PlayerInput playerInput)
    {
        using(Packet _packet = new Packet((int)ClientPackets.playerInput))
        {
            _packet.Write(playerInput.z);
            _packet.Write(playerInput.x);
            _packet.Write(playerInput.isJumping);
            _packet.Write(playerInput.isCrouching);
            _packet.Write(GameManager.players[Client.instance.myId].transform.rotation);
            _packet.Write(GameManager.players[Client.instance.myId].cameraController.transform.rotation);
            SendUDPData(_packet);
        }
    }
    public static void PlayerChangedWeapon(int weapon)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerChangedWeapon))
        {
            packet.Write(weapon);
            SendTCPData(packet);
        }
    }
    public static void PlayerReload(int weapon)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerReload))
        {
            packet.Write(weapon);
            SendTCPData(packet);
        }
    }

    public static void PlayerFire(Vector3 facing)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerFire))
        {
            
            packet.Write(facing);
            SendTCPData(packet);
        }
    }

    public static void Ping()
    {
        using (Packet packet = new Packet((int)(ClientPackets.ping)))
        {
            SendTCPData(packet);
        }
    }

    public static void ClientPing(int ping)
    {
        using (Packet packet = new Packet((int)(ClientPackets.clientPing)))
        {
            packet.Write(ping);
            SendTCPData(packet);
        }
    }
    #endregion
}
