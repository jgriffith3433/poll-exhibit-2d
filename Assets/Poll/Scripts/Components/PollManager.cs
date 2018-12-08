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

    public void OnLogin(string displayName, string fullName)
    {
        PollInstance.OnLogin(displayName, fullName);
    }

    public void FinishPoll()
    {
        ExhibitGameManager.Instance.OnFinishPoll();
    }
}
