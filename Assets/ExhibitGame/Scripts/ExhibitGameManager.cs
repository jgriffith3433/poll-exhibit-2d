using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExhibitGameManager : MonoBehaviour {
    public static ExhibitGameManager Instance { get; private set; }
    
    public string CurrentGameState;
    private string PreviousGameState;

    public Transform PollCameraPosition;
    public Transform LeaderboardCameraPosition;
    public Transform ScreenSaverCameraPosition;
    public Transform AdminCameraPosition;

    public float MinDistanceToStartState = 10.0f;
    public float CameraTransitionSpeed = 2.0f;
    public float CameraScreensaverTransitionSpeed = 18.0f;

    public void Awake()
    {
        Instance = this;
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(2);
        GoToState("StartingLeaderboard");
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
                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, PollCameraPosition.position, Time.deltaTime * CameraTransitionSpeed);
                Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, PollCameraPosition.rotation, Time.deltaTime * CameraTransitionSpeed);
                if ((Camera.main.transform.position - PollCameraPosition.transform.position).magnitude < MinDistanceToStartState &&
                    Quaternion.Angle(Camera.main.transform.rotation, PollCameraPosition.transform.rotation) < MinDistanceToStartState)
                {
                    GoToState("Poll");
                }
                break;
            case "Poll":
                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, PollCameraPosition.position, Time.deltaTime * CameraTransitionSpeed);
                Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, PollCameraPosition.rotation, Time.deltaTime * CameraTransitionSpeed);
                break;
            case "StartingLeaderboard":
                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, LeaderboardCameraPosition.position, Time.deltaTime * CameraTransitionSpeed);
                Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, LeaderboardCameraPosition.rotation, Time.deltaTime * CameraTransitionSpeed);
                if ((Camera.main.transform.position - LeaderboardCameraPosition.transform.position).magnitude < MinDistanceToStartState &&
                    Quaternion.Angle(Camera.main.transform.rotation, LeaderboardCameraPosition.transform.rotation) < MinDistanceToStartState)
                {
                    GoToState("Leaderboard");
                }
                break;
            case "Leaderboard":
                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, LeaderboardCameraPosition.position, Time.deltaTime * CameraTransitionSpeed);
                Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, LeaderboardCameraPosition.rotation, Time.deltaTime * CameraTransitionSpeed);
                break;
            case "StartingScreensaver":
                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, ScreenSaverCameraPosition.position, Time.deltaTime * CameraScreensaverTransitionSpeed);
                Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, ScreenSaverCameraPosition.rotation, Time.deltaTime * CameraScreensaverTransitionSpeed);
                if ((Camera.main.transform.position - ScreenSaverCameraPosition.transform.position).magnitude < MinDistanceToStartState &&
                    Quaternion.Angle(Camera.main.transform.rotation, ScreenSaverCameraPosition.transform.rotation) < MinDistanceToStartState)
                {
                    GoToState("Screensaver");
                }
                break;
            case "Screensaver":
                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, ScreenSaverCameraPosition.position, Time.deltaTime * CameraScreensaverTransitionSpeed);
                Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, ScreenSaverCameraPosition.rotation, Time.deltaTime * CameraScreensaverTransitionSpeed);
                break;
            case "StartingAdmin":
                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, AdminCameraPosition.position, Time.deltaTime * CameraScreensaverTransitionSpeed);
                Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, AdminCameraPosition.rotation, Time.deltaTime * CameraScreensaverTransitionSpeed);
                if ((Camera.main.transform.position - AdminCameraPosition.transform.position).magnitude < MinDistanceToStartState &&
                    Quaternion.Angle(Camera.main.transform.rotation, AdminCameraPosition.transform.rotation) < MinDistanceToStartState)
                {
                    GoToState("Admin");
                }
                break;
            case "Admin":
                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, AdminCameraPosition.position, Time.deltaTime * CameraScreensaverTransitionSpeed);
                Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, AdminCameraPosition.rotation, Time.deltaTime * CameraScreensaverTransitionSpeed);
                break;
            default:
                break;
        }
    }

    public void ResetGame()
    {
        PollManager.Instance.DestroyPoll();
    }

    public void OnFinishPoll()
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
        GoToState("StartingLeaderboard");
    }

    public void OnCancelScreensaver()
    {
        GoToState(PreviousGameState);
    }

    public void OnFinishLeaderboard()
    {
        GoToState("StartingPoll");
    }

    public void OnFinishScreensaver()
    {
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
            LeaderboardManager.Instance.StartingLeaderboard();
            LeaderboardManager.Instance.RestartLeaderboard();
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
