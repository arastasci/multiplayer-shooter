using System;

[Serializable]
public class ScoreData
{
    public int id;
    public int kill = 0;
    public int death = 0;
    public int ping;
    public ScoreData(int id)
    {
        this.id = id;
        // TODO: ping
    }
}