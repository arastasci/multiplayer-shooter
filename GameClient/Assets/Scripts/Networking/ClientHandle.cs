using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
/// <summary>
/// This class consists of <see cref="Client.PacketHandler"/> methods.
/// </summary>
public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from the server: {_msg}");
        Client.instance.myId = _myId;
        // todo: send welcome received packet
        ClientSend.WelcomeReceived();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
        MeasurePing.instance.enabled = true;
    }

    public static void SpawnPlayer(Packet packet)
    {
        int id = packet.ReadInt();
        string username = packet.ReadString();
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();

        GameManager.instance.SpawnPlayer(id, username, position, rotation);
    }
    public static void PlayerPosition(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector3();
        float speed = packet.ReadFloat();
        bool isGrounded = packet.ReadBool();
        bool isAffectedByExplosion = packet.ReadBool();
        GameManager.players[id].transform.position = position;
        GameManager.players[id].isGrounded = isGrounded;
        GameManager.players[id].isAffectedByExplosion = isAffectedByExplosion;
        if(id == Client.instance.myId)
        UIManager.instance.UpdateSpeed(speed);
    }
    public static void PlayerRotation(Packet packet)
    {
        int id = packet.ReadInt();
        Quaternion rotation = packet.ReadQuaternion();
        Quaternion weaponRotation = packet.ReadQuaternion();
        GameManager.players[id].transform.rotation = rotation;
        GameManager.players[id].SetActiveWeaponRotation(weaponRotation);
        
    }
    public static void PlayerCrouch(Packet packet)
    {
        int playerId = packet.ReadInt();
        bool state = packet.ReadBool();
        GameManager.players[playerId].Crouch(state);
    }
    public static void PlayerDisconnected(Packet packet)
    {
        Debug.Log("disconnecting;");
        int id = packet.ReadInt();
        ScoreManager.instance.DeleteRow(id);

        Destroy(GameManager.players[id].gameObject);
        GameManager.players.Remove(id);

    }

    public static void PlayerHealth(Packet packet)
    {
        int id = packet.ReadInt();
        int health = packet.ReadInt();
        int byPlayer = packet.ReadInt();

        GameManager.players[id].SetHealth(health, byPlayer);
    }

    public static void PlayerRespawned(Packet packet)
    {
        int id = packet.ReadInt();
        GameManager.players[id].Respawn();
    }

    public static void LaunchedProjectile(Packet packet)
    {
        int playerID = packet.ReadInt(); // TODO: implement kills
        int projectileID = packet.ReadInt();
        Vector3 projectilePosition = packet.ReadVector3();
        GameManager.instance.SpawnProjectile(projectileID, projectilePosition);
    }
    public static void ProjectilePosition(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector3();
        if (GameManager.projectiles.ContainsKey(id)) 
        { GameManager.instance.UpdateProjectilePosition(id, position); }
    }
    public static void ProjectileExploded(Packet packet)
    {
        int id = packet.ReadInt();
        GameManager.ProjectileExplode(id);
    }
    public static void PlayerReloading(Packet packet)
    {
        int id = packet.ReadInt();
        // make player play reload animation
    }
    // handle player reloaded END ANIMATION BLALBLALBLA
    public static void PlayerWeaponInfo(Packet packet)
    {
        int bulletLeftInMag = packet.ReadInt();
        int bulletLeftTotal = packet.ReadInt();
        UIManager.instance.UpdateAmmoInfo(bulletLeftInMag, bulletLeftTotal);
    }
    public static void PlayerChangeWeapon(Packet packet)
    {
        int playerID = packet.ReadInt();
        int weaponID = packet.ReadInt();
        GameManager.players[playerID].SetActiveWeapon(weaponID);

    }
    public static void UpdateScoreboard(Packet packet)
    {
        int playerCount = packet.ReadInt();
        //bool[] isStillInGame = new bool[ScoreManager.scoreboardInfos.Count];
        for (int i = 1; i <= GameManager.players.Count; i++)
        {
            int playerID = packet.ReadInt();
            int killCount = packet.ReadInt();
            int deathCount = packet.ReadInt();
            GameManager.players[playerID].score.SetKillDeath(killCount, deathCount);
        }
        ScoreManager.instance.UpdateScoreboard();

    }
    public static void CreateItemSpawner(Packet packet)
    {
        int spawnerID = packet.ReadInt();
        int itemType = packet.ReadInt();
        Vector3 position = packet.ReadVector3();
        bool hasItem = packet.ReadBool();

        GameManager.instance.CreateItemSpawner(spawnerID, position, itemType, hasItem);
        //packet.Write(itemType);
        //packet.Write(position);
        //packet.Write(hasItem);
    }
    public static void ItemSpawned(Packet packet)
    {
        int spawnerID = packet.ReadInt();
        int itemType = packet.ReadInt();
        GameManager.itemSpawners[spawnerID].ItemSpawned(itemType);
    }
    public static void ItemPickedUp(Packet packet)
    {
        int spawnerID = packet.ReadInt();
        int byPlayer = packet.ReadInt();
        GameManager.itemSpawners[spawnerID].ItemPickedUp();
    }

    public static void PlayAudio(Packet packet)
    {
        int fxID = packet.ReadInt();
        int fxEnt = packet.ReadInt();
        int entityID = packet.ReadInt();
        switch((FXEntity)fxEnt)
        {
            case FXEntity.player:
                GameManager.players[entityID].PlayAudio(fxID);
                break;
            case FXEntity.projectile:
                GameManager.projectiles[entityID].PlayAudio(fxID);
                break;

        }

    }

    public static void SetPing(Packet packet)
    {
        int id = packet.ReadInt();
        int ping = packet.ReadInt();
        Debug.Log(id + " " + ping);
        MeasurePing.instance.SetPing(GameManager.players[id], ping);
    }

    public static void Ping(Packet packet)
    {
        MeasurePing.instance.Ping();
    }

    public static void SelfKill(Packet packet)
    {
        int playerId = packet.ReadInt();
        if(playerId == Client.instance.myId)
        GameManager.players[playerId].SelfKill();
        UIManager.instance.LogKill(playerId, playerId, -1);
        
    }
}
