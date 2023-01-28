using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    bool isQuitting = false;
    [SerializeField] GameObject explodeParticlePrefab;

    [SerializeField] AudioSource audioSource;
    private void OnApplicationQuit()
    {
        isQuitting = true;
    }
    private void Start()
    {
        PlayAudio((int)FXID.fire);
    }
    public void PlayAudio(int fxID)
    {
        if (audioSource.clip != AudioManager.instance.audioClips[fxID])
        {
            audioSource.clip = AudioManager.instance.audioClips[fxID];
        }
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
    private void OnDestroy()
    {
        if (!isQuitting)
        {
            Instantiate(explodeParticlePrefab, transform.position, Quaternion.identity);
        }
    }
}
