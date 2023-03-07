using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public class ScoreManager : MonoBehaviour 
{

    public GameObject rowUIPrefab;
    public static ScoreManager instance;
    public Transform content;

    private void Awake()
    {
        if (instance == null) instance = this;   
    }
    public  void UpdateScoreboard()
    {
        List<PlayerManager> playersList =  GetPlayerManagers().ToList();   // returns the list ordered by the killCount
        for(int i = 0; i < playersList.Count; i++)
        {
            PlayerManager p = playersList[i];
            if(p.score.row == null)
            p.score.row = Instantiate(rowUIPrefab, content).GetComponent<Row>();
            p.score.row.GetComponent<RectTransform>().SetAsLastSibling();
            SetRow(p, i ); // send i to set the rank
        }

    }
    public void DeleteRow(int id)
    {
        Destroy(GameManager.players[id].score.row.gameObject);
        UpdateScoreboard();
    }
    /// <summary>
    /// Sets the row corresponding to <paramref name="player"/>, ranking <paramref name="i"/>+1.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="i"> i+1 is the rank of the player.</param>
    void SetRow(PlayerManager player, int i)
    {
        Row row = player.score.row;
        row.rank.text = (i + 1).ToString();
        row.userName.text = player.username;
        row.killCount.text = player.score.killCount.ToString();
        row.deathCount.text = player.score.deathCount.ToString();
        
    }

    
    
    IEnumerable<PlayerManager> GetPlayerManagers()
    {
        return GameManager.players.Values.OrderByDescending(x=> x.score.killCount);
    }
}
