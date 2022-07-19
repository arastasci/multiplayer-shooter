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

    public static void PlayerMovement(bool[] inputs)
    {
        using(Packet _packet = new Packet((int)ClientPackets.playerMovement))
        {
            _packet.Write(inputs.Length);
            foreach(bool input in inputs)
            {
                _packet.Write(input);
            }
            _packet.Write(GameManager.players[Client.instance.myId].transform.rotation);
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
    #endregion
}
