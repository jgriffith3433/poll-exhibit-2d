using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class PollQuestionComponent : MonoBehaviour
{
    private List<PollAnswerComponent> PollAnswerInstances;
    private PollQuestionData Data;
    public PollAnswerComponent AnswerPrefab;
    public PollTextComponent QuestionTextPrefab;
    private PollTextComponent QuestionTextInstance;

    public PollConfirmationSingle BubbleSingleConfirmationPrefab;
    public PollConfirmationPie PieConfirmationPrefab;
    public PollConfirmationBar BarConfirmationPrefab;

    private PollConfirmation ConfirmationInstance;

    public PollImageComponent AnswerBackgroundPrefab;
    private PollImageComponent AnswerBackgroundInstance;
    private PollTimerComponent AnswerTimerInstance;

    private bool TransitioningAnswersIn = false;
    private bool TransitioningAnswersOut = false;

    private PollAnswerComponent SelectedAnswer;

    private GameObject AnswersObject;

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

        AnswersObject = new GameObject("Answers");
        AnswersObject.transform.SetParent(transform);
        PollAnswerInstances = new List<PollAnswerComponent>();
        for(var i = 0; i < Data.PollAnswersData.Count; i++)
        {
            var pollAnswerInstance = Instantiate(AnswerPrefab).GetComponent<PollAnswerComponent>();
            pollAnswerInstance.name = "Answer";
            pollAnswerInstance.transform.SetParent(AnswersObject.transform);
            pollAnswerInstance.SetQuestionParent(this);
            pollAnswerInstance.SetPollAnswerData(Data.PollAnswersData[i]);
            pollAnswerInstance.CreateObjects();
            PollAnswerInstances.Add(pollAnswerInstance);
        }
        AnswerBackgroundInstance = Instantiate(AnswerBackgroundPrefab, AnswersObject.transform).GetComponent<PollImageComponent>();
        AnswerTimerInstance = AnswerBackgroundInstance.GetComponentInChildren<PollTimerComponent>();
        AnswerTimerInstance.CreateObjects(false);
        AnswersObject.transform.position += new Vector3(100, AnswersObject.transform.position.y, AnswersObject.transform.position.z);

        HideObjects();
    }

    public void OnSelectedAnswer(PollAnswerComponent selectedAnswer, int answerId, bool correct)
    {
        foreach(var answer in PollAnswerInstances)
        {
            answer.Disabled = true;
        }
        SelectedAnswer = selectedAnswer;
        
        if (correct)
        {
            PollManager.Instance.OnCorrect(Data.QuestionId, answerId);
        }
        else
        {
            PollManager.Instance.OnIncorrect(Data.QuestionId, answerId);
        }
        AnswerTimerInstance.Pause();
    }

    public void OnTimerEnded()
    {
        PollManager.Instance.OnIncorrect(Data.QuestionId, -1);
    }

    public void ShowAsCorrectOrIncorrect()
    {
        QuestionTextInstance.SetTextData(Data.QuestionTextConfirmation);
        QuestionTextInstance.CreateAllObjects();
        QuestionTextInstance.AnimateFadeIn(5);

        if (SelectedAnswer != null)
        {
            SelectedAnswer.ShowAsCorrectOrIncorrect();
        }
        foreach(var answer in PollAnswerInstances)
        {
            answer.ShowIfCorrect();
        }
        StartCoroutine(HideCorrectAnswerAndShowConfirmation());
    }

    public void Update()
    {
        if (TransitioningAnswersIn)
        {
            if (AnswersObject.transform.position.x > 0)
            {
                AnswersObject.transform.position -= new Vector3(1, 0, 0);
            }
            else if (AnswersObject.transform.position.x <= 0)
            {
                AnswersObject.transform.position = new Vector3(0, AnswersObject.transform.position.y, AnswersObject.transform.position.z);
                AnswerTimerInstance.Play();
                TransitioningAnswersIn = false;
            }
        }
        if (TransitioningAnswersOut)
        {
            if (AnswersObject.transform.position.x < 100)
            {
                AnswersObject.transform.position += new Vector3(1, 0, 0);
            }
            else if (AnswersObject.transform.position.x >= 100)
            {
                AnswersObject.transform.position = new Vector3(100, AnswersObject.transform.position.y, AnswersObject.transform.position.z);
                TransitioningAnswersOut = false;
            }
        }
    }

    public IEnumerator HideCorrectAnswerAndShowConfirmation()
    {
        yield return new WaitForSeconds(2);
        TransitioningAnswersOut = true;
        yield return new WaitForSeconds(0.5f);
        

        var questionAnswers = DatabaseManager.Instance.GetPlayerAnswersForQuestionId(Data.QuestionId);
        var answerTimes = new Dictionary<string, List<int>>();
        var answerCorrectIncorrect = new Dictionary<string, bool>();
        foreach (var pollAnswerData in Data.PollAnswersData)
        {
            if (pollAnswerData.AnswerId == SelectedAnswer.Data.AnswerId)
            {
                questionAnswers.Add(SelectedAnswer.Data.AnswerId);
            }
            answerTimes.Add(pollAnswerData.AnswerText, questionAnswers.FindAll(qa => qa == pollAnswerData.AnswerId));
            answerCorrectIncorrect.Add(pollAnswerData.AnswerText, pollAnswerData.Correct);
        }

        if (Data.ConfirmationType == "text")
        {
            ConfirmationInstance = Instantiate(BubbleSingleConfirmationPrefab).GetComponent<PollConfirmation>();
            ConfirmationInstance.transform.SetParent(transform);
            ConfirmationInstance.SetData(answerTimes, answerCorrectIncorrect, Data.PollAnswersData);
            ConfirmationInstance.CreateObjects();
            ConfirmationInstance.ShowObjects();
            yield return new WaitForSeconds(3);
            ConfirmationInstance.HideObjects();
        }
        else if (Data.ConfirmationType == "bar")
        {
            ConfirmationInstance = Instantiate(BarConfirmationPrefab).GetComponent<PollConfirmationBar>();
            ConfirmationInstance.transform.SetParent(transform);
            ConfirmationInstance.SetData(answerTimes, answerCorrectIncorrect, Data.PollAnswersData);
            ConfirmationInstance.CreateObjects();
            ConfirmationInstance.ShowObjects();

            ConfirmationInstance.TransitionIn();
            yield return new WaitForSeconds(2);
            ConfirmationInstance.DoAnimation();
            yield return new WaitForSeconds(3);
            ConfirmationInstance.TransitionOut();
            yield return new WaitForSeconds(1);
        }
        else if (Data.ConfirmationType == "pie")
        {
            ConfirmationInstance = Instantiate(PieConfirmationPrefab).GetComponent<PollConfirmationPie>();
            ConfirmationInstance.transform.SetParent(transform);
            ConfirmationInstance.SetData(answerTimes, answerCorrectIncorrect, Data.PollAnswersData);
            ConfirmationInstance.CreateObjects();
            ConfirmationInstance.ShowObjects();

            ConfirmationInstance.TransitionIn();
            yield return new WaitForSeconds(2);
            ConfirmationInstance.DoAnimation();
            yield return new WaitForSeconds(3);
            ConfirmationInstance.TransitionOut();
            yield return new WaitForSeconds(1);
        }
    }

    public void ShowObjects()
    {
        foreach (var pollAnswerInstance in PollAnswerInstances)
        {
            pollAnswerInstance.ShowObjects();
        }
        AnswerBackgroundInstance.ShowObjects();
        AnswerTimerInstance.ShowObjects();
        AnswerTimerInstance.ShowFirstFrame();
        QuestionTextInstance.gameObject.SetActive(true);
        QuestionTextInstance.AnimateFadeIn(2);
        StartCoroutine(WaitThenTransitionAnswersIn());
    }

    public IEnumerator WaitThenTransitionAnswersIn()
    {
        yield return new WaitForSeconds(3.0f);
        TransitioningAnswersIn = true;
    }

    public void HideObjects()
    {
        foreach (var pollAnswerInstance in PollAnswerInstances)
        {
            pollAnswerInstance.HideObjects();
        }
        AnswerBackgroundInstance.HideObjects();
        AnswerTimerInstance.HideObjects();
        QuestionTextInstance.gameObject.SetActive(false);
    }
}
