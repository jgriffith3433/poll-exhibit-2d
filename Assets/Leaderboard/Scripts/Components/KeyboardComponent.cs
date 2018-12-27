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

    public void ShowCapital()
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

    public IEnumerator WaitAndShowLower()
    {
        yield return new WaitForSeconds(0.2f);
        ShowLower();
    }

    public IEnumerator WaitAndShowCapital()
    {
        yield return new WaitForSeconds(0.2f);
        ShowCapital();
    }

    public void OnSelectedNewInput()
    {
        if (!string.IsNullOrEmpty(Value))
        {
            ShowLower();
        }
        else
        {
            ShowCapital();
        }
    }

    public void ShowABCs()
    {
        Symbols.SetActive(false);
        if (Capitalize)
        {
            StartCoroutine(WaitAndShowCapital());
        }
        else
        {
            StartCoroutine(WaitAndShowLower());
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
                    StartCoroutine(WaitAndShowLower());
                }
                else
                {
                    StartCoroutine(WaitAndShowCapital());
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
                ShowCapital();
                break;
            default:
                Value += keyButtonValue;
                if (Capitalize)
                {
                    StartCoroutine(WaitAndShowLower());
                }
                break;
        }
    }
}
