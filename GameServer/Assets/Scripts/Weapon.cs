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
    private void Start()
    {
        bulletLeftInMag = magSize;
        bulletLeftTotal -= magSize;
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
        if (bulletLeftInMag > 0)
        {
            bulletLeftInMag--;

            if (bulletLeftInMag == 0)
            {
                canShoot = false;
                Reload(playerID);
            }

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
        else
        {
            canReload = true;
        }
        canShoot = true;
        ServerSend.PlayerWeaponInfo(playerID, this);



    }
}
