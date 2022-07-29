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

    public AudioClip[] audioClips;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
}
