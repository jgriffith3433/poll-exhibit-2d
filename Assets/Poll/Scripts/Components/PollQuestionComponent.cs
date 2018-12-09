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

    public PollTextComponent TF_ConfirmationTextPrefab;
    private PollTextComponent TF_ConfirmationTextInstance;

    public BarGraphComponent AnswerBarGraphPrefab;
    private BarGraphComponent AnswerBarGraphInstance;

    public PollImageComponent AnswerBackgroundPrefab;
    private PollImageComponent AnswerBackgroundInstance;

    private bool TransitioningAnswersIn = false;
    private bool TransitioningAnswersOut = false;
    private bool TransitioningBarGraphIn = false;
    private bool TransitioningBarGraphOut = false;
    private Vector3 OriginalAnswerBarGraphInstancePosition;

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
        QuestionTextInstance.AnimateFromTop();

        TF_ConfirmationTextInstance = Instantiate(TF_ConfirmationTextPrefab).GetComponent<PollTextComponent>();
        TF_ConfirmationTextInstance.name = "TF_ConfirmationText";
        TF_ConfirmationTextInstance.transform.SetParent(transform);

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
            if (Data.PollAnswersData[i].Correct)
            {
                TF_ConfirmationTextInstance.SetTextData(Data.PollAnswersData[i].AnswerText);
                TF_ConfirmationTextInstance.CreateAllObjects();
            }
        }
        TF_ConfirmationTextInstance.HideObjects();
        AnswerBackgroundInstance = Instantiate(AnswerBackgroundPrefab, AnswersObject.transform).GetComponent<PollImageComponent>();
        AnswersObject.transform.position += new Vector3(100, AnswersObject.transform.position.y, AnswersObject.transform.position.z);

        HideObjects();
    }

    public void OnSelectedAnswer(PollAnswerComponent selectedAnswer, int answerId, bool correct)
    {
        SelectedAnswer = selectedAnswer;
        if (correct)
        {
            PollManager.Instance.OnCorrect(Data.QuestionId, answerId);
        }
        else
        {
            PollManager.Instance.OnIncorrect(Data.QuestionId, answerId);

        }
    }

    public void ShowAsCorrectOrIncorrect()
    {
        SelectedAnswer.ShowAsCorrectOrIncorrect();
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
        if (TransitioningBarGraphIn)
        {
            if (AnswerBarGraphInstance.transform.position.y < OriginalAnswerBarGraphInstancePosition.y)
            {
                AnswerBarGraphInstance.transform.position += new Vector3(0, 1, 0);
            }
            else if (AnswerBarGraphInstance.transform.position.y >= OriginalAnswerBarGraphInstancePosition.y)
            {
                AnswerBarGraphInstance.transform.position = new Vector3(AnswerBarGraphInstance.transform.position.x, OriginalAnswerBarGraphInstancePosition.y, AnswerBarGraphInstance.transform.position.z);
                TransitioningBarGraphIn = false;
            }
        }
        if (TransitioningBarGraphOut)
        {
            if (AnswerBarGraphInstance.transform.position.y > -100)
            {
                AnswerBarGraphInstance.transform.position -= new Vector3(0, 1, 0);
            }
            else if (AnswerBarGraphInstance.transform.position.y <= -100)
            {
                AnswerBarGraphInstance.transform.position = new Vector3(AnswerBarGraphInstance.transform.position.x, -100, AnswerBarGraphInstance.transform.position.z);
                TransitioningBarGraphOut = false;
                Destroy(AnswerBarGraphInstance.gameObject);
            }
        }
    }

    public IEnumerator HideCorrectAnswerAndShowConfirmation()
    {
        yield return new WaitForSeconds(1);
        TransitioningAnswersOut = true;
        yield return new WaitForSeconds(0.2f);
        AnswerBarGraphInstance = Instantiate(AnswerBarGraphPrefab, transform).GetComponent<BarGraphComponent>();
        OriginalAnswerBarGraphInstancePosition = AnswerBarGraphInstance.transform.position;

        var questionAnswers = DatabaseManager.Instance.GetPlayerAnswersForQuestionId(Data.QuestionId);
        var answerTimes = new Dictionary<string, List<int>>();
        var answerCorrectIncorrect = new Dictionary<string, bool>();
        foreach (var pollAnswerData in Data.PollAnswersData)
        {
            answerTimes.Add(pollAnswerData.AnswerText, questionAnswers.FindAll(qa => qa == pollAnswerData.AnswerId));
            answerCorrectIncorrect.Add(pollAnswerData.AnswerText, pollAnswerData.Correct);
        }
        var mostAnswers = answerTimes.OrderByDescending(at => at.Value.Count).FirstOrDefault().Value.Count;
        AnswerBarGraphInstance.MaxBarValue = mostAnswers;

        foreach (var answer in answerTimes)
        {
            AnswerBarGraphInstance.SetValue(answer.Key, answer.Value.Count, answerCorrectIncorrect[answer.Key] ? BarComponent.BarColor.Red : BarComponent.BarColor.Grey);
        }
        AnswerBarGraphInstance.transform.position += new Vector3(0, -100, 0);
        if (Data.QuestionType == "TF")
        {
            TF_ConfirmationTextInstance.ShowObjects();
            yield return new WaitForSeconds(3);
            TF_ConfirmationTextInstance.HideObjects();
        }
        else
        {
            TransitioningBarGraphIn = true;
            yield return new WaitForSeconds(3);
            TransitioningBarGraphOut = true;
            yield return new WaitForSeconds(1);
        }
    }

    public void ShowObjects()
    {
        TransitioningAnswersIn = true;
        foreach (var pollAnswerInstance in PollAnswerInstances)
        {
            pollAnswerInstance.ShowObjects();
        }
        AnswerBackgroundInstance.ShowObjects();
        QuestionTextInstance.gameObject.SetActive(true);
    }

    public void HideObjects()
    {
        foreach (var pollAnswerInstance in PollAnswerInstances)
        {
            pollAnswerInstance.HideObjects();
        }
        AnswerBackgroundInstance.HideObjects();
        QuestionTextInstance.gameObject.SetActive(false);
    }
}
