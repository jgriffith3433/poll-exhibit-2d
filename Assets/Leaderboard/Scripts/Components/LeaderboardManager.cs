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

    public void FillLeaderboard()
    {
        DestroyLeaderboard();
        LeaderboardInstance = Instantiate(LeaderboardPrefab).GetComponent<LeaderboardComponent>();
        LeaderboardInstance.FillLeaderboard();
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
            LeaderboardInstance.FillLeaderboard();
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
