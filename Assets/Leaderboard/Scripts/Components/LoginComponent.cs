using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class LoginComponent : MonoBehaviour {
    public KeyboardComponent KeyboardInstance;

    public PollTextFieldComponent FirstNameInput;
    public bool FirstNameInputSelected;
    public PollTextFieldComponent LastNameInput;
    public bool LastNameInputSelected;

    public bool ValuesFilled = false;

    public string DisplayName;
    public string FirstName;
    public string LastName;
    

    public void Awake()
    {
        KeyboardInstance.OnValueChanged += OnKeyboardValueChanged;
    }

    public void OnEnter()
    {
        //Submit();
    }

    public void OnKeyboardValueChanged()
    {
        if (FirstNameInputSelected)
        {
            FirstNameInput.SetInputValue(KeyboardInstance.Value);
        }
        else if (LastNameInputSelected)
        {
            LastNameInput.SetInputValue(KeyboardInstance.Value);
        }
    }

    public void FillValues()
    {
        ValuesFilled = false;
        if (FirstNameInput.GetInputValue() != "FIRST NAME*" && !string.IsNullOrEmpty(FirstNameInput.GetInputValue().Trim()) &&
            LastNameInput.GetInputValue() != "LAST NAME*" && !string.IsNullOrEmpty(LastNameInput.GetInputValue().Trim()))
        {
            var displayName = FirstNameInput.GetInputValue().Trim() + " " + LastNameInput.GetInputValue().Trim();
            var nameSplit = displayName.Split(' ');
                
            if (nameSplit.Length > 1 && nameSplit.Length < 5)
            {
                var firstName = true;
                var initials = "";
                foreach (var name in nameSplit)
                {
                    if (name.Length > 0)
                    {
                        if (firstName)
                        {
                            initials += name.ToString() + " ";
                            firstName = false;
                        }
                        else
                        {
                            initials += name[0].ToString().ToUpper() + ". ";
                        }
                    }
                }
                displayName = initials.Trim();
            }
            if (displayName.Length < 15)
            {
                ValuesFilled = true;
                DisplayName = displayName;
                FirstName = FirstNameInput.GetInputValue().Trim();
                LastName = LastNameInput.GetInputValue().Trim();
            }
            else
            {
                FirstNameInput.SetInputValue("");
                LastNameInput.SetInputValue("");
                KeyboardInstance.Value = "";
            }
        }
    }

    public void CreateAllObjects()
    {
        ValuesFilled = false;

        FirstNameInput.CreateAllObjects();
        LastNameInput.CreateAllObjects();

        KeyboardInstance.ShowObjects();
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
                            if (textFieldComponent == FirstNameInput)
                            {
                                KeyboardInstance.ShowObjects();
                                FirstNameInputSelected = true;
                                LastNameInputSelected = false;
                                KeyboardInstance.Value = FirstNameInput.GetInputValue();
                            }
                            else if (textFieldComponent == LastNameInput)
                            {
                                KeyboardInstance.ShowObjects();
                                FirstNameInputSelected = false;
                                LastNameInputSelected = true;
                                KeyboardInstance.Value = LastNameInput.GetInputValue();
                            }
                            KeyboardInstance.OnSelectedNewInput();
                        }
                    }
                }
            }
        }
    }
}
