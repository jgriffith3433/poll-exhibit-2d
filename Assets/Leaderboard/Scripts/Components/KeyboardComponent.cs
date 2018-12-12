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

    public void ShowObjects()
    {
        gameObject.SetActive(true);
        Hidden = false;
    }

    public void HideObjects()
    {
        gameObject.SetActive(false);
        Hidden = true;
    }

    public void PressKey(string keyButtonValue)
    {
        switch(keyButtonValue.ToLower())
        {
            case "continue":
                LeaderboardManager.Instance.OnContinue();
                break;
            case "<":
                if (Value.Length > 0)
                {
                    Value = Value.Substring(0, Value.Length - 1);
                }
                break;
            case "a":
            case "b":
            case "c":
            case "d":
            case "e":
            case "f":
            case "g":
            case "h":
            case "i":
            case "j":
            case "k":
            case "l":
            case "m":
            case "n":
            case "o":
            case "p":
            case "q":
            case "r":
            case "s":
            case "t":
            case "u":
            case "v":
            case "w":
            case "x":
            case "y":
            case "z":
                Value += keyButtonValue;
                break;
            case "space":
                Value += " ";
                break;
            default:
                break;
        }
    }
}
