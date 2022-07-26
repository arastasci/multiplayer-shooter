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
        if (!canShoot) return;
        
        if(bulletLeftInMag == 0 )
        {
            canShoot = false;

            Reload(playerID);
            return;
        }
        bulletLeftInMag--;
        StartCoroutine(ShootCooldown());
        //if (bulletLeftInMag > 0)
        //{
        //    bulletLeftInMag--;

        //    if (bulletLeftInMag == 0)
        //    {
        //        canShoot = false;
        //        Reload(playerID);
        //    }

        //}
        //else 
        //{
        //    Reload(playerID);  
        //}
    }

    IEnumerator ShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(this.firePeriod);
        canShoot = true;
    }
    IEnumerator WrappedReload(int playerID)
    {
        yield return new WaitForSeconds(this.reloadTime);
        if(bulletLeftTotal <= magSize - bulletLeftInMag)
        {
            bulletLeftInMag += bulletLeftTotal;
            bulletLeftTotal = 0;
        }
        else
        {
            bulletLeftTotal -= magSize- bulletLeftInMag;
            bulletLeftInMag = magSize;
        }
        if (bulletLeftTotal == 0)
        {
            canReload = false;
        }
        else
        {
            canReload = true;
        }
        canShoot = true;
        ServerSend.PlayerWeaponInfo(playerID, this);



    }
}
