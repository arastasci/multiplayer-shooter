using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] GameObject rowUIPrefab;
    Dictionary<int, Row> rows = new Dictionary<int, Row>();
    // Update is called once per frame
    public void DrawScoreboard(ScoreManager.ScoreCard[] scoreboardArr)
    {
        for (int i = 0; i < scoreboardArr.Length; i++)
        {
            if (rows.TryGetValue(scoreboardArr[i].id, out Row val))
            {
                val.GetComponent<RectTransform>().SetSiblingIndex(i);

                SetRow(val, scoreboardArr[i],i);
            }
            else
            {
                
                ScoreManager.ScoreCard playerRow = scoreboardArr[i];
                Row row = Instantiate(rowUIPrefab, transform).GetComponent<Row>();
                rows.Add(playerRow.id,row);
                SetRow(row, playerRow,i);
            }
            
            
            
        }
    }
    void SetRow(Row row, ScoreManager.ScoreCard playerRow,int i)
    {
        row.rank.text = (i + 1).ToString();
        row.userName.text = GameManager.players[playerRow.id].name;
        row.killCount.text = playerRow.killCount.ToString();
        row.deathCount.text = playerRow.deathCount.ToString();
    }
}
