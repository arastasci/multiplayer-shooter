using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound 
{
    [SerializeField] AudioClip clip;
    public float volume;
    public float pitch;
    public Sound(AudioClip clip, float volume, float pitch)
    {
        this.clip = clip;
        this.volume = volume;
        this.pitch = pitch;
    }
}
