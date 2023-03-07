using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;
    public int magSize;
    public float reloadTime;
    public int bulletLeftInMag;
    public int bulletLeftTotal;
    public float firePeriod;
    public bool canReload = true;
    public bool canShoot = true;
    public int maxBullet;
    
    private void Awake()
    {
        bulletLeftTotal -= magSize;
        bulletLeftInMag = magSize;
    }
    private void Start()
    {
        
        
        canReload = true;
        canShoot = true;
    }

    public void Fill(int playerId)
    {
        bulletLeftTotal = maxBullet;
        bulletLeftInMag = magSize;
        canReload = true;
        ServerSend.PlayerWeaponInfo(playerId, this);
    }
    public void Reload(int playerID)
    {
        if (canReload)
        {
            canShoot = false;
            canReload = false;
            StartCoroutine(WrappedReload(playerID));
        }
    }
    public void DecrementBullet(int playerID)
    {
        
        if (bulletLeftInMag == 0 )
        {
            canShoot = false;
            Reload(playerID);
            return;
        }
        bulletLeftInMag--;
        StartCoroutine(ShootCooldown()); 
        
    }

    IEnumerator ShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(firePeriod);
        if(bulletLeftInMag > 0)
        canShoot = true;
    }
    IEnumerator WrappedReload(int playerID)
    {
        yield return new WaitForSeconds(reloadTime);
        int bulletCount = Math.Min(bulletLeftTotal, magSize - bulletLeftInMag);
        bulletLeftTotal -= bulletCount;
        bulletLeftInMag += bulletCount;
       
        if (bulletLeftTotal == 0)
        {
            canReload = false;
        }
        else
        {
            canReload = true;
        }
        canShoot = true;
        ServerSend.PlayerWeaponInfo(playerID, this); // send info so it's displayed in UI
        
    }
}
