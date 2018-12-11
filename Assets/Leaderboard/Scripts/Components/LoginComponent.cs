using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class LoginComponent : MonoBehaviour {
    public PollTextComponent LoginTitle;
    public PollButtonComponent SubmitButton;
    public PollTextFieldComponent DisplayNameField;

    public PollTextComponent TxtScore;
    public PollTextComponent TxtTotalTime;


    public bool submitted;

    public void Login(string displayName, string fullName)
    {
        PollManager.Instance.OnLogin(displayName, fullName);
    }

    public void Submit()
    {
        if (!submitted)
        {
            if (DisplayNameField.GetInputValue() != "Enter full name..." && !string.IsNullOrEmpty(DisplayNameField.GetInputValue()))
            {
                string displayName = DisplayNameField.GetInputValue().Trim();
                var nameSplit = displayName.Split(' ');
                
                if (nameSplit.Length > 1 && nameSplit.Length < 5)
                {
                    var initials = "";
                    foreach (var name in nameSplit)
                    {
                        if (name.Length > 0)
                        {
                            initials += name[0].ToString().ToUpper() + ". ";
                        }
                    }
                    displayName = initials.Trim();
                }
                if (displayName.Length < 15)
                {
                    submitted = true;
                    Login(displayName, DisplayNameField.GetInputValue().Trim());
                }
                else
                {
                    DisplayNameField.SetInputValue("");
                }
            }
        }
    }

    public void CreateAllObjects(int score, TimeSpan? totalTime)
    {
        submitted = false;

        TxtScore.SetTextData(score.ToString());
        if (totalTime.HasValue)
        {
            TxtTotalTime.SetTextData(string.Format("{0:D2}:{1:D2}:{2:D2}", totalTime.Value.Hours, totalTime.Value.Minutes, totalTime.Value.Seconds));
        }

        TxtTotalTime.CreateAllObjects();
        TxtScore.CreateAllObjects();
        DisplayNameField.CreateAllObjects();
        SubmitButton.CreateAllObjects();
    }
}
