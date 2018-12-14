using UnityEngine;
using System.Collections;

public class ScreensaverManager : MonoBehaviour {
    public static ScreensaverManager Instance { get; private set; }

    public ScreensaverComponent ScreensaverPrefab;
    private ScreensaverComponent ScreensaverInstance;

    public bool DiableScreensaver = false;
    private int InactivityUntilScreensaver = 20;
    private float LastActivityTime = 0;
    private bool CanCancel;

    void Awake()
    {
        Instance = this;
    }

    IEnumerator Start()
    {
        ScreensaverInstance = Instantiate(ScreensaverPrefab).GetComponent<ScreensaverComponent>();
        yield return StartCoroutine(ScreensaverInstance.CreateObjects());
    }

    public void StartingScreensaver()
    {

    }

    public void StartScreensaver()
    {
        if (CanCancel)
        {
            ScreensaverInstance.ShowCountdown();
        }
        else
        {
            ScreensaverInstance.ShowObjects();
        }
    }

    public void FinishScreensaver()
    {
        ScreensaverInstance.HideObjects();
    }

    public void FinishCountdown()
    {
        CanCancel = false;
        ExhibitGameManager.Instance.ResetGame();
        StartScreensaver();
    }

    public void Update()
    {
        if (DiableScreensaver)
        {
            LastActivityTime = Time.time;
            return;
        }
        if (ScreensaverInstance.Hidden)
        {
            if (Input.anyKeyDown || Input.touchCount > 0)
            {
                LastActivityTime = Time.time;
            }
            if (Time.time - LastActivityTime >= InactivityUntilScreensaver)
            {
                LastActivityTime = Time.time;
                if (ExhibitGameManager.Instance.CurrentGameState != "Leaderboard")
                {
                    CanCancel = true;
                }
                ExhibitGameManager.Instance.OnInactive();
            }
        }
        else
        {
            LastActivityTime = Time.time;
            if (Input.anyKeyDown || Input.touchCount > 0)
            {
                if (CanCancel)
                {
                    ScreensaverInstance.HideCountdown();
                    ExhibitGameManager.Instance.OnCancelScreensaver();
                }
                else
                {
                    ExhibitGameManager.Instance.OnActive();
                }
            }
        }
    }
}
