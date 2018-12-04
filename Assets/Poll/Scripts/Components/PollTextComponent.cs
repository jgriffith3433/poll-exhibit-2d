using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PollTextComponent : MonoBehaviour {
    private TextMeshPro m_textMeshPro;
    private string Text;

    public void Awake()
    {
        m_textMeshPro = gameObject.GetComponent<TextMeshPro>();
    }

    public void SetTextData(string text)
    {
        Text = text;
    }

    public void CreateAllObjects()
    {
        ChangeText("#FFFFFF");
    }

    public void HideObjects()
    {
        gameObject.SetActive(false);
    }

    public void ShowObjects()
    {
        gameObject.SetActive(true);
    }

    public void ChangeText(string color)
    {
        m_textMeshPro.SetText("<" + color  + ">" + Text + "</color>");
    }
}
