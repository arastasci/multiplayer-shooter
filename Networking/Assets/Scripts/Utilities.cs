using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities : MonoBehaviour
{

    public static Utilities instance;

    public int lastPlayerCountInScoreboard;
    private void Awake()
    {
        if(instance == null)
        instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    public struct ScoreboardInfo
    {
        public int deathCount;
        public int killCount;
        public int id;
        public ScoreboardInfo(int id, int kill, int death)
        {
            this.id = id;
            killCount = kill;
            deathCount = death;
        }
    }
    
}
