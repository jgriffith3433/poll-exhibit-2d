using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PollQuestionComponent : MonoBehaviour
{
    private List<PollAnswerComponent> PollAnswerInstances;
    private PollQuestionData Data;
    public PollAnswerComponent AnswerPrefab;
    public PollTextComponent QuestionTextPrefab;
    private PollTextComponent QuestionTextInstance;
    
    public void SetPollQuestionData(PollQuestionData data)
    {
        Data = data;
    }

    public void CreateObjects()
    {
        QuestionTextInstance = Instantiate(QuestionTextPrefab).GetComponent<PollTextComponent>();
        QuestionTextInstance.name = "Question Text";
        QuestionTextInstance.transform.SetParent(transform);
        QuestionTextInstance.transform.position = Data.QuestionTextPosition;
        QuestionTextInstance.SetTextData(Data.QuestionText);
        QuestionTextInstance.CreateAllObjects();

        var answersObject = new GameObject("Answers");
        answersObject.transform.SetParent(transform);
        PollAnswerInstances = new List<PollAnswerComponent>();
        for(var i = 0; i < Data.PollAnswersData.Count; i++)
        {
            var pollAnswerInstance = Instantiate(AnswerPrefab).GetComponent<PollAnswerComponent>();
            pollAnswerInstance.name = "Answer";
            pollAnswerInstance.transform.SetParent(answersObject.transform);
            pollAnswerInstance.SetQuestionParent(this);
            pollAnswerInstance.SetPollAnswerData(Data.PollAnswersData[i]);
            pollAnswerInstance.CreateObjects();
            PollAnswerInstances.Add(pollAnswerInstance);
        }
        gameObject.SetActive(false);
    }

    public void OnCorrect()
    {
        PollManager.Instance.OnCorrect();
    }

    public void OnIncorrect()
    {
        PollManager.Instance.OnIncorrect();
    }

    public void ShowCorrectAnswer()
    {
        for (var i = 0; i < PollAnswerInstances.Count; i++)
        {
            PollAnswerInstances[i].ShowAsCorrectOrIncorrect();
        }
    }

    public void ShowObjects()
    {
        gameObject.SetActive(true);
    }

    public void HideObjects()
    {
        gameObject.SetActive(false);
    }
}
