using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities : MonoBehaviour
{

    public static Utilities instance;

    
    private void Awake()
    {
        if(instance == null)
        instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    
    
}
