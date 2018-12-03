using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PollButtonComponent : MonoBehaviour {
    private TextMeshPro m_textMeshPro;
    private string Text = "Submit";

    public void SetButtonTextData(string text)
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
        m_textMeshPro.SetText("<" + color + ">" + Text + "</color>");
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

        var leaderboardComponent = transform.parent.GetComponent<LeaderboardComponent>();
        if (leaderboardComponent != null)
        {
            leaderboardComponent.Play();
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
