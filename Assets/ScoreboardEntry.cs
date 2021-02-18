using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardEntry : MonoBehaviour
{
    public int index;
    public Text IndexText, NameText, ScoreText;
    public InputField InputField;
    // Start is called before the first frame update
    void Start()
    {
        var leaderBoard = Leaderboard.GetLeaderboard();
        IndexText.text = (index + 1).ToString();
        NameText.text = leaderBoard.Item1[index];
        ScoreText.text = leaderBoard.Item2[index].ToString();
        if(PlayerPrefs.GetInt("EditLeaderboardEntryIndex") == index)
        {
            InputField.onValueChanged.AddListener(UpdateName);
        }
        else if(PlayerPrefs.GetInt("EditLeaderboardEntryIndex") == -1)
        {
            InputField.gameObject.SetActive(false);
        }
    }

    void UpdateName(string name)
    {
        name = CleanName(name);
        NameText.text = name;
        var leaderBoard = Leaderboard.GetLeaderboard();
        leaderBoard.Item1[index] = name;
        Leaderboard.SetLeaderboard(leaderBoard.Item1, leaderBoard.Item2);
    }

    string CleanName(string name)
    {
        //Remove our separator character so the scoreboard entry stays clean
        name = name.Replace("|", "");
        if(name.Length > 16)
        { 
            //Limit name to 16 characters
            name = name.Substring(0, 16);
        }
        return name;
    }

    public void FinishEditingScoreboard()
    {
        PlayerPrefs.SetInt("EditLeaderboardEntryIndex", -1);
    }
}
