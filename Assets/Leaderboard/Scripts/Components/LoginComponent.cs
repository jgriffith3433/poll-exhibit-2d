using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class LoginComponent : MonoBehaviour {
    public PollTextComponent LoginTitle;
    public KeyboardComponent KeyboardInstance;

    public PollTextComponent TxtScore;
    public PollTextComponent TxtTotalTime;

    public PollTextFieldComponent NameInput;
    public bool NameInputSelected;
    public PollTextFieldComponent EmailInput;
    public bool EmailInputSelected;

    public bool submitted;

    public void Awake()
    {
        KeyboardInstance.OnValueChanged += OnKeyboardValueChanged;
    }

    public void Login(string displayName, string fullName)
    {
        LeaderboardManager.Instance.OnLogin(displayName, fullName);
    }

    public void OnContinue()
    {
        Submit();
    }

    public void OnKeyboardValueChanged()
    {
        if (NameInputSelected)
        {
            NameInput.SetInputValue(KeyboardInstance.Value);
        }
        else if (EmailInputSelected)
        {
            EmailInput.SetInputValue(KeyboardInstance.Value);
        }
    }

    private void Submit()
    {
        if (!submitted)
        {
            if (NameInput.GetInputValue() != "First Last Name" && !string.IsNullOrEmpty(NameInput.GetInputValue()))
            {
                var displayName = NameInput.GetInputValue().Trim();
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
                    Login(displayName, NameInput.GetInputValue().Trim());
                }
                else
                {
                    NameInput.SetInputValue("");
                    KeyboardInstance.Value = "";
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
            TxtTotalTime.SetTextData(string.Format("{0:N0}:{1:N0}:{2:N0}", totalTime.Value.Hours, totalTime.Value.Minutes, totalTime.Value.Seconds));
        }

        TxtTotalTime.CreateAllObjects();
        TxtScore.CreateAllObjects();
        NameInput.CreateAllObjects();
        EmailInput.CreateAllObjects();

        KeyboardInstance.HideObjects();
    }

    public void Update()
    {
        Vector2 touchClickPosition = Vector2.zero;
        if (Input.GetMouseButtonDown(0))
        {
            touchClickPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        for (var i = 0; i < Input.touchCount; ++i)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                touchClickPosition = Input.GetTouch(i).position;
            }
        }
        if (touchClickPosition != Vector2.zero)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(touchClickPosition), out hitInfo))
            {
                if (hitInfo.collider != null)
                {
                    if (hitInfo.transform)
                    {
                        var textFieldComponent = hitInfo.transform.GetComponent<PollTextFieldComponent>();
                        if (textFieldComponent)
                        {
                            if (textFieldComponent == NameInput)
                            {
                                KeyboardInstance.ShowObjects();
                                NameInputSelected = true;
                                EmailInputSelected = false;
                                KeyboardInstance.Value = NameInput.GetInputValue();
                            }
                            else if (textFieldComponent == EmailInput)
                            {
                                KeyboardInstance.ShowObjects();
                                NameInputSelected = false;
                                EmailInputSelected = true;
                                KeyboardInstance.Value = EmailInput.GetInputValue();
                            }
                        }
                    }
                }
            }
        }
    }
}
