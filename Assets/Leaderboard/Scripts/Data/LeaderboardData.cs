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
    public string PollLeadboardDataRawText { get; set; }

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
                PollLeadboardDataRawText = request.text;
                StartParse(JSON.Parse(PollLeadboardDataRawText));
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
                        PlayerScore = int.Parse(playerXObj.Value[i]["Score"].Value)
                    });
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
        }
    }

    public void AddPlayerScore(string displayName, int score)
    {
        if (PlayerData.Count == 10)
        {
            PlayerData.RemoveAt(0);
        }
        for(var i = 0; i < PlayerData.Count; i++)
        {
            PlayerData[i].PlayerBaseName = "Player " + (i + 1);
        }
        PlayerData.Add(new LeaderboardPlayerData
        {
            PlayerBaseName = "Player " + (PlayerData.Count + 1).ToString(),
            PlayerDisplayName = displayName,
            PlayerScore = score
        });
    }

    public void SaveLeaderboard()
    {
        try
        {
            var rootXObj = new JSONObject();
            foreach(var playerData in PlayerData)
            {

                var playerDataChild = new JSONArray();
                var playerDataChildObject = new JSONObject();
                playerDataChildObject["DisplayName"] = new JSONString(playerData.PlayerDisplayName);
                playerDataChildObject["Score"] = new JSONString(playerData.PlayerScore.ToString());
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