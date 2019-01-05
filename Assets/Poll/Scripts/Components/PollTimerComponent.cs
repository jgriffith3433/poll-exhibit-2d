using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class PollTimerComponent : PollImageSequenceComponent
{
    protected override void Awake()
    {
        ImageSequenceFolder = Application.dataPath + "/Resources/Poll/Images/Timer";
        base.Awake();
        OnSequenceEnded += SequenceEnded;
    }

    protected void SequenceEnded()
    {
        PollManager.Instance.OnTimerEnded();
    }
}
