using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using System.Linq;
using System.IO;

public class ExhibitData
{
    private string RemoteUrl;
    public bool IsDoneParsing = false;
    public string ExhibitDataRawText { get; set; }

    public List<ExhibitPlayerData> PlayerData { get; set; }

    public ExhibitData(string remoteUrl)
    {
        PlayerData = new List<ExhibitPlayerData>();
        RemoteUrl = remoteUrl;
    }

    public IEnumerator GetData() {
        WWW request = new WWW(RemoteUrl);
        yield return request;
        if (string.IsNullOrEmpty(request.error)) {
            try
            {
                ExhibitDataRawText = request.text;
                StartParse(JSON.Parse(ExhibitDataRawText));
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
                    ExhibitDataRawText = "{}";
                    StartParse(JSON.Parse(ExhibitDataRawText));
                }
                catch (Exception e)
                {
                    Debug.LogError("Database : Invalid document format, please check your settings, with exception " + e.ToString());
                }
            }
            else
            {
                Debug.LogError("Database : URL request failed ," + request.error);
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
                var playerAnswerData = new List<PlayerAnswerData>();
                foreach(var playerAnswerXObj in playerXObj.Value["PlayerAnswerData"].Linq)
                {
                    playerAnswerData.Add(new PlayerAnswerData
                    {
                        QuestionId = int.Parse(playerAnswerXObj.Value["QuestionId"].Value),
                        AnswerId = int.Parse(playerAnswerXObj.Value["AnswerId"].Value)
                    });
                }
                PlayerData.Add(new ExhibitPlayerData
                {
                    PlayerDisplayName = playerXObj.Value["DisplayName"].Value,
                    PlayerScore = int.Parse(playerXObj.Value["Score"].Value),
                    PlayerAnswerData = playerAnswerData
                });
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
        }
    }

    public void AddPlayerScore(string displayName, int score, List<PlayerAnswerData> playerAnswerData)
    {
        PlayerData.Add(new ExhibitPlayerData
        {
            PlayerDisplayName = displayName,
            PlayerScore = score,
            PlayerAnswerData = playerAnswerData
        });
    }

    public void SaveExhibitData()
    {
        try
        {
            var rootXObj = new JSONObject();
            foreach(var playerData in PlayerData)
            {
                var playerDataChildObject = new JSONObject();
                playerDataChildObject["DisplayName"] = new JSONString(playerData.PlayerDisplayName);
                playerDataChildObject["Score"] = new JSONString(playerData.PlayerScore.ToString());

                var playerAnswers = new JSONArray();
                foreach(var playerAnswer in playerData.PlayerAnswerData)
                {
                    var playerAnswerDataChildObject = new JSONObject();
                    playerAnswerDataChildObject["QuestionId"] = new JSONString(playerAnswer.QuestionId.ToString());
                    playerAnswerDataChildObject["AnswerId"] = new JSONString(playerAnswer.AnswerId.ToString());
                    playerAnswers.Add(playerAnswerDataChildObject);
                }
                playerDataChildObject["PlayerAnswerData"] = playerAnswers;

                rootXObj.Add(playerDataChildObject);
            }
            File.WriteAllText(RemoteUrl, rootXObj.ToString());
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
        }
    }
}