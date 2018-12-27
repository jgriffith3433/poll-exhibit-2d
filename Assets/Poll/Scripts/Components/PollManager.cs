using System;
using System.Collections;
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

    public void Awake()
    {
        _instance = this;
    }

    public void RestartPoll()
    {
        PollInstance = Instantiate(PollPrefab).GetComponent<PollComponent>();
        PollInstance.RestartPoll();
    }

    public void BeginPoll(string displayName, string firstName, string lastName)
    {
        PollInstance.Login(displayName, firstName, lastName);
        PollInstance.BeginPoll();
    }

    public void DestroyPoll()
    {
        if (PollInstance)
        {
            Destroy(PollInstance.gameObject);
        }
    }

    public void OnCorrect(int questionId, int answerId)
    {
        PollInstance.OnCorrect(questionId, answerId);
    }

    public void OnIncorrect(int questionId, int answerId)
    {
        PollInstance.OnIncorrect(questionId, answerId);
    }

    public void OnTimerEnded()
    {
        PollInstance.OnTimerEnded();
    }

    public void FinishPoll(int score, TimeSpan totalTime, List<PollUserAnswer> userAnswers, string displayName, string firstName, string lastName)
    {
        ExhibitGameManager.Instance.OnFinishPoll(score, totalTime, userAnswers, displayName, firstName, lastName);
    }
}
