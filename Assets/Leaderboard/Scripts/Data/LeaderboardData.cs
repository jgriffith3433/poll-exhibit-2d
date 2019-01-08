using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using System.Linq;
using System.IO;

public class LeaderboardData
{
    private string RemoteUrl;
    public bool IsDoneParsing = false;
    public string PollLeaderboardDataRawText { get; set; }

    public List<LeaderboardPlayerData> PlayerData { get; set; }
    public LeaderboardData()
    {
        PlayerData = new List<LeaderboardPlayerData>();
    }

    public void SetData(string databaseString)
    {
        PollLeaderboardDataRawText = databaseString;
        StartParse(JSON.Parse(PollLeaderboardDataRawText));
        IsDoneParsing = true;
    }

    public IEnumerator GetData(string remoteUrl) {
        RemoteUrl = remoteUrl;
        WWW request = new WWW(RemoteUrl);
        yield return request;

        if (string.IsNullOrEmpty(request.error))
        {
            try
            {
                PollLeaderboardDataRawText = request.text;
                StartParse(JSON.Parse(PollLeaderboardDataRawText));
            }
            catch (Exception e)
            {
                Debug.LogError("Database : Invalid document format, please check your settings, with exception " + e.ToString());
            }
        }
        else
        {
            if (request.error.ToLower() == "404 not found")
            {
                try
                {
                    PollLeaderboardDataRawText = "{}";
                    StartParse(JSON.Parse(PollLeaderboardDataRawText));
                }
                catch (Exception e)
                {
                    Debug.LogError("Database : Invalid document format, please check your settings, with exception " + e.ToString());
                }
            }
            else
            {
                Debug.LogError("Database : URL request failed, " + request.error);
            }
        }
        IsDoneParsing = true;
    }

    private void StartParse(JSONNode xObj)
    {
        try
        {
            foreach(var playerXObj in xObj.Linq)
            {
                for(var i = 0; i < playerXObj.Value.Count; i++)
                {
                    PlayerData.Add(new LeaderboardPlayerData
                    {
                        PlayerBaseName = playerXObj.Key,
                        PlayerDisplayName = playerXObj.Value[i]["DisplayName"].Value,
                        PlayerFirstName = playerXObj.Value[i]["FirstName"].Value,
                        PlayerLastName = playerXObj.Value[i]["LastName"].Value,
                        PlayerScore = int.Parse(playerXObj.Value[i]["Score"].Value),
                        TotalTime = TimeSpan.Parse(playerXObj.Value[i]["TotalTime"].Value),
                        MadeTheLeaderboard = playerXObj.Value[i]["MadeTheLeaderboard"].AsBool
                    });
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
        }
    }

    public void AddPlayerScore(string displayName, string firstName, string lastName, int score, string totalTime, bool madeLeaderboard, int max = 10)
    {
        if (PlayerData.Count == max)
        {
            PlayerData.RemoveAt(max - 1);
        }
        for(var i = 0; i < PlayerData.Count; i++)
        {
            PlayerData[i].PlayerBaseName = "Player " + (i + 1);
        }
        PlayerData.Add(new LeaderboardPlayerData
        {
            PlayerBaseName = "Player " + (PlayerData.Count + 1).ToString(),
            PlayerDisplayName = displayName,
            PlayerFirstName = firstName,
            PlayerLastName = lastName,
            PlayerScore = score,
            TotalTime = TimeSpan.Parse(totalTime),
            MadeTheLeaderboard = madeLeaderboard
        });
    }

    private JSONObject CreateLeaderboardObject()
    {
        var leaderboardXObj = new JSONObject();
        var orderedPlayerScoreData = PlayerData.OrderByDescending(pd => pd.PlayerScore).ToList();
        var sortedPlayerData = new List<LeaderboardPlayerData>();

        foreach(var playerScoreData in orderedPlayerScoreData)
        {
            if (!sortedPlayerData.Contains(playerScoreData))
            {
                var sameScores = orderedPlayerScoreData.Where(pd => pd.PlayerScore == playerScoreData.PlayerScore).OrderByDescending(pd => pd.PlayerScore / pd.TotalTime.TotalSeconds);
                sortedPlayerData.AddRange(sameScores);
            }
        }

        foreach (var playerData in sortedPlayerData)
        {
            var playerDataChild = new JSONArray();
            var playerDataChildObject = new JSONObject();
            playerDataChildObject["DisplayName"] = new JSONString(playerData.PlayerDisplayName);
            playerDataChildObject["FirstName"] = new JSONString(playerData.PlayerFirstName);
            playerDataChildObject["LastName"] = new JSONString(playerData.PlayerLastName);
            playerDataChildObject["Score"] = new JSONString(playerData.PlayerScore.ToString());
            playerDataChildObject["TotalTime"] = new JSONString(playerData.TotalTime.ToString());
            playerDataChildObject["MadeTheLeaderboard"] = new JSONString(playerData.MadeTheLeaderboard.ToString());
            playerDataChild.Add(playerDataChildObject);
            leaderboardXObj[playerData.PlayerBaseName] = playerDataChild;
        }
        return leaderboardXObj;
    }

    public void SaveLeaderboard()
    {
        try
        {
            File.WriteAllText(RemoteUrl, GetLeaderboardString());
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
        }
    }

    public string GetLeaderboardString()
    {
        return CreateLeaderboardObject().ToString();
    }
}