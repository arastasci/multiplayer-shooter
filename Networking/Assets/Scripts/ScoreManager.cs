using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScoreManager : MonoBehaviour 
{

    public GameObject rowUIPrefab;
    public static ScoreManager instance;
    public ScoreUI scoreUI;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    public struct ScoreCard
    {
        public int deathCount;
        public int killCount;
        public int id;
        
        public ScoreCard(int id, int kill, int death)
        {
            this.id = id;
            killCount = kill;
            deathCount = death;
            
        }
        
    }
    public static List<ScoreCard> scoreboardInfos = new List<ScoreCard>();

    public static IEnumerable<ScoreCard> GetScoreboard()
    {
        return scoreboardInfos.OrderByDescending(x => x.killCount);
    }
    public void AddScore(ScoreCard score)
    {
        scoreboardInfos.Add(score);
    }
    public void DeleteScore(int index)
    {
        scoreboardInfos.RemoveAt(index);
    }
    public void UpdateScoreboard()
    {
        ScoreCard[] scoreboardArr = GetScoreboard().ToArray();

        scoreUI.DrawScoreboard(scoreboardArr);

    }

}
