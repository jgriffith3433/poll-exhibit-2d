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

    public void ShowLeaderboard(int score, TimeSpan? totalTime, List<PollUserAnswer> userAnswers, bool fromPoll, string displayName, string firstName, string lastName)
    {
        LeaderboardInstance = Instantiate(LeaderboardPrefab).GetComponent<LeaderboardComponent>();
        LeaderboardInstance.ShowLeaderboard(score, totalTime, userAnswers, fromPoll, displayName, firstName, lastName);
        LeaderboardInstance.ShowObjects();
        if (fromPoll)
        {
            ScreensaverManager.Instance.DiableScreensaver = true;
        }
        else
        {
            ScreensaverManager.Instance.DiableScreensaver = false;
        }
    }

    public void OnAddPlayerScore(string displayName, int score, string totalTime, string email)
    {
        ScreensaverManager.Instance.DiableScreensaver = false;
    }

    public void OnEnter()
    {
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
            LeaderboardInstance.ShowLeaderboard(0, null, null, false, null, null, null);
            LeaderboardInstance.ShowObjects();
        }
        else
        {
            LeaderboardInstance.ShowLeaderboardTopDown();
            LeaderboardInstance.HideFinishPoll();
            LeaderboardInstance.ShowObjects();
        }
    }

    public void HideLeaderboardScreensaver()
    {
        if (LeaderboardInstance != null)
        {
            DestroyLeaderboard();
        }
        ShowLeaderboardScreensaver();
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
