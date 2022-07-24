using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

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
        GameManager.players[id].transform.position = position;
    }
    public static void PlayerRotation(Packet packet)
    {
        int id = packet.ReadInt();
        Quaternion rotation = packet.ReadQuaternion();
        GameManager.players[id].transform.rotation = rotation;
    }

    public static void PlayerDisconnected(Packet packet)
    {
        int id = packet.ReadInt();
        Destroy(GameManager.players[id].gameObject);
        GameManager.players.Remove(id);
    }

    public static void PlayerHealth(Packet packet)
    {
        int id = packet.ReadInt();
        int health = packet.ReadInt();
        GameManager.players[id].SetHealth(health);
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
        bool[] isStillInGame = new bool[ScoreManager.scoreboardInfos.Count];
        for (int i = 0; i < playerCount; i++)
        {
            int playerID = packet.ReadInt();
            int killCount = packet.ReadInt();
            int deathCount = packet.ReadInt();
            bool found = false;
            for(int j = 0; i < ScoreManager.scoreboardInfos.Count; i++)
            {
                ScoreManager.ScoreCard card = ScoreManager.scoreboardInfos[j];
                if (card.id == playerID)
                {
                    card = new ScoreManager.ScoreCard(card.id, killCount, deathCount);
                    found = true;
                    isStillInGame[j] = true;
                    break;
                }
            }
            if(!found)
            {
                ScoreManager.instance.AddScore(new ScoreManager.ScoreCard(playerID, killCount, deathCount));
            }
        }
        for(int i = 0; i < isStillInGame.Length; i++)
        {
            if (!isStillInGame[i])
            {
                ScoreManager.instance.DeleteScore(i);
            }
        }
        ScoreManager.instance.UpdateScoreboard();
    }
}
