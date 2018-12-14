using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }
    private PollButtonComponent BtnCombineDbs;
    private PollTextComponent TxtTitle;
    private string DatabasePath;
    private ExhibitData Data;
    private bool Loading = true;
    private bool PreviousDiableScreensaver;

    void Awake()
    {
        Instance = this;
        DatabasePath = Application.dataPath + "/" + SystemInfo.deviceName + "-database.json";
        LoadDatabase();
        TxtTitle = GetComponentInChildren<PollTextComponent>();
        TxtTitle.gameObject.SetActive(false);
        BtnCombineDbs = GetComponentInChildren<PollButtonComponent>();
        BtnCombineDbs.gameObject.SetActive(false);
    }

    private void LoadDatabase()
    {
        Data = new ExhibitData(DatabasePath);
        StartCoroutine(Data.GetData());
        StartCoroutine(CheckIsDoneParsing());
    }

    IEnumerator CheckIsDoneParsing()
    {
        if (Data != null && Data.IsDoneParsing)
        {
            Loading = false;
        }
        else
        {
            yield return new WaitForSeconds(1);
            yield return CheckIsDoneParsing();
        }
    }

    public void SavePlayerScore(string displayName, int score, TimeSpan totalTime, List<PlayerAnswerData> playerAnswerData)
    {
        Data.AddPlayerScore(displayName, score, totalTime, playerAnswerData);
        Data.SaveExhibitData();
    }

    public List<int> GetPlayerAnswersForQuestionId(int questionId)
    {
        var playerAnswers = new List<int>();
        foreach (var playerData in Data.PlayerData)
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
        TxtTitle.gameObject.SetActive(true);
    }

    public void HideAdminScreen()
    {
        ScreensaverManager.Instance.DiableScreensaver = PreviousDiableScreensaver;
        BtnCombineDbs.gameObject.SetActive(false);
        TxtTitle.gameObject.SetActive(false);
    }

    public void CombineDatabases()
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
                Debug.Log("Poll : URL request failed ," + request.error);
            }
        }
        masterData.SaveExhibitData();
    }

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
