using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }
    private PollButtonComponent BtnCombineDbs;
    public PollTextComponent AdminScreenText;
    private string AllLeaderboardDatabasePath;
    private string LeaderboardDatabasePath;
    private string ExhibitDatabasePath;
    private ExhibitData ExhibitData;
    private LeaderboardData LeaderboardData;
    private LeaderboardData AllLeaderboardData;
    private bool Loading = true;
    private bool PreviousDiableScreensaver;
    public NetworkManagerHUD NetworkManager;

    void Awake()
    {
        Instance = this;
        AllLeaderboardDatabasePath = Application.dataPath + "/" + SystemInfo.deviceName + "-all-leaderboard-database.json";
        LeaderboardDatabasePath = Application.dataPath + "/" + SystemInfo.deviceName + "-leaderboard-database.json";
        ExhibitDatabasePath = Application.dataPath + "/" + SystemInfo.deviceName + "-exhibit-database.json";
        LoadDatabases();
        AdminScreenText.gameObject.SetActive(false);
        BtnCombineDbs = GetComponentInChildren<PollButtonComponent>();
        BtnCombineDbs.gameObject.SetActive(false);
    }

    private void LoadDatabases()
    {
        ExhibitData = new ExhibitData();
        LeaderboardData = new LeaderboardData();
        AllLeaderboardData = new LeaderboardData();
        StartCoroutine(ExhibitData.GetData(ExhibitDatabasePath));
        StartCoroutine(AllLeaderboardData.GetData(AllLeaderboardDatabasePath));
        StartCoroutine(LeaderboardData.GetData(LeaderboardDatabasePath));
        StartCoroutine(CheckIsDoneParsing());
    }

    IEnumerator CheckIsDoneParsing()
    {
        if (ExhibitData != null && ExhibitData.IsDoneParsing && LeaderboardData.IsDoneParsing && AllLeaderboardData.IsDoneParsing)
        {
            Loading = false;
        }
        else
        {
            yield return new WaitForSeconds(1);
            yield return CheckIsDoneParsing();
        }
    }

    public void SavePlayerScore(string displayName, int score, string totalTime, List<PlayerAnswerData> playerAnswerData)
    {
        ExhibitData.AddPlayerScore(displayName, score, totalTime, playerAnswerData);
        ExhibitData.SaveExhibitData();
    }

    public void SaveLeaderboard(string displayName, string firstName, string lastName, int score, string totalTime)
    {
        LeaderboardData.AddPlayerScore(displayName, firstName, lastName, score, totalTime);
        LeaderboardData.SaveLeaderboard();
        AllLeaderboardData.AddPlayerScore(displayName, firstName, lastName, score, totalTime, 999999999);
        AllLeaderboardData.SaveLeaderboard();
    }

    public string GetExhibitDatabaseString()
    {
        return ExhibitData.GetDatabaseString();
    }

    public string GetLeaderboardDatabaseString()
    {
        return LeaderboardData.GetLeaderboardString();
    }

    public List<int> GetPlayerAnswersForQuestionId(int questionId)
    {
        var playerAnswers = new List<int>();
        foreach (var playerData in ExhibitData.PlayerData)
        {
            var playerAnswerData = playerData.PlayerAnswerData.Find(pd => pd.QuestionId == questionId);
            if (playerAnswerData != null)
            {
                playerAnswers.Add(playerAnswerData.AnswerId);
            }
        }
        return playerAnswers;
    }

    public void ShowAdminScreen()
    {
        PreviousDiableScreensaver = ScreensaverManager.Instance.DiableScreensaver;
        ScreensaverManager.Instance.DiableScreensaver = true;
        BtnCombineDbs.gameObject.SetActive(true);
        AdminScreenText.gameObject.SetActive(true);
        NetworkManager.showGUI = true;
    }

    public void HideAdminScreen()
    {
        ScreensaverManager.Instance.DiableScreensaver = PreviousDiableScreensaver;
        BtnCombineDbs.gameObject.SetActive(false);
        AdminScreenText.gameObject.SetActive(false);
        NetworkManager.showGUI = false;
    }

    /*public void CombineDatabases()
    {
        StartCoroutine(StartCombineDatabases());
        ExhibitGameManager.Instance.OnAdminKeysPressed();
    }

    public IEnumerator StartCombineDatabases()
    {
        var masterData = new ExhibitData(Application.dataPath + "/" + Guid.NewGuid().ToString() + "-master-db.json");

        var di = new DirectoryInfo(Application.dataPath + "/");
        var databaseFiles = di.GetFiles("*-database.json");

        foreach (var fi in databaseFiles)
        {
            var request = new WWW(fi.FullName);
            yield return request;
            if (string.IsNullOrEmpty(request.error))
            {
                var databaseData = new ExhibitData(fi.FullName);
                yield return databaseData.GetData();
                foreach (var playerScore in databaseData.PlayerData)
                {
                    masterData.AddPlayerScore(playerScore.PlayerDisplayName, playerScore.PlayerScore, playerScore.TotalTime, playerScore.PlayerAnswerData);
                }
            }
            else
            {
                Debug.Log("Poll : URL request failed, " + request.error);
            }
        }
        masterData.SaveExhibitData();
    }*/

    public void Update()
    {
        if (!Loading)
        {
#if UNITY_EDITOR
            if (Input.GetKeyUp(KeyCode.O))
            {
                ExhibitGameManager.Instance.OnAdminKeysPressed();
            }
#else
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
            if ((Input.GetKeyUp(KeyCode.F12) && Input.GetKey(KeyCode.LeftControl)))
            {
                ExhibitGameManager.Instance.OnAdminKeysPressed();
            }
#endif
        }
    }
}
