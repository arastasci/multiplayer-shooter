using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Weapon(int id, int magSize, int maxBullets, float reloadTime, float firePeriod)
    {
        this.id = id;
        this.magSize = magSize;
        this.reloadTime = reloadTime;
        bulletLeftInMag = magSize;
        this.bulletLeftTotal = maxBullets - magSize;
        this.firePeriod = firePeriod;
        canReload = true;
    }
    public int id;
    public int magSize;
    public float reloadTime;
    public int bulletLeftInMag;
    public int bulletLeftTotal;
    public float firePeriod;
    public bool canReload;
    public bool canShoot = true;
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
        if (bulletLeftInMag > 0)
        {
            bulletLeftInMag--;

        }
        else 
        {
            Reload(playerID);  
        }
    }
    IEnumerator WrappedReload(int playerID)
    {
        yield return new WaitForSeconds(this.reloadTime);
        bulletLeftTotal = Mathf.Max(magSize, 0);
        bulletLeftInMag = Mathf.Min(bulletLeftTotal, magSize);
        if (bulletLeftTotal == 0)
        {
            canReload = false;
        }
        ServerSend.PlayerWeaponInfo(playerID, this);



    }
}
