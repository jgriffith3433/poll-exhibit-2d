using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PollButtonComponent : MonoBehaviour {
    private TextMeshPro m_textMeshPro;
    private string Text = "Submit";

    public void Awake()
    {
        m_textMeshPro = gameObject.GetComponent<TextMeshPro>();
    }

    public void SetButtonTextData(string text)
    {
        Text = text;
    }

    public void CreateAllObjects()
    {
        m_textMeshPro.SetText(Text);
    }

    public void OnClick()
    {
        var answerComponent = transform.parent.GetComponent<PollAnswerComponent>();
        if (answerComponent != null)
        {
            answerComponent.OnChoose();
        }

        var loginComponent = transform.parent.GetComponent<LoginComponent>();
        if (loginComponent != null)
        {
            loginComponent.Submit();
        }

        var databaseComponent = transform.parent.GetComponent<DatabaseManager>();
        if (databaseComponent != null)
        {
            databaseComponent.CombineDatabases();
        }

        var pollComponent = transform.parent.GetComponent<PollComponent>();
        if (pollComponent != null)
        {
            pollComponent.StartOver();
        }
    }
}
