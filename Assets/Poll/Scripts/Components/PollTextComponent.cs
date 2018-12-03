using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PollTextComponent : MonoBehaviour {
    private TextMeshPro m_textMeshPro;
    private string Text;

    public void SetTextData(string text)
    {
        Text = text;
    }

    public void CreateAllObjects()
    {
        m_textMeshPro = gameObject.GetComponent<TextMeshPro>();
        ChangeText("#FFFFFF");
    }

    public void ChangeText(string color)
    {
        m_textMeshPro.SetText("<" + color  + ">" + Text + "</color>");
    }
}
