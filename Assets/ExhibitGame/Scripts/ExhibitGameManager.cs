﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExhibitGameManager : MonoBehaviour {
    public static ExhibitGameManager Instance { get; private set; }
    
    public string CurrentGameState;
    private string PreviousGameState;
    private bool UseSequenceForBackground = false;
    public bool Connected = false;
    public bool Started = false;

    public Player Player;

    public PollImageSequenceComponent ExhibitBackgroundSequencePrefab;
    private PollImageSequenceComponent ExhibitBackgroundSequenceInstance;

    public PollTextComponent LoadingTextInstance;
    
    public PollImageComponent ExhibitBackgroundPrefab;
    private PollImageComponent ExhibitBackgroundInstance;

    public PollTimerComponent LoadTimerPrefab;
    private PollTimerComponent LoadTimerInstance;

    public PollImageSequenceComponent LoadCorrectPrefab;
    private PollImageSequenceComponent LoadCorrectInstance;

    public PollImageSequenceComponent LoadIncorrectPrefab;
    private PollImageSequenceComponent LoadIncorrectInstance;

    private int PollScore;
    private TimeSpan? PollTotalTime;
    private List<PollUserAnswer> PollUserAnswers;

    public void Awake()
    {
        Instance = this;
    }

    IEnumerator Start()
    {
        if (UseSequenceForBackground)
        {
            ExhibitBackgroundSequenceInstance = Instantiate(ExhibitBackgroundSequencePrefab).GetComponent<PollImageSequenceComponent>();
            ExhibitBackgroundSequenceInstance.transform.SetParent(transform);
            ExhibitBackgroundSequenceInstance.transform.position += new Vector3(0, 0, 6);
            ExhibitBackgroundSequenceInstance.SetImageSequenceFolder("ExhibitGame/Images/background_image_sequence");
            ExhibitBackgroundSequenceInstance.SetLoop(true);
            ExhibitBackgroundSequenceInstance.CreateObjects(true);
        }
        else
        {
            ExhibitBackgroundInstance = Instantiate(ExhibitBackgroundPrefab).GetComponent<PollImageComponent>();
            ExhibitBackgroundInstance.transform.SetParent(transform);
            ExhibitBackgroundInstance.transform.position += new Vector3(0, 0, 6);
            ExhibitBackgroundInstance.CreateObjects("ExhibitGame/Images/BackgroundImage.jpg");
        }
        LoadTimerInstance = Instantiate(LoadTimerPrefab).GetComponent<PollTimerComponent>();
        LoadTimerInstance.transform.SetParent(transform);
        LoadTimerInstance.CreateObjects(false);

        LoadCorrectInstance = Instantiate(LoadCorrectPrefab).GetComponent<PollImageSequenceComponent>();
        LoadCorrectInstance.transform.SetParent(transform);
        LoadCorrectInstance.SetImageSequenceFolder("Poll/Images/CorrectAnswer");
        LoadCorrectInstance.CreateObjects(false);

        LoadIncorrectInstance = Instantiate(LoadIncorrectPrefab).GetComponent<PollImageSequenceComponent>();
        LoadIncorrectInstance.transform.SetParent(transform);
        LoadIncorrectInstance.SetImageSequenceFolder("Poll/Images/IncorrectAnswer");
        LoadIncorrectInstance.CreateObjects(false);
        
        LoadingTextInstance.AnimateFadeIn();
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(CheckIsConnected());
    }

    IEnumerator CheckIsConnected()
    {
        Connected = Player != null;
        if (Connected && !Started)
        {
            Player.CmdLoadInitialData();
            Started = true;
            yield return StartCoroutine(CheckIsDoneLoading());
        }
        yield return new WaitForSeconds(3);
        StartCoroutine(CheckIsConnected());
    }

    IEnumerator CheckIsDoneLoading()
    {
        var stillLoading = false;
        if (UseSequenceForBackground)
        {
            stillLoading = ExhibitBackgroundSequenceInstance.Loading || LoadTimerInstance.Loading || LoadCorrectInstance.Loading || LoadIncorrectInstance.Loading || Player.Loading || ScreensaverManager.Instance.Loading;
        }
        else
        {
            stillLoading = ExhibitBackgroundInstance.Loading || LoadTimerInstance.Loading || LoadCorrectInstance.Loading || LoadIncorrectInstance.Loading || Player.Loading || ScreensaverManager.Instance.Loading;
        }
        if (stillLoading)
        {
            yield return new WaitForSeconds(1);
            yield return StartCoroutine(CheckIsDoneLoading());
        }
        else
        {
            ScreensaverManager.Instance.HideScreensaver();
            LoadingTextInstance.gameObject.SetActive(false);
            LoadTimerInstance.HideObjects();
            LoadCorrectInstance.HideObjects();
            LoadIncorrectInstance.HideObjects();
            GoToState("StartingLeaderboard");
        }
    }

    public void Update()
    {
        switch (CurrentGameState)
        {
            case "StartingStart":
                break;
            case "Start":
                break;
            case "StartingPoll":
                GoToState("Poll");
                break;
            case "Poll":
               break;
            case "StartingLeaderboard":
                GoToState("Leaderboard");
                break;
            case "Leaderboard":
                break;
            case "StartingScreensaver":
                GoToState("Screensaver");
                break;
            case "Screensaver":
                break;
            case "StartingAdmin":
                GoToState("Admin");
                break;
            case "Admin":
                break;
            default:
                break;
        }
    }

    public void ResetGame()
    {
        Player.CmdLoadInitialData();
        PollManager.Instance.DestroyPoll();
        LeaderboardManager.Instance.DestroyLeaderboard();
    }

    public void OnFinishPoll(int score, TimeSpan totalTime, List<PollUserAnswer> userAnswers)
    {
        PollScore = score;
        PollTotalTime = totalTime;
        PollUserAnswers = userAnswers;
        GoToState("StartingLeaderboard");
    }

    public void OnInactive()
    {
        LeaderboardManager.Instance.OnInactive();
        GoToState("StartingScreensaver");
    }

    public void OnActive()
    {
        GoToState("StartingLeaderboard");
    }

    public void OnCancelScreensaver()
    {
        GoToState(PreviousGameState);
    }

    public void OnFinishLeaderboard()
    {
        GoToState("StartingPoll");
    }

    public void OnSaveLeaderboard()
    {
        GoToState("StartingLeaderboard");
    }

    public void OnFinishScreensaver()
    {
        GoToState("StartingLeaderboard");
    }

    public void OnAdminKeysPressed()
    {
        if (CurrentGameState == "Admin")
        {
            DatabaseManager.Instance.HideAdminScreen();
            GoToState(PreviousGameState);
        }
        else
        {
            GoToState("StartingAdmin");
        }
    }

    public void GoToState(string newState)
    {
        var previousState = CurrentGameState;
        CurrentGameState = newState;
        if (newState == "StartingStart")
        {
            PreviousGameState = previousState;
            GoToState("Start");
        }
        else if (newState == "Start")
        {
            GoToState("StartingPoll");
        }
        else if (newState == "StartingPoll")
        {
            PreviousGameState = previousState;
            PollManager.Instance.RestartPoll();
        }
        else if (newState == "Poll")
        {
        }
        else if (newState == "StartingLeaderboard")
        {
            PreviousGameState = previousState;
            ResetGame();
            LeaderboardManager.Instance.ShowLeaderboard(PollScore, PollTotalTime, PollUserAnswers, PreviousGameState == "Poll");
            PollScore = 0;
            PollTotalTime = null;
        }
        else if (newState == "Leaderboard")
        {
        }
        else if (newState == "StartingScreensaver")
        {
            PreviousGameState = previousState;
            ScreensaverManager.Instance.StartingScreensaver();
        }
        else if (newState == "Screensaver")
        {
            ScreensaverManager.Instance.StartScreensaver();
        }
        else if (newState == "StartingAdmin")
        {
            PreviousGameState = previousState;
        }
        else if (newState == "Admin")
        {
            DatabaseManager.Instance.ShowAdminScreen();
        }
    }
}
