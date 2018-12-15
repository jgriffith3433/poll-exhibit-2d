using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using System.Linq;
using System;

public class PollComponent : MonoBehaviour
{
    private List<int> AskedQuestions;
    private List<PollUserAnswer> UserAnswers;

    public PollQuestionComponent QuestionPrefab;
    private List<PollQuestionComponent> QuestionInstances;
    private PollQuestionComponent CurrentQuestion;

    public PollTextComponent TitleTextPrefab;
    private PollTextComponent TitleTextInstance;
    
    public PollButtonComponent BtnStartOver;

    public PollTextComponent TopScoreTitleInstance;
    public PollTextComponent TopScoreTextInstance;
    public PollTextComponent TopScoreTextLabelInstance;
    public PollTextComponent YourScoreTextInstance;
    public PollTextComponent YourScoreTextLabelInstance;

    public TestKnowledgeComponent TestKnowledgePrefab;
    private TestKnowledgeComponent TestKnowledgeInstance;

    private PollData Data;

    private int TopScore;
    private int YourScore;

    private bool Loading = true;
    private bool Hidden = false;

    private TimeSpan StartedTime;

    private LeaderboardData m_LeaderboardData;

    public void RestartPoll()
    {
        AskedQuestions = new List<int>();
        UserAnswers = new List<PollUserAnswer>();
        Loading = true;
        Data = new PollData(Application.dataPath + "/poll-questions.json");
        m_LeaderboardData = new LeaderboardData();
        TopScoreTitleInstance.HideObjects();
        TopScoreTextInstance.HideObjects();
        TopScoreTextLabelInstance.HideObjects();
        YourScoreTextInstance.HideObjects();
        YourScoreTextLabelInstance.HideObjects();
        BtnStartOver.gameObject.SetActive(false);

        ShowInstructions();
    }

    public void BeginPoll()
    {
        HideInstructions();
        StartCoroutine(Data.GetData());
        m_LeaderboardData.SetData(ExhibitGameManager.Instance.Player.GetLeaderboardString());
        StartCoroutine(CheckIsDoneParsing());
    }

    public void ShowInstructions()
    {
        TestKnowledgeInstance = Instantiate(TestKnowledgePrefab);
    }

    public void HideInstructions()
    {
        Destroy(TestKnowledgeInstance.gameObject);
    }

    public void SetTopScore(int score)
    {
        TopScoreTextInstance.SetTextData(score.ToString());
        TopScoreTextInstance.CreateAllObjects();
    }

    public void SetYourScore(int score)
    {
        YourScoreTextInstance.SetTextData(score.ToString());
        YourScoreTextInstance.CreateAllObjects();
    }

    public void GoToNextQuestion()
    {
        if (CurrentQuestion != null)
        {
            CurrentQuestion.HideObjects();
        }
        CurrentQuestion = GetRandomQuestion();
        CurrentQuestion.ShowObjects();
    }

    private PollQuestionComponent GetRandomQuestion()
    {
        var rand = new System.Random();
        var nextQuestionIndex = -1;
        var tries = 0;
        while (nextQuestionIndex == -1 && tries < 100)
        {
            tries++;
            var randomQuestionIndex = rand.Next(0, QuestionInstances.Count);
            if (AskedQuestions.Contains(randomQuestionIndex))
            {
                continue;
            }
            nextQuestionIndex = randomQuestionIndex;
        }
        nextQuestionIndex = nextQuestionIndex == -1 ? rand.Next(0, QuestionInstances.Count) : nextQuestionIndex;
        AskedQuestions.Add(nextQuestionIndex);
        return QuestionInstances[nextQuestionIndex];
    }

    IEnumerator CheckIsDoneParsing()
    {
        if (Data != null && m_LeaderboardData != null && Data.IsDoneParsing && m_LeaderboardData.IsDoneParsing)
        {
            StartedTime = TimeSpan.FromSeconds(Time.time);
            CreateObjects();
            GoToNextQuestion();
            Loading = false;
        }
        else
        {
            yield return new WaitForSeconds(1);
            yield return CheckIsDoneParsing();
        }
    }

    public void OnCorrect(int questionId, int answerId)
    {
        UserAnswers.Add(new PollUserAnswer
        {
            Correct = true,
            AnswerId = answerId,
            QuestionId = questionId
        });
        StartCoroutine(ShowCorrectAnswerGoToNextQuestion());
    }

