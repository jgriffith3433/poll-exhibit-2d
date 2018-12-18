﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PollFinishedComponent : MonoBehaviour
{
    public int FramesSinceUpdateValues = 0;

    private int Score;
    private int CurrentScore;

    private TimeSpan TotalTime;
    private int CurrentHours;
    private int CurrentMinutes;
    private int CurrentSeconds;

    public PollTextComponent CorrectAnswersText;
    public PollTextComponent TotalTimeLabelText;

    public void SetValues(int score, TimeSpan totalTime)
    {
        Score = score;
        TotalTime = totalTime;
    }

    public void Update()
    {
        if (FramesSinceUpdateValues > 5)
        {
            FramesSinceUpdateValues = 0;
            if (CurrentScore < Score)
            {
                CurrentScore++;
            }
            else
            {
                if (CurrentHours < TotalTime.Hours)
                {
                    CurrentHours++;
                }
                else
                {
                    if (CurrentMinutes < TotalTime.Minutes)
                    {
                        CurrentMinutes++;
                    }
                    else
                    {
                        if (CurrentSeconds < TotalTime.Seconds)
                        {
                            CurrentSeconds++;
                        }
                    }
                }
            }
            if (CurrentScore < 10)
            {
                CorrectAnswersText.SetTextData("0" + CurrentScore.ToString());
            }
            else
            {
                CorrectAnswersText.SetTextData(CurrentScore.ToString());
            }
            CorrectAnswersText.CreateAllObjects();
            TotalTimeLabelText.SetTextData(string.Format("{0:N0}:{1:N0}:{2:N0}", 
                CurrentHours < 10 ? "0" + CurrentHours.ToString() : CurrentHours.ToString(), 
                CurrentMinutes < 10 ? "0" + CurrentMinutes.ToString() : CurrentMinutes.ToString(),
                CurrentSeconds < 10 ? "0" + CurrentSeconds.ToString() : CurrentSeconds.ToString()));
            TotalTimeLabelText.CreateAllObjects();
        }
        FramesSinceUpdateValues++;
    }
}
