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
    public PollTextFieldComponent PhoneInput;
    public bool PhoneInputSelected;

    public bool submitted;

    public void Awake()
    {
        KeyboardInstance.OnValueChanged += OnKeyboardValueChanged;
    }

    public void Login(string displayName, string fullName, string email, string phoneNumber)
    {
        LeaderboardManager.Instance.OnLogin(displayName, fullName, email, phoneNumber);
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
        else if (PhoneInputSelected)
        {
            PhoneInput.SetInputValue(KeyboardInstance.Value);
        }
    }

    private void Submit()
    {
        if (!submitted)
        {
            if (NameInput.GetInputValue() != "FIRST LAST NAME*" && !string.IsNullOrEmpty(NameInput.GetInputValue()) &&
                EmailInput.GetInputValue() != "EMAIL*" && !string.IsNullOrEmpty(EmailInput.GetInputValue()) &&
                PhoneInput.GetInputValue() != "PHONE*" && !string.IsNullOrEmpty(PhoneInput.GetInputValue()))
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
                    Login(displayName, NameInput.GetInputValue().Trim(), EmailInput.GetInputValue().Trim(), PhoneInput.GetInputValue().Trim());
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

        if (score < 10)
        {
            TxtScore.SetTextData("0" + score.ToString());
        }
        else
        {
            TxtScore.SetTextData(score.ToString());
        }

        if (totalTime.HasValue)
        {
            TxtTotalTime.SetTextData(string.Format("{0:N0}:{1:N0}:{2:N0}",
                totalTime.Value.Hours < 10 ? "0" + totalTime.Value.Hours.ToString() : totalTime.Value.Hours.ToString(),
                totalTime.Value.Minutes < 10 ? "0" + totalTime.Value.Minutes.ToString() : totalTime.Value.Minutes.ToString(),
                totalTime.Value.Seconds < 10 ? "0" + totalTime.Value.Seconds.ToString() : totalTime.Value.Seconds.ToString()));
        }

        TxtTotalTime.CreateAllObjects();
        TxtScore.CreateAllObjects();
        NameInput.CreateAllObjects();
        EmailInput.CreateAllObjects();
        PhoneInput.CreateAllObjects();

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
                                PhoneInputSelected = false;
                                KeyboardInstance.Value = NameInput.GetInputValue();
                            }
                            else if (textFieldComponent == EmailInput)
                            {
                                KeyboardInstance.ShowObjects();
                                NameInputSelected = false;
                                EmailInputSelected = true;
                                PhoneInputSelected = false;
                                KeyboardInstance.Value = EmailInput.GetInputValue();
                            }
                            else if (textFieldComponent == PhoneInput)
                            {
                                KeyboardInstance.ShowObjects();
                                NameInputSelected = false;
                                EmailInputSelected = false;
                                PhoneInputSelected = true;
                                KeyboardInstance.Value = PhoneInput.GetInputValue();
                            }
                        }
                    }
                }
            }
        }
    }
}
