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

    public string GetButtonText()
    {
        return m_textMeshPro.text;
    }

    public void CreateAllObjects()
    {
        m_textMeshPro.SetText(Text);
    }

    public IEnumerator AnimatePress()
    {
        m_textMeshPro.fontSize += 10;
        yield return new WaitForSeconds(0.2f);
        m_textMeshPro.fontSize -= 10;
    }

    public void OnClick()
    {
        var answerComponent = transform.parent.GetComponent<PollAnswerComponent>();
        if (answerComponent != null)
        {
            answerComponent.OnChoose();
        }
        
        if (transform.parent.parent != null)
        {
            if (transform.parent.parent.parent != null)
            {
                if (transform.parent.parent.parent.parent != null)
                {
                    var keyboardComponent = transform.parent.parent.parent.parent.GetComponent<KeyboardComponent>();
                    if (keyboardComponent != null)
                    {
                        StartCoroutine(AnimatePress());
                        keyboardComponent.PressKey(GetButtonText());
                    }
                }
                var testKnowledgeComponent = transform.parent.parent.parent.GetComponent<TestKnowledgeComponent>();
                if (testKnowledgeComponent != null)
                {
                    testKnowledgeComponent.PlayNow();
                }
            }
        }

        /*
        var databaseComponent = transform.parent.GetComponent<DatabaseManager>();
        if (databaseComponent != null)
        {
            databaseComponent.CombineDatabases();
        }*/

        var pollFinishedComponent = transform.parent.GetComponent<PollFinishedComponent>();
        if (pollFinishedComponent != null)
        {
            LeaderboardManager.Instance.OnPlay();
        }

        var pollComponent = transform.parent.GetComponent<PollComponent>();
        if (pollComponent != null)
        {
            pollComponent.StartOver();
        }
    }
}