    public IEnumerator ShowCorrectAnswerGoToNextQuestion()
    {
        CurrentQuestion.ShowAsCorrectOrIncorrect();
        yield return new WaitForSeconds(8);
        if (AskedQuestions.Count == Data.NumberOfQuestionsAsked || AskedQuestions.Count == QuestionInstances.Count)
        {
            HideObjects();
            var elapsedTime = (TimeSpan.FromSeconds(Time.time) - StartedTime);
            var yourScore = UserAnswers.Where(ua => ua.Correct == true).Count();
            PollManager.Instance.FinishPoll(yourScore, elapsedTime, UserAnswers);
        }
        else
        {
            GoToNextQuestion();
            var topScore = m_LeaderboardData.PlayerData.OrderByDescending(pd => pd.PlayerScore).FirstOrDefault();
            var yourScore = UserAnswers.Where(ua => ua.Correct == true).Count();
            SetYourScore(yourScore);
            if (topScore != null)
            {
                if (yourScore > topScore.PlayerScore)
                {
                    SetTopScore(yourScore);
                }
            }
            else
            {
                SetTopScore(yourScore);
            }
        }
    }

    public void OnIncorrect(int questionId, int answerId)
    {
        UserAnswers.Add(new PollUserAnswer
        {
            Correct = false,
            AnswerId = answerId,
            QuestionId = questionId
        });
        StartCoroutine(ShowCorrectAnswerThenFinishPoll());
    }

    public void OnTimerEnded()
    {
        CurrentQuestion.OnTimerEnded();
    }

    public IEnumerator ShowCorrectAnswerThenFinishPoll()
    {
        CurrentQuestion.ShowAsCorrectOrIncorrect();
        yield return new WaitForSeconds(8);
        HideObjects();
        var elapsedTime = (TimeSpan.FromSeconds(Time.time) - StartedTime);
        var yourScore = UserAnswers.Where(ua => ua.Correct == true).Count();
        PollManager.Instance.FinishPoll(yourScore, elapsedTime, UserAnswers);
    }

    public void StartOver()
    {
        ExhibitGameManager.Instance.ResetGame();
        PollManager.Instance.RestartPoll();
    }

    private void CreateObjects()
    {
        Hidden = false;
        Destroy(TestKnowledgeInstance);

        TitleTextInstance = Instantiate(TitleTextPrefab).GetComponent<PollTextComponent>();
        TitleTextInstance.name = "Poll Title";
        TitleTextInstance.transform.SetParent(transform);
        TitleTextInstance.transform.position = Data.PollTitlePosition;
        TitleTextInstance.SetTextData(Data.PollTitle);
        TitleTextInstance.CreateAllObjects();

        var questionsObject = new GameObject("Questions");
        questionsObject.transform.SetParent(transform);
        QuestionInstances = new List<PollQuestionComponent>();
        foreach (var pollQuestionData in Data.QuestionsData)
        {
            var pollQuestionInstance = Instantiate(QuestionPrefab).GetComponent<PollQuestionComponent>();
            pollQuestionInstance.name = "Question";
            pollQuestionInstance.transform.SetParent(questionsObject.transform);
            pollQuestionInstance.SetPollQuestionData(pollQuestionData);
            pollQuestionInstance.CreateObjects();
            QuestionInstances.Add(pollQuestionInstance);
        }

        TopScoreTitleInstance.ShowObjects();
        TopScoreTextInstance.ShowObjects();
        TopScoreTextLabelInstance.ShowObjects();
        YourScoreTextInstance.ShowObjects();
        YourScoreTextLabelInstance.ShowObjects();
        BtnStartOver.gameObject.SetActive(false);
        SetYourScore(0);
        var topScore = m_LeaderboardData.PlayerData.OrderByDescending(pd => pd.PlayerScore).FirstOrDefault();
        if (topScore != null)
        {
            SetTopScore(topScore.PlayerScore);
        }
    }

    public void HideObjects()
    {
        Hidden = true;
        TopScoreTitleInstance.HideObjects();
        TopScoreTextInstance.HideObjects();
        TopScoreTextLabelInstance.HideObjects();
        YourScoreTextInstance.HideObjects();
        YourScoreTextLabelInstance.HideObjects();
        CurrentQuestion.HideObjects();
        TitleTextInstance.gameObject.SetActive(false);
        BtnStartOver.gameObject.SetActive(false);
    }

    public void Update()
    {
        if (!Hidden && !Loading)
        {
            Vector2 touchClickPosition = Vector2.zero;
            if (Input.GetMouseButtonDown(0))
            {
                touchClickPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }
            for (var i = 0; i < Input.touchCount; ++i)
            {
                if (Input.GetTouch(i).phase == TouchPhase.Began)
                {
                    touchClickPosition = Input.GetTouch(i).position;
                }
            }
            if (touchClickPosition != Vector2.zero)
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(touchClickPosition), out hitInfo))
                {
                    if (hitInfo.collider != null)
                    {
                        if (hitInfo.transform)
                        {
                            var buttonComponent = hitInfo.transform.GetComponent<PollButtonComponent>();
                            if (buttonComponent)
                            {
                                buttonComponent.OnClick();
                            }
                        }
                    }
                }
            }
        }
    }
}