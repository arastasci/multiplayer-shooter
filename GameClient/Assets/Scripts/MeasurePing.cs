using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasurePing : MonoBehaviour
{
    private float time;
    private float pingTime;

    public static MeasurePing instance;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists.");
            Destroy(this);
        }
        
        
    }
    private void OnEnable()
    {
        time = 0;
        isPinged = false;
    }

    private bool isPinged;
    private void Update()
    {
        if (Time.time - time > 8 && !isPinged)
        {
            ClientSend.Ping();
            time = Time.time;
            pingTime = time;
            isPinged = true;
            Debug.Log("ping");
        }
    }

    public void SetPing(PlayerManager player, int ping)
    {
        Row row = player.score.row;
        row.ping.text = ping.ToString();
    }

    public void Ping()
    {
        int ping = (int)(1000f * (Time.time - pingTime));
        ClientSend.ClientPing(ping);
        Debug.Log(Time.time - pingTime);
        GameManager.players[Client.instance.myId].score.row.ping.text  = ping.ToString();
        isPinged = false;
    }
    
    
    
}
