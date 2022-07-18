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
    public static void PlayerShoot(Vector3 facing)
    {
        using(Packet packet = new Packet((int)(ClientPackets.playerShoot)))
        {
            packet.Write(facing);
            SendTCPData(packet);
        }
    }
    public static void PlayerLaunchProjectile(Vector3 facing)
    {
        using(Packet packet = new Packet((int)ClientPackets.playerLaunchProjectile))
        {
            packet.Write(facing);
            SendTCPData(packet);
        }
    }
    #endregion
}
