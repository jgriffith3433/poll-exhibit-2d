using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PollAnswerComponent : MonoBehaviour
{
    public PollTextComponent AnswerTextPrefab;
    private PollTextComponent AnswerTextInstance;

    public PollButtonComponent AnswerButtonTextPrefab;
    private PollButtonComponent AnswerButtonTextInstance;

    public PollImageComponent AnswerCorrectBackgroundPrefab;
    public PollImageComponent AnswerIncorrectBackgroundPrefab;
    private PollImageComponent CorrectOrIncorrectBackgroundInstance;

    public PollImageSequenceComponent SelectedImageSequencePrefab;
    private PollImageSequenceComponent SelectedImageSequenceInstance;

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

        if (Data.Correct)
        {
            CorrectOrIncorrectBackgroundInstance = Instantiate(AnswerCorrectBackgroundPrefab).GetComponent<PollImageComponent>();
        }
        else
        {
            CorrectOrIncorrectBackgroundInstance = Instantiate(AnswerIncorrectBackgroundPrefab).GetComponent<PollImageComponent>();
        }
        CorrectOrIncorrectBackgroundInstance.transform.SetParent(transform);
        CorrectOrIncorrectBackgroundInstance.transform.position = Data.AnswerButtonTextPosition + new Vector3(0, 0, 0.5f);

        SelectedImageSequenceInstance = Instantiate(SelectedImageSequencePrefab).GetComponent<PollImageSequenceComponent>();
        SelectedImageSequenceInstance.transform.SetParent(transform);
        SelectedImageSequenceInstance.transform.position = Data.AnswerButtonTextPosition + new Vector3(0, 0, 0.5f);
        if (Data.Correct)
        {
            SelectedImageSequenceInstance.SetImageSequenceFolder("Poll/Images/CorrectAnswer");
        }
        else
        {
            SelectedImageSequenceInstance.SetImageSequenceFolder("Poll/Images/IncorrectAnswer");
        }
        SelectedImageSequenceInstance.CreateObjects(false);
        SelectedImageSequenceInstance.SetLoop(false);
        HideObjects();
    }

    public void HideObjects()
    {
        AnswerTextInstance.gameObject.SetActive(false);
        AnswerButtonTextInstance.gameObject.SetActive(false);
        CorrectOrIncorrectBackgroundInstance.HideObjects();
    }

    public void ShowObjects()
    {
        AnswerTextInstance.gameObject.SetActive(true);
        AnswerButtonTextInstance.gameObject.SetActive(true);
    }

    public void OnChoose()
    {
        IsSelected = true;
        QuestionParent.OnSelectedAnswer(this, Data.AnswerId, Data.Correct);
        SelectedImageSequenceInstance.Play();
    }

    public void ShowAsCorrectOrIncorrect()
    {
        CorrectOrIncorrectBackgroundInstance.gameObject.SetActive(true);
    }
}
