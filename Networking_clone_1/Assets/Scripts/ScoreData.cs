using System;
using UnityEngine;
[Serializable]
public class ScoreData { 
    public int deathCount;
    public int killCount;
    public int id;
    public Row row;
    public ScoreData(int id, int kill, int death)
    {
        this.id = id;
        killCount = kill;
        deathCount = death;

    }
    public ScoreData(int id, int kill, int death, Row row)
    {
        this.id = id;
        killCount = kill;
        deathCount = death;
        this.row = row;
        Debug.Log(this.row == null);
    }
    public void SetRow(Row row)
    {
        this.row = row;
    }
}