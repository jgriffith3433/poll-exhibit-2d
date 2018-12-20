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

    public LoginComponent LoginPrefab;
    private LoginComponent LoginInstance;

    public PollFinishedComponent PollFinishedPrefab;
    private PollFinishedComponent PollFinishedInstance;

    private int Score;
    private TimeSpan? TotalTime;
    private List<PollUserAnswer> UserAnswers;
    private bool FromPoll;

    private bool Loading = true;
    private bool LoginHidden = false;
    private bool PollFinishedHidden = false;
    private bool LeaderboardHidden = false;

    public bool Hidden;

    public void ShowLeaderboard(int score, TimeSpan? totalTime, List<PollUserAnswer> userAnswers, bool fromPoll)
    {
        UserAnswers = userAnswers;
        FromPoll = fromPoll;
        Score = score;
        TotalTime = totalTime;

        if (fromPoll)
        {
            SavePlayerData();
        }

        Loading = true;
        ShowObjects();
        ExhibitGameManager.Instance.Player.GetDatabaseString();
        Data = new LeaderboardData();
        Data.SetData(ExhibitGameManager.Instance.Player.GetLeaderboardString());
        StartCoroutine(CheckIsDoneParsing());
    }

    public void OnLogin(string displayName, string fullName, string email, string phoneNumber)
    {
        ExhibitGameManager.Instance.Player.CmdSaveLeaderboard(displayName, fullName, Score, TotalTime.Value.ToString(), email, phoneNumber);
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
        yield return new WaitForSeconds(0.5f);
        ExhibitGameManager.Instance.OnSaveLeaderboard();
    }

    IEnumerator CheckIsDoneParsing()
    {
        if (Data != null && Data.IsDoneParsing)
        {
            CreateObjects();
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
        LoginInstance = Instantiate(LoginPrefab).GetComponent<LoginComponent>();
        LoginInstance.transform.SetParent(transform);
        LoginInstance.CreateAllObjects(Score, TotalTime);
        PollFinishedInstance = Instantiate(PollFinishedPrefab).GetComponent<PollFinishedComponent>();
        PollFinishedInstance.transform.SetParent(transform);
        if (FromPoll)
        {
            var lowestScore = Data.PlayerData.Count == 10 ? Data.PlayerData.LastOrDefault() : null;
            if (lowestScore != null)
            {
                if (Score > 0 && Score / TotalTime.Value.TotalSeconds > lowestScore.PlayerScore / lowestScore.TotalTime.TotalSeconds)
                {
                    ShowLogin();
                    HideLeaderboardTopDown();
                    HideFinishPoll();
                }
                else
                {
                    PollFinishedInstance.SetValues(Score, TotalTime.Value);
                    ShowFinishPoll();
                    HideLeaderboardTopDown();
                    HideLogin();
                }
            }
            else
            {
                if (Score > 0)
                {
                    ShowLogin();
                    HideLeaderboardTopDown();
                    HideFinishPoll();
                }
                else
                {
                    PollFinishedInstance.SetValues(Score, TotalTime.Value);
                    ShowFinishPoll();
                    HideLeaderboardTopDown();
                    HideLogin();
                }
            }
        }
        else
        {
            HideFinishPoll();
            HideLogin();
        }
    }

    public void OnEnter()
    {
        LoginInstance.OnEnter();
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

    public void ShowLogin()
    {
        LoginInstance.gameObject.SetActive(true);
        LoginHidden = false;
    }

    public void HideLogin()
    {
        LoginInstance.gameObject.SetActive(false);
        LoginHidden = true;
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
        if (!Hidden || !LoginHidden && !Loading && ExhibitGameManager.Instance.CurrentGameState != "Screensaver")
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
                if (LoginHidden && PollFinishedHidden && !LeaderboardHidden)
                {
                    LeaderboardManager.Instance.OnPlay();
                }
            }
        }
    }
}
