using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeEffect : MonoBehaviour
{
    ParticleSystem particle;
    [SerializeField] AudioSource audioSource;

    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        particle.Play();
        audioSource.Play();
    }
    private void FixedUpdate()
    {
        if (!particle.isPlaying) Destroy(gameObject);
    }


}
