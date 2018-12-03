using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using System.Linq;

public class PollComponent : MonoBehaviour
{
    private List<int> AskedQuestions;
    private List<PollUserAnswer> UserAnswers;

    public PollQuestionComponent QuestionPrefab;
    private List<PollQuestionComponent> QuestionInstances;
    private PollQuestionComponent CurrentQuestion;

    public PollTextComponent TitleTextPrefab;
    private PollTextComponent TitleTextInstance;

    public LoginComponent LoginPrefab;
    private LoginComponent LoginInstance;

    private PollButtonComponent BtnStartOver;

    private PollData Data;

    private bool Loading = true;
    private bool Hidden = false;
    private bool LoginHidden = false;

    private LeaderboardData m_LeaderboardData;

    public void RestartPoll()
    {
        AskedQuestions = new List<int>();
        UserAnswers = new List<PollUserAnswer>();
        Loading = true;
        Data = new PollData(Application.dataPath + "/poll-questions-test.json");
        m_LeaderboardData = new LeaderboardData(Application.dataPath + "/poll-leaderboard-test.json");

        StartCoroutine(Data.GetData());
        StartCoroutine(m_LeaderboardData.GetData());
        StartCoroutine(CheckIsDoneParsing());
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

    public void OnCorrect()
    {
        UserAnswers.Add(new PollUserAnswer
        {
            Correct = true
        });
        StartCoroutine(ShowCorrectAnswerGoToNextQuestion());
    }

    public IEnumerator ShowCorrectAnswerGoToNextQuestion()
    {
        CurrentQuestion.ShowCorrectAnswer();
        yield return new WaitForSeconds(2);
        if (AskedQuestions.Count == Data.NumberOfQuestionsAsked || AskedQuestions.Count == QuestionInstances.Count)
        {
            HideObjects();
            ShowLogin();
        }
        else
        {
            GoToNextQuestion();
        }
    }

    public void OnIncorrect()
    {
        UserAnswers.Add(new PollUserAnswer
        {
            Correct = false
        });
        StartCoroutine(ShowCorrectAnswerThenLogin());
    }

    public IEnumerator ShowCorrectAnswerThenLogin()
    {
        CurrentQuestion.ShowCorrectAnswer();
        yield return new WaitForSeconds(2);
        HideObjects();
        ShowLogin();
    }

    public void OnLogin(string displayName, string fullName)
    {
        SaveLeaderboard(displayName, fullName);
        HideLogin();
    }

    public void StartOver()
    {
        ExhibitGameManager.Instance.ResetGame();
        PollManager.Instance.RestartPoll();
    }

    public void SaveLeaderboard(string displayName, string fullName)
    {
        var correctAnswers = UserAnswers.Where(ua => ua.Correct == true).ToList();
        //var percentScore = (int)(((float)correctAnswers.Count / (float)Data.NumberOfQuestionsAsked) * 100);
        //m_LeaderboardData.AddPlayerScore(displayName, percentScore);
        m_LeaderboardData.AddPlayerScore(displayName, correctAnswers.Count);
        m_LeaderboardData.SaveLeaderboard();
        DatabaseManager.Instance.SavePlayerScore(fullName, correctAnswers.Count);
        PollManager.Instance.OnSaveLeaderboard();
    }

    private void CreateObjects()
    {
        Hidden = false;

        BtnStartOver = GetComponentInChildren<PollButtonComponent>();

        LoginInstance = Instantiate(LoginPrefab).GetComponent<LoginComponent>();
        LoginInstance.transform.SetParent(transform);
        LoginInstance.CreateAllObjects();
        HideLogin();

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
    }

    public void HideObjects()
    {
        Hidden = true;
        CurrentQuestion.HideObjects();
        TitleTextInstance.gameObject.SetActive(false);
        BtnStartOver.gameObject.SetActive(false);
    }

    public void ShowLogin()
    {
        LoginInstance.gameObject.SetActive(true);
        LoginHidden = false;
    }

    public void HideLogin()
    {
        LoginInstance.gameObject.SetActive(false);
        LoginHidden = true;
    }

    public void Update()
    {
        if (!Hidden || !LoginHidden && !Loading)
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