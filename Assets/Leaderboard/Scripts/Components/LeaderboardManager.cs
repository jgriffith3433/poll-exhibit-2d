using UnityEngine;
using System.Collections;

public class LeaderboardManager : MonoBehaviour {
    public static LeaderboardManager Instance { get; private set; }

    public LeaderboardComponent LeaderboardPrefab;
    private LeaderboardComponent LeaderboardInstance;

    void Awake ()
    {
        Instance = this;
    }

    public void ShowLeaderboard(int score, float totalTime)
    {
        DestroyLeaderboard();
        LeaderboardInstance = Instantiate(LeaderboardPrefab).GetComponent<LeaderboardComponent>();
        LeaderboardInstance.ShowLeaderboard(score, totalTime);
        LeaderboardInstance.ShowObjects();
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
            LeaderboardInstance.ShowLeaderboard(0, 0.0f);
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
