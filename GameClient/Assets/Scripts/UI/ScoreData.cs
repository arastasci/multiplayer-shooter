using System;
using UnityEngine;

/// <summary>
/// This is mainly a data storage class holding the score of players.
/// </summary>
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
    public void SetKillDeath(int kill, int death)
    {
        killCount = kill;
        deathCount = death;
    }
    public void SetRow(Row row)
    {
        this.row = row;
    }
}