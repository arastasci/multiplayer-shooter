using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend
{
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }

    private static void SendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].udp.SendData(_packet);
    }

    private static void SendUDPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].udp.SendData(_packet);
        }
    }
    private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (_exceptClient == i) continue; // i did it lol
            Server.clients[i].udp.SendData(_packet);
        }
    }


    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(_packet);
        }
    }
    private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (_exceptClient == i) continue; // i did it lol
            Server.clients[i].tcp.SendData(_packet);
        }
    }

    public static void Welcome(int _toClient, string _msg)
    {
        using (Packet packet = new Packet((int)ServerPackets.welcome))
        {
            packet.Write(_msg);
            packet.Write(_toClient);
            SendTCPData(_toClient, packet);
        }
    }
    public static void SpawnPlayer(int _toClient, Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            packet.Write(player.id);
            packet.Write(player.username);
            packet.Write(player.transform.position);
            packet.Write(player.transform.rotation);
            SendTCPData(_toClient, packet);
        }
    }

    public static void PlayerPosition(Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerPosition))
        {
            packet.Write(player.id);
            packet.Write(player.transform.position);

            SendUDPDataToAll(packet);
        }
    }
    public static void PlayerRotation(Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerRotation))
        {
            packet.Write(player.id);
            packet.Write(player.transform.rotation);

            SendUDPDataToAll(player.id, packet);
        }
    }
    public static void PlayerDisconnected(int playerID)
    {
        using(Packet packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            packet.Write(playerID);
            SendTCPDataToAll(packet);
        }
    }
    public static void PlayerHealth(Player player)
    {
        using(Packet packet = new Packet((int)ServerPackets.playerHealth))
        {
            packet.Write(player.id);
            packet.Write(player.health);
            SendTCPDataToAll(packet);
        }
    }

    public static void PlayerRespawned(Player player)
    {
        using(Packet packet = new Packet((int)ServerPackets.playerRespawned))
        {
            packet.Write(player.id);
            SendTCPDataToAll(packet);
        }
    }

    public static void Projectile
    public static void ProjectileExploded(Vector3 position)
    {
        using (Packet packet = new Packet((int)ServerPackets.projectileExploded))
        {
            packet.Write(position); // will play particle effect on client side
            SendTCPDataToAll(packet);
        }
    }

    public static void CreateItemSpawner(int toClient, int spawnerID, Vector3 position, bool hasItem)
    {
        using(Packet packet = new Packet((int)ServerPackets.createItemSpawner))
        {
            packet.Write(spawnerID);
            packet.Write(position);
            packet.Write(hasItem);
            SendTCPData(toClient, packet);
        }
    }
}
