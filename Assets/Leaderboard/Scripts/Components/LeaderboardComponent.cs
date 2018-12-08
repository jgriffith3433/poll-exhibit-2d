using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardComponent : MonoBehaviour
{
    public LeadboardTopDownComponent LeadboardTopDownPrefab;
    private LeadboardTopDownComponent LeadboardTopDownInstance;
    private LeaderboardData Data;

    public LoginComponent LoginPrefab;
    private LoginComponent LoginInstance;

    public PollFinishedComponent PollFinishedPrefab;
    private PollFinishedComponent PollFinishedInstance;

    private int Score;
    private float TotalTime;

    private bool Loading = true;
    private bool LoginHidden = false;
    private bool PollFinishedHidden = false;

    public bool Hidden;

    public void ShowLeaderboard(int score, float totalTime)
    {
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
        LoginInstance.CreateAllObjects();
        PollFinishedInstance = Instantiate(PollFinishedPrefab).GetComponent<PollFinishedComponent>();
        PollFinishedInstance.transform.SetParent(transform);
        HideFinishPoll();
        HideLogin();
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
