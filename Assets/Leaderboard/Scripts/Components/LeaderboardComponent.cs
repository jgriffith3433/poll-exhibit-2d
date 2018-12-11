using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeaderboardComponent : MonoBehaviour
{
    private LeaderboardData Data;

    public LeadboardTopDownComponent LeadboardTopDownPrefab;
    private LeadboardTopDownComponent LeadboardTopDownInstance;

    public PollTextComponent TxtTitle;
    public PollTextComponent TxtSubTitle;
    public PollTextComponent TxtPlayNow;

    public LoginComponent LoginPrefab;
    private LoginComponent LoginInstance;

    public PollFinishedComponent PollFinishedPrefab;
    private PollFinishedComponent PollFinishedInstance;

    private int Score;
    private TimeSpan? TotalTime;
    private bool FromPoll;

    private bool Loading = true;
    private bool LoginHidden = false;
    private bool PollFinishedHidden = false;

    public bool Hidden;

    public void ShowLeaderboard(int score, TimeSpan? totalTime, bool fromPoll)
    {
        FromPoll = fromPoll;
        Score = score;
        TotalTime = totalTime;
        Loading = true;
        ShowObjects();
        Data = new LeaderboardData(Application.dataPath + "/poll-leaderboard.json");
        StartCoroutine(Data.GetData());
        StartCoroutine(CheckIsDoneParsing());
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
        LeadboardTopDownInstance = Instantiate(LeadboardTopDownPrefab).GetComponent<LeadboardTopDownComponent>();
        LeadboardTopDownInstance.transform.SetParent(transform);
        LeadboardTopDownInstance.SetPlayerData(Data.PlayerData);
        LeadboardTopDownInstance.CreateObjects();
        LoginInstance = Instantiate(LoginPrefab).GetComponent<LoginComponent>();
        LoginInstance.transform.SetParent(transform);
        LoginInstance.CreateAllObjects(Score, TotalTime);
        PollFinishedInstance = Instantiate(PollFinishedPrefab).GetComponent<PollFinishedComponent>();
        PollFinishedInstance.transform.SetParent(transform);
        if (FromPoll)
        {
            var lowestScore = Data.PlayerData.LastOrDefault();
            if (lowestScore != null)
            {
                if (Score / TotalTime.Value.TotalSeconds > lowestScore.PlayerScore / lowestScore.TotalTime.TotalSeconds)
                {
                    ShowLogin();
                    HideLeadboardTopDown();
                    HideFinishPoll();
                }
                else
                {
                    PollFinishedInstance.SetValues(Score, TotalTime.Value);
                    HideLogin();
                }
            }
            else
            {
                ShowLogin();
                HideLeadboardTopDown();
                HideFinishPoll();
            }
        }
        else
        {
            HideFinishPoll();
            HideLogin();
        }
    }

    public void ShowLeadboardTopDown()
    {
        LeadboardTopDownInstance.gameObject.SetActive(true);
        TxtTitle.ShowObjects();
        TxtSubTitle.ShowObjects();
        TxtPlayNow.ShowObjects();
    }

    public void HideLeadboardTopDown()
    {
        LeadboardTopDownInstance.gameObject.SetActive(false);
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
                if (LoginHidden && PollFinishedHidden)
                {
                    LeaderboardManager.Instance.OnPlay();
                }
            }
        }
    }
}
