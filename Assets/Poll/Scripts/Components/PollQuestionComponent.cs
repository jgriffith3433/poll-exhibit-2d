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

    public PieChartComponent AnswerPieChartPrefab;
    private PieChartComponent AnswerPieChartInstance;

    public PollImageComponent AnswerBackgroundPrefab;
    private PollImageComponent AnswerBackgroundInstance;
    private PollTimerComponent AnswerTimerInstance;

    private bool TransitioningAnswersIn = false;
    private bool TransitioningAnswersOut = false;
    private bool TransitioningBarGraphIn = false;
    private bool TransitioningBarGraphOut = false;
    private bool TransitioningPieChartIn = false;
    private bool TransitioningPieChartOut = false;
    private Vector3 OriginalAnswerBarGraphInstancePosition;
    private Vector3 OriginalAnswerPieChartInstancePosition;

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
        AnswerTimerInstance = AnswerBackgroundInstance.GetComponentInChildren<PollTimerComponent>();
        AnswerTimerInstance.CreateObjects(false);
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
        if (TransitioningPieChartIn)
        {
            if (AnswerPieChartInstance.transform.position.y < OriginalAnswerPieChartInstancePosition.y)
            {
                AnswerPieChartInstance.transform.position += new Vector3(0, 1, 0);
            }
            else if (AnswerPieChartInstance.transform.position.y >= OriginalAnswerPieChartInstancePosition.y)
            {
                AnswerPieChartInstance.transform.position = new Vector3(AnswerPieChartInstance.transform.position.x, OriginalAnswerPieChartInstancePosition.y, AnswerPieChartInstance.transform.position.z);
                TransitioningPieChartIn = false;
            }
        }
        if (TransitioningPieChartOut)
        {
            Destroy(AnswerPieChartInstance.gameObject);
            TransitioningPieChartOut = false;
            /*if (AnswerPieChartInstance.transform.position.y > -100)
            {
                AnswerPieChartInstance.transform.position -= new Vector3(0, 1, 0);
            }
            else if (AnswerPieChartInstance.transform.position.y <= -100)
            {
                AnswerPieChartInstance.transform.position = new Vector3(AnswerPieChartInstance.transform.position.x, -100, AnswerPieChartInstance.transform.position.z);
                TransitioningPieChartOut = false;
                Destroy(AnswerPieChartInstance.gameObject);
            }*/
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
            answerTimes.Add(pollAnswerData.AnswerText, questionAnswers.FindAll(qa => qa == pollAnswerData.AnswerId));
            answerCorrectIncorrect.Add(pollAnswerData.AnswerText, pollAnswerData.Correct);
        }

        if (Data.ConfirmationType == "text")
        {
            TF_ConfirmationTextInstance.ShowObjects();
            yield return new WaitForSeconds(3);
            TF_ConfirmationTextInstance.HideObjects();
        }
        else if (Data.ConfirmationType == "bar")
        {
            AnswerBarGraphInstance = Instantiate(AnswerBarGraphPrefab, transform).GetComponent<BarGraphComponent>();
            OriginalAnswerBarGraphInstancePosition = AnswerBarGraphInstance.transform.position;
            AnswerBarGraphInstance.MaxBarValue = answerTimes.OrderByDescending(at => at.Value.Count).FirstOrDefault().Value.Count;

            foreach (var answer in answerTimes)
            {
                AnswerBarGraphInstance.SetValue(answer.Key, answer.Value.Count, answerCorrectIncorrect[answer.Key] ? BarComponent.BarColor.Red : BarComponent.BarColor.Grey);
            }
            AnswerBarGraphInstance.transform.position += new Vector3(0, -100, 0);

            TransitioningBarGraphIn = true;
            yield return new WaitForSeconds(2);
            AnswerBarGraphInstance.DoAnimation();
            yield return new WaitForSeconds(3);
            TransitioningBarGraphOut = true;
            yield return new WaitForSeconds(1);
        }
        else if (Data.ConfirmationType == "pie")
        {
            AnswerPieChartInstance = Instantiate(AnswerPieChartPrefab, transform).GetComponent<PieChartComponent>();
            OriginalAnswerPieChartInstancePosition = AnswerPieChartInstance.transform.position;
            foreach (var answer in answerTimes)
            {
                AnswerPieChartInstance.SetValue(answer.Key, answer.Value.Count, answerCorrectIncorrect[answer.Key] ? PieChartComponent.PieChartColor.Red : PieChartComponent.PieChartColor.Grey);
            }
            AnswerPieChartInstance.transform.position += new Vector3(0, -100, 0);
            AnswerPieChartInstance.gameObject.SetActive(false);

            TransitioningPieChartIn = true;
            yield return new WaitForSeconds(2);
            AnswerPieChartInstance.gameObject.SetActive(true);
            AnswerPieChartInstance.DoAnimation();
            yield return new WaitForSeconds(3);
            TransitioningPieChartOut = true;
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
