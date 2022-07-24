using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScoreManager : MonoBehaviour 
{

    public GameObject rowUIPrefab;
    public static ScoreManager instance;
    public ScoreUI scoreUI;
    public GameObject content;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    //public struct ScoreCard
    //{
    //    public int deathCount;
    //    public int killCount;
    //    public int id;

    //    public ScoreCard(int id, int kill, int death)
    //    {
    //        this.id = id;
    //        killCount = kill;
    //        deathCount = death;

    //    }

    //}
    public static List<ScoreData> scoreDatas = new List<ScoreData>();

    public static IEnumerable<ScoreData> GetScoreboard()
    {
        return scoreDatas.OrderByDescending(x => x.killCount);
    }
    public void AddData(ScoreData scoreData)
    {
        scoreDatas.Add(scoreData);
        Debug.Log(scoreData.id + " is added to the scoreboardArr");
    }
    public void DeleteScore(int index)
    {
        Debug.Log("deleting score of " + index);
        Debug.Log("destroying scoredata with id " + scoreDatas.Where(scoreData => scoreData.id == index).FirstOrDefault().id);
        GameObject imStupid = scoreDatas[index].row.gameObject;
        scoreDatas.Remove(scoreDatas.Where(scoreData => scoreData.id == index).FirstOrDefault());
        Destroy(imStupid);

        UpdateScoreboard();
    }
    public void UpdateScoreboard()
    {
        
        ScoreData[] scoreboardArr = GetScoreboard().ToArray();
        Debug.Log( "scorebordarr length"+scoreboardArr.Length);
        scoreUI.DrawScoreboard(scoreboardArr);
    }

}
