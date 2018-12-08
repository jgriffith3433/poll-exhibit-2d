﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExhibitGameManager : MonoBehaviour {
    public static ExhibitGameManager Instance { get; private set; }
    
    public string CurrentGameState;
    private string PreviousGameState;
    private bool UseSequenceForBackground = false;
    public PollImageSequenceComponent ExhibitBackgroundSequencePrefab;
    private PollImageSequenceComponent ExhibitBackgroundSequenceInstance;

    public PollImageComponent ExhibitBackgroundPrefab;
    private PollImageComponent ExhibitBackgroundInstance;

    public void Awake()
    {
        Instance = this;
        if (UseSequenceForBackground)
        {
            ExhibitBackgroundSequenceInstance = Instantiate(ExhibitBackgroundSequencePrefab).GetComponent<PollImageSequenceComponent>();
            ExhibitBackgroundSequenceInstance.transform.SetParent(transform);
            ExhibitBackgroundSequenceInstance.transform.position += new Vector3(0, 0, 2);
            ExhibitBackgroundSequenceInstance.SetImageSequenceFolder("background_image_sequence");
            ExhibitBackgroundSequenceInstance.SetLoop(true);
        }
        else
        {
            ExhibitBackgroundInstance = Instantiate(ExhibitBackgroundPrefab).GetComponent<PollImageComponent>();
            ExhibitBackgroundInstance.transform.SetParent(transform);
            ExhibitBackgroundInstance.transform.position += new Vector3(0, 0, 2);
            ExhibitBackgroundInstance.CreateObjects("ExhibitGame/Images/BackgroundImage.jpg");
        }
    }

    IEnumerator Start()
    {
        if (UseSequenceForBackground)
        {
            ExhibitBackgroundSequenceInstance.CreateObjects(true);
        }
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(CheckIsDoneLoading());
    }

    IEnumerator CheckIsDoneLoading()
    {
        var stillLoading = false;
        if (UseSequenceForBackground)
        {
            stillLoading = ExhibitBackgroundSequenceInstance.Loading;
        }
        else
        {
            stillLoading = ExhibitBackgroundInstance.Loading;
        }
        if (stillLoading)
        {
            yield return new WaitForSeconds(1);
            yield return StartCoroutine(CheckIsDoneLoading());
        }
        else
        {
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
        PollManager.Instance.DestroyPoll();
        LeaderboardManager.Instance.DestroyLeaderboard();
    }

    public void OnFinishPoll()
    {
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
            LeaderboardManager.Instance.ShowLeaderboard(0, 0.0f);
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
