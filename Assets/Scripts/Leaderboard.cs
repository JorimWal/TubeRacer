using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Leaderboard
{
    public static (string[], int[]) GetLeaderboard()
    {
        string[] names = new string[10];
        int[] scores = new int[10];
        string leaderBoard = PlayerPrefs.GetString("Leaderboard");
        //Load the leaderboard from playerprefs or create a new one if none is present
        if (leaderBoard == "")
        {
            for (int i = 0; i < 10; i++)
            {
                names[i] = "John Doe";
                scores[i] = (10 - i) * 100;
            }
        }
        else
        {
            string[] splits = leaderBoard.Split('|');
            for (int i = 0; i < splits.Length / 2; i++)
            {
                names[i] = splits[i * 2];
                int parsedScore = 0;
                if (int.TryParse(splits[(i * 2) + 1], out parsedScore))
                    scores[i] = parsedScore;
            }
        }

        return (names, scores);
    }

    public static void SetLeaderboard(string[] names, int[] scores)
    {

        //Turn the leaderboard back into a string and write to playerprefs
        string leaderBoard = "";
        for (int i = 0; i < 10; i++)
        {
            leaderBoard += $"|{names[i]}|{scores[i]}";
        }
        //Remove unnecessary splitters
        leaderBoard = leaderBoard.Substring(1);
        PlayerPrefs.SetString("Leaderboard", leaderBoard);
    }
}
