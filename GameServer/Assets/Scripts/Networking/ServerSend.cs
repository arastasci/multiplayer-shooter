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
    public static void PlayerCrouch(Player player, bool state)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerCrouch))
        {
            packet.Write(player.id);
            packet.Write(state);
            SendTCPDataToAll(packet);
        }
    }
    public static void PlayerPosition(Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerPosition))
        {
            packet.Write(player.id);
            packet.Write(player.transform.position);
            packet.Write(player.playerMovement.planarSpeed);
            packet.Write(player.playerMovement.isGrounded);
            packet.Write(player.affectedByExplosion);
            SendUDPDataToAll(packet);
        }
    }
    public static void PlayerRotation(Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerRotation))
        {
            packet.Write(player.id);
            packet.Write(player.transform.rotation);
            packet.Write(player.weaponRotation);
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
    public static void PlayerHealth(Player player, int byPlayer)
    {
        using(Packet packet = new Packet((int)ServerPackets.playerHealth))
        {
            packet.Write(player.id);
            packet.Write(player.health);
            packet.Write(byPlayer);
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

    public static void ProjectileExploded(int id)
    {
        using (Packet packet = new Packet((int)ServerPackets.projectileExploded))
        {
            packet.Write(id);
            SendTCPDataToAll(packet);
        }
    }

    public static void ProjectileLaunched(Player byPlayer, int projectileID, Vector3 position)
    {
        using(Packet packet = new Packet((int)ServerPackets.projectileLaunched))
        {
            packet.Write(byPlayer.id);
            packet.Write(projectileID);
            packet.Write(position);
            SendTCPDataToAll(packet);
        }
    }
    public static void ProjectilePosition(Projectile projectile)
    {
        using (Packet packet = new Packet((int)ServerPackets.projectilePosition))
        {
            packet.Write(projectile.id);
            packet.Write(projectile.transform.position);
            SendUDPDataToAll(packet);
        }
    }
    public static void CreateItemSpawner(int toClient, int spawnerID,int itemType, Vector3 position, bool hasItem)
    {
        using(Packet packet = new Packet((int)ServerPackets.createItemSpawner))
        {
            packet.Write(spawnerID);
            packet.Write(itemType);
            packet.Write(position);
            packet.Write(hasItem);
            SendTCPData(toClient, packet);
        }
    }
    
    public static void PlayerReloading(int id)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerReloading))
        {
            packet.Write(id);
            SendTCPDataToAll(packet);
        }
    }
    public static void PlayerReloaded(int id)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerReloaded))
        {
            packet.Write(id);
            SendTCPDataToAll(packet);
        }
    }
    public static void PlayerWeaponInfo(int ofClient, Weapon weapon)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerWeaponInfo))
        {
            packet.Write(weapon.bulletLeftInMag);
            packet.Write(weapon.bulletLeftTotal);
            SendTCPData(ofClient, packet);
        }
    }
    public static void PlayerChangeWeapon(int toClient,int playerID,int weaponID)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerChangeWeapon))
        {
            packet.Write(playerID);
            packet.Write(weaponID);
            if(toClient == -1)
            {
                SendTCPDataToAll(packet);
            }
            else
            {
                SendTCPData(toClient, packet);
            }
        }
    }
    
    public static void UpdateScoreBoard()
    {
        using (Packet packet = new Packet((int)ServerPackets.updateScoreboard))
        {
            int playerCount = 0;
            foreach(Client client in Server.clients.Values)
            {
                if (client.player != null) playerCount++;
            }
            packet.Write(playerCount);
            for(int i = 1; i <= playerCount; i++)
            {
                Client client = Server.clients[i];
                if (client.player == null) continue;
                packet.Write(client.player.id);
                packet.Write(client.player.GetKill());
                packet.Write(client.player.GetDeath());
               
            }
            SendTCPDataToAll(packet);
        }
    }
    
    public static void ItemSpawned(int spawnerID, int itemType)
    {
        using (Packet packet = new Packet((int)ServerPackets.itemSpawned))
        {
            packet.Write(spawnerID);
            packet.Write(itemType);
            SendTCPDataToAll(packet);
        }
    }
    public static void ItemPickedUp(int spawnerID, int byPlayer)
    {
        using (Packet packet = new Packet((int)ServerPackets.itemPickedUp))
        {
            packet.Write(spawnerID);
            packet.Write(byPlayer);
            SendTCPDataToAll(packet);
        }
    }

    public static void PlayAudio(int fxID,int fxEnt, int entityID )
    {
        using (Packet packet = new Packet((int)ServerPackets.playAudio))
        {
            packet.Write(fxID);
            packet.Write(fxEnt);
            packet.Write(entityID);
            SendTCPDataToAll(packet);
        }
    }

    public static void SelfKill(int player)
    {
        using (Packet packet = new Packet((int)ServerPackets.selfkill))
        {
            packet.Write(player);
            SendTCPDataToAll(packet);
        }
    }

    public static void Ping(int pingedClient)
    {
        using (Packet packet = new Packet((int)ServerPackets.ping))
        {
            SendTCPData(pingedClient, packet);
        }
    }

    public static void OthersPing(int pingedClient, int ping)
    {
        using (Packet packet = new Packet((int)ServerPackets.clientPing))
        {
            packet.Write(pingedClient);
            packet.Write(ping);
            foreach (Client c in Server.clients.Values)
            {
                if (c.id != pingedClient) SendTCPData(c.id,packet);
            }
        }
    }
}


