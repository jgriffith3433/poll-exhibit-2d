using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeyboardComponent : MonoBehaviour
{
    public delegate void OnValueChangedDelegate();
    public OnValueChangedDelegate OnValueChanged;
    private string _value;
    public string Value { get {
            return _value;
        }
        set {
            _value = value;
            if (OnValueChanged != null)
            {
                OnValueChanged();
            }
        }
    }
    public bool Hidden = false;
    public bool Capitalize = true;
    public GameObject Symbols;
    public GameObject Capital_A_B_Cs;
    public GameObject Lower_A_B_Cs;

    public void ShowObjects()
    {
        gameObject.SetActive(true);
        Hidden = false;
    }

    public void ShowCaptial()
    {
        Capital_A_B_Cs.SetActive(true);
        Lower_A_B_Cs.SetActive(false);
        Capitalize = true;
    }

    public void ShowLower()
    {
        Capital_A_B_Cs.SetActive(false);
        Lower_A_B_Cs.SetActive(true);
        Capitalize = false;
    }

    public void ShowABCs()
    {
        Symbols.SetActive(false);
        if (Capitalize)
        {
            ShowCaptial();
        }
        else
        {
            ShowLower();
        }
    }

    public void ShowSymbols()
    {
        Symbols.SetActive(true);
        Capital_A_B_Cs.SetActive(false);
        Lower_A_B_Cs.SetActive(false);
    }

    public void HideObjects()
    {
        gameObject.SetActive(false);
        Hidden = true;
    }

    public void PressKey(string keyButtonValue)
    {
        switch (keyButtonValue.ToLower())
        {
            case "enter":
                LeaderboardManager.Instance.OnEnter();
                break;
            case "123 sym":
                ShowSymbols();
                break;
            case "abc":
                ShowABCs();
                break;
            case "capitalize":
                if (Capitalize)
                {
                    ShowLower();
                }
                else
                {
                    ShowCaptial();
                }
                break;
            case "backspace":
                if (Value.Length > 0)
                {
                    Value = Value.Substring(0, Value.Length - 1);
                }
                break;
            case "space":
                Value += " ";
                ShowCaptial();
                break;
            default:
                Value += keyButtonValue;
                if (Capitalize)
                {
                    ShowLower();
                }
                break;
        }
    }
}
