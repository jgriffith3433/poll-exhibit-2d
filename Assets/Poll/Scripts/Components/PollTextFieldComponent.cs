using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PollTextFieldComponent : MonoBehaviour {
    private TMP_InputField m_textMeshPro;
    
    public void CreateAllObjects()
    {
        m_textMeshPro = gameObject.GetComponent<TMP_InputField>();
    }

    public string GetInputValue()
    {
        return m_textMeshPro.text;
    }

    public void SetInputValue(string newVal)
    {
        m_textMeshPro.text = newVal;
    }
}
