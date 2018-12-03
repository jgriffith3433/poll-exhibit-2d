using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PollAnswerComponent : MonoBehaviour
{
    public PollTextComponent AnswerTextPrefab;
    private PollTextComponent AnswerTextInstance;

    public PollButtonComponent AnswerButtonTextPrefab;
    private PollButtonComponent AnswerButtonTextInstance;

    private PollQuestionComponent QuestionParent;
    private PollAnswerData Data;

    public bool IsSelected;

    public void SetQuestionParent(PollQuestionComponent questionParent)
    {
        QuestionParent = questionParent;
    }

    public void SetPollAnswerData(PollAnswerData data)
    {
        Data = data;
    }

    public void CreateObjects()
    {
        AnswerTextInstance = Instantiate(AnswerTextPrefab).GetComponent<PollTextComponent>();
        AnswerTextInstance.transform.SetParent(transform);
        AnswerTextInstance.transform.position = Data.AnswerTextPosition;

        AnswerTextInstance.SetTextData(Data.AnswerText);
        AnswerTextInstance.CreateAllObjects();

        AnswerButtonTextInstance = Instantiate(AnswerButtonTextPrefab).GetComponent<PollButtonComponent>();
        AnswerButtonTextInstance.transform.SetParent(transform);
        AnswerButtonTextInstance.transform.position = Data.AnswerButtonTextPosition;
        AnswerButtonTextInstance.SetButtonTextData(Data.AnswerButtonText);
        AnswerButtonTextInstance.CreateAllObjects();
    }

    public void OnChoose()
    {
        IsSelected = true;
        if (Data.Correct)
        {
            QuestionParent.OnCorrect();
        }
        else
        {
            QuestionParent.OnIncorrect();
        }
    }

    public void ShowAsCorrectOrIncorrect()
    {
        if (IsSelected)
        {
            //change font size or translate
        }
        if (Data.Correct)
        {
            AnswerButtonTextInstance.ChangeText("#05FF05");
            AnswerTextInstance.ChangeText("#05FF05");
        }
        else
        {
            AnswerButtonTextInstance.ChangeText("#FF0505");
            AnswerTextInstance.ChangeText("#FF0505");
        }
    }
}
