using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExhibitGameManager : MonoBehaviour
{
    public static ExhibitGameManager Instance { get; private set; }

    public string CurrentGameState;
    private string PreviousGameState;
    public bool Connected = false;
    public bool Started = false;

    public Player Player;

    public PollBackgroundSequences PollBackgroundSequencesPrefab;
    private PollBackgroundSequences PollBackgroundSequencesInstance;
    private bool DisablePollBackgroundSequences = true;

    public PollTextComponent LoadingTextInstance;

    public PollImageComponent ExhibitLogoPrefab;
    private PollImageComponent ExhibitLogoInstance;

    public PollImageComponent ExhibitBackgroundPrefab;
    private PollImageComponent ExhibitBackgroundInstance;
    private PollData PollData;

    public PollImageSequenceComponent LoadPrefab;
    public PollTimerComponent LoadTimerPrefab;
    public PollImageSequenceComponent LoadCorrectPrefab;
    public PollImageSequenceComponent LoadIncorrectPrefab;

    private GameObject ConfirmationObj;
    private int LoadConfirmationSequenceIndex;
    private bool LoadedConfirmationSequences = false;
    private List<PollImageSequenceComponent> LoadConfirmationSequenceInstances;
    private List<PollImageSequenceComponent> LoadSequenceInstances;
    private PollImageSequenceComponent LoadBackgroundSequenceInstance;

    private int PollScore;
    private string DisplayName;
    private string FirstName;
    private string LastName;

    private TimeSpan? PollTotalTime;
    private List<PollUserAnswer> PollUserAnswers;

    public void Awake()
    {
        Instance = this;
        LoadSequenceInstances = new List<PollImageSequenceComponent>();
        LoadConfirmationSequenceInstances = new List<PollImageSequenceComponent>();
        PollData = new PollData(Application.dataPath + "/poll-questions.json");
        StartCoroutine(PollData.GetData());
    }

    IEnumerator Start()
    {
        LoadBackgroundSequenceInstance = Instantiate(LoadPrefab).GetComponent<PollImageSequenceComponent>();
        LoadBackgroundSequenceInstance.transform.SetParent(transform);
        LoadBackgroundSequenceInstance.SetImageSequenceFolder("ExhibitGame/Images/background_image_sequence");
        LoadBackgroundSequenceInstance.CreateObjects(false);

        ExhibitBackgroundInstance = Instantiate(ExhibitBackgroundPrefab).GetComponent<PollImageComponent>();
        ExhibitBackgroundInstance.transform.SetParent(transform);
        ExhibitBackgroundInstance.transform.position += new Vector3(0, 0, 6);
        ExhibitBackgroundInstance.CreateObjects("ExhibitGame/Images/BackgroundImage.png");

        ExhibitLogoInstance = Instantiate(ExhibitLogoPrefab).GetComponent<PollImageComponent>();
        ExhibitLogoInstance.name = "Logo";
        ExhibitLogoInstance.transform.SetParent(transform);
        ExhibitLogoInstance.transform.position += new Vector3(0, 0, 1);
        ExhibitLogoInstance.CreateObjects("ExhibitGame/Images/Logo.png");

        var loadTimerInstance = Instantiate(LoadTimerPrefab).GetComponent<PollTimerComponent>();
        loadTimerInstance.transform.SetParent(transform);
        loadTimerInstance.CreateObjects(false);
        LoadSequenceInstances.Add(loadTimerInstance);

        var loadCorrectInstance = Instantiate(LoadCorrectPrefab).GetComponent<PollImageSequenceComponent>();
        loadCorrectInstance.transform.SetParent(transform);
        loadCorrectInstance.SetImageSequenceFolder("Poll/Images/CorrectAnswer");
        loadCorrectInstance.CreateObjects(false);
        LoadSequenceInstances.Add(loadCorrectInstance);

        var loadIncorrectInstance = Instantiate(LoadIncorrectPrefab).GetComponent<PollImageSequenceComponent>();
        loadIncorrectInstance.transform.SetParent(transform);
        loadIncorrectInstance.SetImageSequenceFolder("Poll/Images/IncorrectAnswer");
        loadIncorrectInstance.CreateObjects(false);
        LoadSequenceInstances.Add(loadIncorrectInstance);

        LoadingTextInstance.AnimateFadeIn();
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(CheckIsConnected());
    }

    private void LoadNextConfirmationSequence()
    {
        var percentLoaded = ((float)(LoadConfirmationSequenceIndex) / PollData.QuestionsData.Count) * 100;
        LoadingTextInstance.SetTextData("LOADING... (" + percentLoaded.ToString("00") + "%)");
        LoadingTextInstance.CreateAllObjects();

        if (LoadConfirmationSequenceIndex > PollData.QuestionsData.Count - 1)
        {
            LoadedConfirmationSequences = true;
        }
        else
        {
            var loadConfirmationInstance = Instantiate(LoadPrefab).GetComponent<PollImageSequenceComponent>();
            loadConfirmationInstance.transform.SetParent(ConfirmationObj.transform);
            loadConfirmationInstance.SetImageSequenceFolder("Poll/Images/Confirmations/" + PollData.QuestionsData[LoadConfirmationSequenceIndex].ConfirmationDirectory);
            loadConfirmationInstance.CreateObjects(false);
            LoadConfirmationSequenceInstances.Add(loadConfirmationInstance);
            StartCoroutine(CheckConfirmationImageSequence());
        }
    }

    IEnumerator CheckConfirmationImageSequence()
    {
        if (LoadConfirmationSequenceInstances[LoadConfirmationSequenceIndex].Loading == false)
        {
            LoadConfirmationSequenceInstances[LoadConfirmationSequenceIndex].HideObjects();
            LoadConfirmationSequenceIndex++;
            LoadNextConfirmationSequence();
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(CheckConfirmationImageSequence());
        }
    }

    IEnumerator CheckIsConnected()
    {
        var players = FindObjectsOfType<Player>();
        foreach(var player in players)
        {
            player.CheckNetworkPlayer();
        }
        Connected = Player != null && PollData.IsDoneParsing;

        if (Connected && !Started)
        {
            ConfirmationObj = new GameObject("Confirmation Sequences");
            ConfirmationObj.transform.SetParent(transform);
            LoadConfirmationSequenceIndex = 0;
            LoadNextConfirmationSequence();

            Player.CmdLoadInitialData();
            Started = true;
            yield return StartCoroutine(CheckIsDoneLoading());
        }
        yield return new WaitForSeconds(3);
        StartCoroutine(CheckIsConnected());
    }

    IEnumerator CheckIsDoneLoading()
    {
        var stillLoading = ExhibitBackgroundInstance.Loading || LoadBackgroundSequenceInstance.Loading || LoadSequenceInstances.Count(lsi => lsi.Loading) > 0 || Player.Loading || ScreensaverManager.Instance.Loading || !LoadedConfirmationSequences;
        if (stillLoading)
        {
            yield return new WaitForSeconds(1);
            yield return StartCoroutine(CheckIsDoneLoading());
        }
        else
        {
            if (!DisablePollBackgroundSequences)
            {
                PollBackgroundSequencesInstance = Instantiate(PollBackgroundSequencesPrefab);
                PollBackgroundSequencesInstance.transform.SetParent(transform);
            }

            ScreensaverManager.Instance.HideScreensaver();
            LoadingTextInstance.gameObject.SetActive(false);
            foreach (var lsi in LoadSequenceInstances)
            {
                lsi.HideObjects();//destroy??
            }
            GoToState("StartingLeaderboard");
        }
    }

    public void Update()
    {
        switch (CurrentGameState)
        {
            case "StartingStart":
                break;
            case "Start":
                break;
            case "StartingPoll":
                GoToState("Poll");
                break;
            case "Poll":
                break;
            case "StartingLeaderboard":
                GoToState("Leaderboard");
                break;
            case "Leaderboard":
                break;
            case "StartingScreensaver":
                GoToState("Screensaver");
                break;
            case "Screensaver":
                break;
            case "StartingAdmin":
                GoToState("Admin");
                break;
            case "Admin":
                break;
            default:
                break;
        }
    }

    public void ResetGame()
    {
        Player.CmdLoadInitialData();
        PollManager.Instance.DestroyPoll();
        LeaderboardManager.Instance.DestroyLeaderboard();    
    }

    public void OnFinishPoll(int score, TimeSpan totalTime, List<PollUserAnswer> userAnswers, string displayName, string firstName, string lastName)
    {
        PollScore = score;
        DisplayName = displayName;
        FirstName = firstName;
        LastName = lastName;
        PollTotalTime = totalTime;
        PollUserAnswers = userAnswers;
    }

    public void OnFinishPollInactive()
    {
        GoToState("StartingLeaderboard");
    }

    public void OnInactive()
    {
        LeaderboardManager.Instance.OnInactive();
        GoToState("StartingScreensaver");
    }

    public void OnActive()
    {
        GoToState("StartingPoll");
    }

    public void OnCancelScreensaver()
    {
        GoToState(PreviousGameState);
    }

    public void OnFinishLeaderboard()
    {
        GoToState("StartingPoll");
    }

    public void OnSaveLeaderboard()
    {
        GoToState("StartingLeaderboard");
    }

    public void OnFinishScreensaver()
    {
        GoToState("StartingLeaderboard");
    }

    public void OnDoNotConsent()
    {
        CurrentGameState = "";
        GoToState("StartingLeaderboard");
    }

    public void OnAdminKeysPressed()
    {
        if (CurrentGameState == "Admin")
        {
            DatabaseManager.Instance.HideAdminScreen();
            GoToState(PreviousGameState);
        }
        else
        {
            GoToState("StartingAdmin");
        }
    }

    private IEnumerator WaitAndShowLeaderboard(string previousGameState)
    {
        yield return new WaitForSeconds(2);
        LeaderboardManager.Instance.ShowLeaderboard(PollScore, PollTotalTime, PollUserAnswers, previousGameState == "Poll", DisplayName, FirstName, LastName);
    }

    public void GoToState(string newState)
    {
        var previousState = CurrentGameState;
        CurrentGameState = newState;
        if (newState == "StartingStart")
        {
            PreviousGameState = previousState;
            GoToState("Start");
        }
        else if (newState == "Start")
        {
            GoToState("StartingPoll");
        }
        else if (newState == "StartingPoll")
        {
            PreviousGameState = previousState;
            PollManager.Instance.RestartPoll();
        }
        else if (newState == "Poll")
        {
        }
        else if (newState == "StartingLeaderboard")
        {
            PreviousGameState = previousState;
            ResetGame();
            StartCoroutine(WaitAndShowLeaderboard(PreviousGameState));
            /*DisplayName = null;
            FirstName = null;
            LastName = null;
            PollScore = 0;
            PollTotalTime = null;*/
        }
        else if (newState == "Leaderboard")
        {
        }
        else if (newState == "StartingScreensaver")
        {
            PreviousGameState = previousState;
            ScreensaverManager.Instance.StartingScreensaver();
        }
        else if (newState == "Screensaver")
        {
            ScreensaverManager.Instance.StartScreensaver();
        }
        else if (newState == "StartingAdmin")
        {
            PreviousGameState = previousState;
        }
        else if (newState == "Admin")
        {
            DatabaseManager.Instance.ShowAdminScreen();
        }
    }
}
