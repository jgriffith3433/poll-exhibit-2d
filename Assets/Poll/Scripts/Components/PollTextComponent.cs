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
        if (m_textMeshPro == null)
        {
            m_textMeshPro = gameObject.GetComponentInChildren<TextMeshPro>();
        }
    }

    public void SetTextData(string text)
    {
        Text = text;
    }

    public void CreateAllObjects()
    {
        m_textMeshPro.SetText(Text);
    }

    public void HideObjects()
    {
        gameObject.SetActive(false);
    }

    public void ShowObjects()
    {
        gameObject.SetActive(true);
    }
}
