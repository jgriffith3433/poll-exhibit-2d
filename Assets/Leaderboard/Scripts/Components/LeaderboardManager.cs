using UnityEngine;
using System.Collections;
using ChartAndGraph;

public class LeaderboardManager : MonoBehaviour {
    public static LeaderboardManager Instance { get; private set; }

    public LeaderboardComponent LeaderboardPrefab;
    private LeaderboardComponent LeaderboardInstance;

    void Awake ()
    {
        Instance = this;
        LeaderboardInstance = Instantiate(LeaderboardPrefab).GetComponent<LeaderboardComponent>();
    }

    public void StartingLeaderboard()
    {
        LeaderboardInstance.RestartLeaderboard();
    }

    public void RestartLeaderboard()
    {
        LeaderboardInstance.FillLeaderboard();
    }

    public void OnInactive()
    {
        LeaderboardInstance.HideObjects();
    }

    public void OnPlay()
    {
        LeaderboardInstance.HideObjects();
        ExhibitGameManager.Instance.OnFinishLeaderboard();
    }
}
