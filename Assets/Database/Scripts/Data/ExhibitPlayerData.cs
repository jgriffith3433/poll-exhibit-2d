using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExhibitPlayerData
{
    public string PlayerDisplayName { get; set; }
    public int PlayerScore { get; set; }
    public TimeSpan TotalTime { get; set; }
    public List<PlayerAnswerData> PlayerAnswerData { get; set; }
}
