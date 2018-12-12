using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class LeaderboardManager : MonoBehaviour {
    public static LeaderboardManager Instance { get; private set; }

    public LeaderboardComponent LeaderboardPrefab;
    private LeaderboardComponent LeaderboardInstance;

    void Awake ()
    {
        Instance = this;
    }

    public void ShowLeaderboard(int score, TimeSpan? totalTime, List<PollUserAnswer> userAnswers, bool fromPoll)
    {
        LeaderboardInstance = Instantiate(LeaderboardPrefab).GetComponent<LeaderboardComponent>();
        LeaderboardInstance.ShowLeaderboard(score, totalTime, userAnswers, fromPoll);
        LeaderboardInstance.ShowObjects();
    }

    public void OnLogin(string displayName, string fullName)
    {
        LeaderboardInstance.SaveLeaderboard(displayName, fullName);
    }

    public void OnContinue()
    {
        LeaderboardInstance.OnContinue();
    }

    public void OnInactive()
    {
        if (LeaderboardInstance != null)
        {
            LeaderboardInstance.HideObjects();
        }
    }

    public void ShowLeaderboardScreensaver()
    {
        if (LeaderboardInstance == null)
        {
            LeaderboardInstance = Instantiate(LeaderboardPrefab).GetComponent<LeaderboardComponent>();
            LeaderboardInstance.ShowLeaderboard(0, null, null, false);
            LeaderboardInstance.ShowObjects();
        }
        LeaderboardInstance.ShowObjects();
    }

    public void HideLeaderboardScreensaver()
    {
        if (LeaderboardInstance != null)
        {
            LeaderboardInstance.HideObjects();
        }
    }

    public void DestroyLeaderboard()
    {
        if (LeaderboardInstance != null)
        {
            Destroy(LeaderboardInstance.gameObject);
        }
    }

    public void OnPlay()
    {
        DestroyLeaderboard();
        ExhibitGameManager.Instance.OnFinishLeaderboard();
    }
}
