using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] GameObject rowUIPrefab;
    // Update is called once per frame
    public void DrawScoreboard(ScoreData[] scoreboardArr)
    {
        for(int i = 0; i < scoreboardArr.Length; i++) 
        {
            Debug.Log(i);
            if (scoreboardArr[i].row == null)
            {
                scoreboardArr[i].row = Instantiate(rowUIPrefab, transform).GetComponent<Row>();
                Debug.Log("instantiating for: " + i);
            }

            scoreboardArr[i].row.GetComponent<RectTransform>().SetAsLastSibling();
            SetRow(scoreboardArr[i], i);
            
        }
    }
    void SetRow(ScoreData playerData, int i)
    {
        foreach(PlayerManager player in GameManager.players.Values)
        {
            Debug.Log(player.id);
        }
        Row row = playerData.row;
        row.rank.text = (i + 1).ToString();
        row.userName.text = GameManager.players[playerData.id].username;
        row.killCount.text = playerData.killCount.ToString();
        row.deathCount.text = playerData.deathCount.ToString();
    }
}
