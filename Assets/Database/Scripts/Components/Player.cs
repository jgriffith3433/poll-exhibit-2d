using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    private string DatabaseString;
    private string LeaderboardString;
    public bool Loading = true;

    void Awake()
    {
        ExhibitGameManager.Instance.Player = this;
    }

    [Command]
    public void CmdLoadInitialData()
    {
        RpcUpdateDatabase(DatabaseManager.Instance.GetExhibitDatabaseString());
        RpcUpdateLeaderboard(DatabaseManager.Instance.GetLeaderboardDatabaseString());
    }

    [Command]
    public void CmdSavePlayerScore(string displayName, int score, string totalTime, int[] answerIds, int[] questionIds)
    {
        var playerAnswerData = new List<PlayerAnswerData>();
        for(var i = 0; i < answerIds.Length; i++)
        {
            playerAnswerData.Add(new PlayerAnswerData
            {
                AnswerId = answerIds[i],
                QuestionId = questionIds[i]
            });
        }
        DatabaseManager.Instance.SavePlayerScore(displayName, score, totalTime, playerAnswerData);

        RpcUpdateDatabase(DatabaseManager.Instance.GetExhibitDatabaseString());
    }

    [Command]
    public void CmdSaveLeaderboard(string displayName, string firstName, string lastName, int score, string totalTime)
    {
        DatabaseManager.Instance.SaveLeaderboard(displayName, firstName, lastName, score, totalTime);

        RpcUpdateLeaderboard(DatabaseManager.Instance.GetLeaderboardDatabaseString());
    }

    [ClientRpc]
    void RpcUpdateDatabase(string databaseStr)
    {
        DatabaseString = databaseStr;
    }

    [ClientRpc]
    void RpcUpdateLeaderboard(string leaderboardString)
    {
        LeaderboardString = leaderboardString;
        Loading = false;
    }

    public string GetDatabaseString()
    {
        return DatabaseString;
    }

    public string GetLeaderboardString()
    {
        return LeaderboardString;
    }
}