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

    public LeaderboardData(string remoteUrl)
    {
        PlayerData = new List<LeaderboardPlayerData>();
        RemoteUrl = remoteUrl;
    }

    public IEnumerator GetData() {
        WWW request = new WWW(RemoteUrl);
        yield return request;
        if (String.IsNullOrEmpty(request.error)) {
            try
            {
                PollLeaderboardDataRawText = request.text;
                StartParse(JSON.Parse(PollLeaderboardDataRawText));
            }
            catch (Exception e)
            {
                Debug.LogError("Poll : Invalid document format, please check your settings, with exception " + e.ToString());
            }
        }
        else
        {
            Debug.LogError("Poll : URL request failed ," + request.error);
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
                        PlayerScore = int.Parse(playerXObj.Value[i]["Score"].Value),
                        TotalTime = TimeSpan.Parse(playerXObj.Value[i]["TotalTime"].Value)
                    });
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
        }
    }

    public void AddPlayerScore(string displayName, int score, TimeSpan totalTime)
    {
        if (PlayerData.Count == 10)
        {
            PlayerData.RemoveAt(9);
        }
        for(var i = 0; i < PlayerData.Count; i++)
        {
            PlayerData[i].PlayerBaseName = "Player " + (i + 1);
        }
        PlayerData.Add(new LeaderboardPlayerData
        {
            PlayerBaseName = "Player " + (PlayerData.Count + 1).ToString(),
            PlayerDisplayName = displayName,
            PlayerScore = score,
            TotalTime = totalTime
        });
    }

    public void SaveLeaderboard()
    {
        try
        {
            var rootXObj = new JSONObject();
            PlayerData = PlayerData.OrderByDescending(pd => pd.PlayerScore / pd.TotalTime.TotalSeconds).ToList();
            foreach (var playerData in PlayerData)
            {
                var playerDataChild = new JSONArray();
                var playerDataChildObject = new JSONObject();
                playerDataChildObject["DisplayName"] = new JSONString(playerData.PlayerDisplayName);
                playerDataChildObject["Score"] = new JSONString(playerData.PlayerScore.ToString());
                playerDataChildObject["TotalTime"] = new JSONString(playerData.TotalTime.ToString());
                playerDataChild.Add(playerDataChildObject);
                rootXObj[playerData.PlayerBaseName] = playerDataChild;
            }
            File.WriteAllText(RemoteUrl, rootXObj.ToString());
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
        }
    }
}