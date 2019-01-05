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

    public PollConfirmationAnim_03 AnimConfirmationPrefab_03;
    public PollConfirmationAnim_04 AnimConfirmationPrefab_04;
    public PollConfirmationAnim_05 AnimConfirmationPrefab_05;
    public PollConfirmationAnim_06 AnimConfirmationPrefab_06;
    public PollConfirmationAnim_07 AnimConfirmationPrefab_07;
    public PollConfirmationAnim_08 AnimConfirmationPrefab_08;
    public PollConfirmationAnim_09 AnimConfirmationPrefab_09;
    public PollConfirmationAnim_10 AnimConfirmationPrefab_10;
    public PollConfirmationAnim_11 AnimConfirmationPrefab_11;
    public PollConfirmationAnim_12 AnimConfirmationPrefab_12;
    public PollConfirmationAnim_13 AnimConfirmationPrefab_13;
    public PollConfirmationAnim_14 AnimConfirmationPrefab_14;
    public PollConfirmationAnim_15 AnimConfirmationPrefab_15;
    public PollConfirmationAnim_16 AnimConfirmationPrefab_16;
    public PollConfirmationAnim_17 AnimConfirmationPrefab_17;
    public PollConfirmationAnim_18 AnimConfirmationPrefab_18;
    public PollConfirmationAnim_19 AnimConfirmationPrefab_19;
    public PollConfirmationAnim_20 AnimConfirmationPrefab_20;
    public PollConfirmationAnim_21 AnimConfirmationPrefab_21;
    public PollConfirmationAnim_22 AnimConfirmationPrefab_22;
    public PollConfirmationAnim_23 AnimConfirmationPrefab_23;
    public PollConfirmationAnim_24 AnimConfirmationPrefab_24;
    public PollConfirmationAnim_25 AnimConfirmationPrefab_25;
    public PollConfirmationAnim_26 AnimConfirmationPrefab_26;
    public PollConfirmationAnim_27 AnimConfirmationPrefab_27;
    public PollConfirmationAnim_28 AnimConfirmationPrefab_28;
    public PollConfirmationAnim_29 AnimConfirmationPrefab_29;
    public PollConfirmationAnim_30 AnimConfirmationPrefab_30;
    public PollConfirmationAnim_31 AnimConfirmationPrefab_31;
    public PollConfirmationAnim_32 AnimConfirmationPrefab_32;

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
        else if (Data.ConfirmationType == "anim")
        {
            switch(Data.ConfirmationDirectory)
            {
                case "03":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_03);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_03>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                case "04":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_04);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_04>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                case "05":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_05);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_05>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                case "06":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_06);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_06>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                case "07":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_07);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_07>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                case "08":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_08);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_08>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                case "09":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_09);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_09>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                case "10":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_10);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_10>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                case "11":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_11);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_11>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                case "12":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_12);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_12>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                case "13":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_13);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_13>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                case "14":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_14);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_14>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                case "15":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_15);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_15>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                case "16":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_16);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_16>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                case "17":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_17);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_17>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                case "18":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_18);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_18>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                case "19":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_19);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_19>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                case "20":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_20);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_20>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                case "21":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_21);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_21>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                case "22":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_22);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_22>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                case "23":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_23);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_23>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                case "24":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_24);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_24>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                case "25":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_25);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_25>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                case "26":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_26);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_26>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                case "27":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_27);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_27>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                case "28":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_28);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_28>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                case "29":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_29);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_29>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                case "30":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_30);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_30>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                case "31":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_31);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_31>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                case "32":
                    ConfirmationInstance = Instantiate(AnimConfirmationPrefab_32);
                    ConfirmationInstance.GetComponent<PollConfirmationAnim_32>().AnimDirectoryName = Data.ConfirmationDirectory;
                    break;
                default:
                    break;
            }
            if (ConfirmationInstance != null)
            {
                ConfirmationInstance.transform.SetParent(transform);
                ConfirmationInstance.SetData(answerTimes, answerCorrectIncorrect, Data.PollAnswersData);
                ConfirmationInstance.CreateObjects();
                ConfirmationInstance.ShowObjects();

                ConfirmationInstance.TransitionIn();
                yield return new WaitForSeconds(2);
                ConfirmationInstance.DoAnimation();
                yield return new WaitForSeconds(5);
                ConfirmationInstance.TransitionOut();
                yield return new WaitForSeconds(1);
            }
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
