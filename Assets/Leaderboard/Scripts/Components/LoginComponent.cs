using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoginComponent : MonoBehaviour {
    public PollTextComponent LoginTitle;
    public PollButtonComponent SubmitButton;
    public PollTextFieldComponent DisplayNameField;
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

    public void CreateAllObjects()
    {
        submitted = false;
        DisplayNameField.CreateAllObjects();
        SubmitButton.CreateAllObjects();
    }
}
