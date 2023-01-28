using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;
    public int health;
    public int maxHealth;
    public MeshRenderer model;
    [SerializeField] GameObject[] weapons = new GameObject[2];
    PlayerController playerController;
    CameraController cameraController;
    public ScoreData score;
    int activeWeapon = 1;
    [SerializeField] AudioSource audioSource;
    public bool isGrounded;
    public bool isAffectedByExplosion;
    public AudioClip[] sounds;
    private MeshRenderer[] meshRenderers;
    int currentClip = -1;
    public void Initialize(int id, string userName)
    {
        this.id = id;
        username = userName;
        health = maxHealth;
        if (id == Client.instance.myId) 
        {
            UIManager.instance.slider.maxValue = maxHealth;
            UIManager.instance.UpdateHealth(maxHealth);
            playerController = GetComponent<PlayerController>();
            cameraController = transform.Find("Camera").GetComponent<CameraController>();
        }

        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        score = new ScoreData(id, 0, 0);
        
    }

    public void PlayAudio(int fxID)
    {
        if(currentClip == -1)
        {
            currentClip = fxID;
        } 
        
        if (!audioSource.isPlaying || (fxID != (int)FXID.walk))
        {
            currentClip = fxID;
            audioSource.clip = AudioManager.instance.audioClips[currentClip];

            if (currentClip == (int)FXID.walk)
            {
                if (!isGrounded || isAffectedByExplosion) return;
                audioSource.volume = Random.Range(0.7f, 1f);
                audioSource.pitch = Random.Range(0.7f, 1f);
            }
            audioSource.Play();

        }
    }
    public void SetHealth(int health, int byPlayer)
    {
        this.health = health;
        if(id == Client.instance.myId)
        {
            UIManager.instance.UpdateHealth(health);
        }
        if(health <= 0)
        {
            Die(byPlayer);
        }
    }

    public void Die(int byPlayer)
    {
        SetMesh(false);
        weapons[activeWeapon].SetActive(false);
        if(id == Client.instance.myId)
        {
            playerController.enabled = false;
            cameraController.LockToKiller(byPlayer);
            // make cam follow killer CM
            UIManager.instance.DisplayKiller(GameManager.players[byPlayer].username);
        }
        

    }
    public void Crouch(bool state)
    {
        if (state)
        {
            transform.localScale = new Vector3(1, 0.5f, 1);
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }
    public void Respawn()
    {
        SetMesh(true);
        weapons[activeWeapon].SetActive(true);
        if(id == Client.instance.myId)
        {
            playerController.enabled = true;
            cameraController.GoBackToPlayer();
            SetHealth(maxHealth, id); // id totally useless here, should refactor the code later
            UIManager.instance.HideKiller();
        }
    }

    public void SetActiveWeapon(int id)
    {
        weapons[activeWeapon].SetActive(false);
        activeWeapon = id;
        weapons[activeWeapon].SetActive(true);
    }
    private void OnDestroy()
    {
        if(TryGetComponent<CameraController>(out cameraController))
        {
            cameraController.GoBackToPlayer();
        }
    }

    private void SetMesh(bool value)
    {
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.enabled = value;
        }
    }
}
