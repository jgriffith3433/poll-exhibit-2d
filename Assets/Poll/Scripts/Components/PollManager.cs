﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollManager : MonoBehaviour {
    private static PollManager _instance;
    public static PollManager Instance {
        get {
            return _instance;
        }
    }

    public PollComponent PollPrefab;
    private PollComponent PollInstance;
    private string DisplayName;

    public void Awake()
    {
        _instance = this;
    }

    public void RestartPoll()
    {
        PollInstance = Instantiate(PollPrefab).GetComponent<PollComponent>();
        PollInstance.RestartPoll();
    }

    public void HideLogin()
    {
        PollInstance.HideLogin();
    }

    public void DestroyPoll()
    {
        if (PollInstance)
        {
            Destroy(PollInstance.gameObject);
        }
    }

    public void OnCorrect()
    {
        PollInstance.OnCorrect();
    }

    public void OnIncorrect()
    {
        PollInstance.OnIncorrect();
    }

    public void OnLogin(string displayName, string fullName)
    {
        PollInstance.OnLogin(displayName, fullName);
    }

    public void OnSaveLeaderboard()
    {
        ExhibitGameManager.Instance.OnFinishPoll();
    }
}
