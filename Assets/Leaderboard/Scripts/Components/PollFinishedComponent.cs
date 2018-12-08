using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PollFinishedComponent : MonoBehaviour {
    public bool submitted;

    public void StartOver()
    {
        LeaderboardManager.Instance.OnPlay();
    }

    public void Submit()
    {
        if (!submitted)
        {
            StartOver();
        }
    }
}
