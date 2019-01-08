using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeaderboardComponent : MonoBehaviour
{
    private LeaderboardData Data;

    public LeaderboardTopDownComponent LeaderboardTopDownPrefab;
    private LeaderboardTopDownComponent LeaderboardTopDownInstance;
    
    public PollTextComponent TxtTitle;
    public PollTextComponent TxtSubTitle;
    public PollTextComponent TxtPlayNow;
    
    public PollFinishedComponent PollFinishedPrefab;
    private PollFinishedComponent PollFinishedInstance;

    private int Score;
    private TimeSpan? TotalTime;
    private List<PollUserAnswer> UserAnswers;
    private bool FromPoll;

    private string DisplayName;
    private string FirstName;
    private string LastName;

    private bool Loading = true;
    
    private bool PollFinishedHidden = false;
    private bool LeaderboardHidden = false;

    private bool MadeTopTenLeaderboard = false;

    public bool Hidden;

    public void ShowLeaderboard(int score, TimeSpan? totalTime, List<PollUserAnswer> userAnswers, bool fromPoll, string displayName, string firstName, string lastName)
    {
        UserAnswers = userAnswers;
        FromPoll = fromPoll;
        Score = score;
        TotalTime = totalTime;
        DisplayName = displayName;
        FirstName = firstName;
        LastName = lastName;
        
        Loading = true;
        ShowObjects();
        ExhibitGameManager.Instance.Player.GetDatabaseString();
        Data = new LeaderboardData();
        Data.SetData(ExhibitGameManager.Instance.Player.GetLeaderboardString());
        StartCoroutine(CheckIsDoneParsing());
    }

    public void SaveLeaderboard()
    {
        ExhibitGameManager.Instance.Player.CmdSaveLeaderboard(DisplayName, FirstName, LastName, Score, TotalTime.Value.ToString(), MadeTopTenLeaderboard);
        StartCoroutine(ShowLeaderboardAfterSaving());
    }

    public void SavePlayerData()
    {
        var playerAnswerDataAnswers = new List<int>();
        var playerAnswerDataQuestions = new List<int>();
        foreach (var userAnswer in UserAnswers)
        {
            playerAnswerDataAnswers.Add(userAnswer.AnswerId);
            playerAnswerDataQuestions.Add(userAnswer.QuestionId);
        }
        ExhibitGameManager.Instance.Player.CmdSavePlayerScore(Guid.NewGuid().ToString(), Score, TotalTime.Value.ToString(), playerAnswerDataAnswers.ToArray(), playerAnswerDataQuestions.ToArray());
    }

    IEnumerator ShowLeaderboardAfterSaving()
    {
        yield return new WaitForSeconds(10f);
        ExhibitGameManager.Instance.OnSaveLeaderboard();
    }

    IEnumerator CheckIsDoneParsing()
    {
        if (Data != null && Data.IsDoneParsing)
        {
            CreateObjects();
            if (FromPoll)
            {
                SavePlayerData();
            }
            Loading = false;
        }
        else
        {
            yield return new WaitForSeconds(1);
            yield return CheckIsDoneParsing();
        }
    }

    public void CreateObjects()
    {
        LeaderboardTopDownInstance = Instantiate(LeaderboardTopDownPrefab).GetComponent<LeaderboardTopDownComponent>();
        LeaderboardTopDownInstance.transform.SetParent(transform);
        LeaderboardTopDownInstance.SetPlayerData(Data.PlayerData);
        LeaderboardTopDownInstance.CreateObjects();
        LeaderboardTopDownInstance.AnimateFadeIn();
        
        PollFinishedInstance = Instantiate(PollFinishedPrefab).GetComponent<PollFinishedComponent>();
        PollFinishedInstance.transform.SetParent(transform);
        if (FromPoll)
        {
            var lowestScore = Data.PlayerData.Count == 10 ? Data.PlayerData.LastOrDefault() : null;
            if (lowestScore != null)
            {
                if (Score > 0 && Score / TotalTime.Value.TotalSeconds > lowestScore.PlayerScore / lowestScore.TotalTime.TotalSeconds)
                {
                    MadeTopTenLeaderboard = true;
                    HideLeaderboardTopDown();
                    PollFinishedInstance.SetValues(Score, TotalTime.Value);
                    ShowFinishPoll();
                }
                else
                {
                    PollFinishedInstance.SetValues(Score, TotalTime.Value);
                    ShowFinishPoll();
                    HideLeaderboardTopDown();
                    MadeTopTenLeaderboard = false;
                }
            }
            else
            {
                if (Score > 0)
                {
                    MadeTopTenLeaderboard = true;
                    HideLeaderboardTopDown();
                    PollFinishedInstance.SetValues(Score, TotalTime.Value);
                    ShowFinishPoll();
                }
                else
                {
                    PollFinishedInstance.SetValues(Score, TotalTime.Value);
                    ShowFinishPoll();
                    HideLeaderboardTopDown();
                    MadeTopTenLeaderboard = false;
                }
            }
            SaveLeaderboard();
        }
        else
        {
            HideFinishPoll();
            MadeTopTenLeaderboard = false;
        }
    }

    public void ShowLeaderboardTopDown()
    {
        LeaderboardHidden = false;
        LeaderboardTopDownInstance.gameObject.SetActive(true);
        TxtTitle.ShowObjects();
        TxtSubTitle.ShowObjects();
        TxtPlayNow.ShowObjects();
    }

    public void HideLeaderboardTopDown()
    {
        LeaderboardHidden = true;
        LeaderboardTopDownInstance.gameObject.SetActive(false);
        TxtTitle.HideObjects();
        TxtSubTitle.HideObjects();
        TxtPlayNow.HideObjects();
    }

    public void ShowFinishPoll()
    {
        PollFinishedInstance.gameObject.SetActive(true);
        PollFinishedHidden = false;
        ScreensaverManager.Instance.DiableScreensaver = false;
    }

    public void HideFinishPoll()
    {
        PollFinishedInstance.gameObject.SetActive(false);
        PollFinishedHidden = true;
    }

    public void ShowObjects()
    {
        gameObject.SetActive(true);
        Hidden = false;
    }

    public void HideObjects()
    {
        Hidden = true;
        gameObject.SetActive(false);
    }

    public void Update()
    {
        if (!Hidden && !Loading && ExhibitGameManager.Instance.CurrentGameState != "Screensaver")
        {
            Vector2 touchClickPosition = Vector2.zero;
            if (Input.GetMouseButtonDown(0))
            {
                touchClickPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }
            for (var i = 0; i < Input.touchCount; ++i)
            {
                if (Input.GetTouch(i).phase == TouchPhase.Began)
                {
                    touchClickPosition = Input.GetTouch(i).position;
                }
            }
            if (touchClickPosition != Vector2.zero)
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(touchClickPosition), out hitInfo))
                {
                    if (hitInfo.collider != null)
                    {
                        if (hitInfo.transform)
                        {
                            var buttonComponent = hitInfo.transform.GetComponent<PollButtonComponent>();
                            if (buttonComponent)
                            {
                                buttonComponent.OnClick();
                            }
                        }
                    }
                }
                if (PollFinishedHidden && !LeaderboardHidden)
                {
                    LeaderboardManager.Instance.OnPlay();
                }
            }
        }
    }
}
