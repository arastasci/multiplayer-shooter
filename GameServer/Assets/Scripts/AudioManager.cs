using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum FXEntity
{
    player = 0,
    projectile

}
public enum FXID
{
    walk = 0,
    jump,
    wallJump,
    speedy,
    health,
    grounded,
    die,
    fire,
    explode,
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    
    private void Awake()
    {
        if(instance == null)
        instance = this;
    }

    public void PlayAudio(FXID fXID,FXEntity entity, int entityID)
    {
        int effectsID = (int)fXID;
        int entityType = (int)entity;

        ServerSend.PlayAudio(effectsID,entityType,entityID);


    }




}
