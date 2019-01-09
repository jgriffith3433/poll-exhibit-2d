using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreensaverComponent : MonoBehaviour {

    public bool Hidden;
    public bool CountdownHidden;
    private ScreensaverData Data;
    public ScreensaverCountdownComponent CountdownComponent;
    public PollImageComponent ScreensaverImagePrefab;
    private List<PollImageComponent> ScreensaverImages;
    private int CurrentScreensaverImage = 0;
    public PollButtonComponent BtnStartOver;
    private bool ShowScreensaverImages = false;

    public GameObject ScreensaverTextInstance;

    public PollImageSequenceComponent LeftScreensaverBackgroundSequencePrefab;
    private PollImageSequenceComponent LeftScreensaverBackgroundSequenceInstance;

    public PollImageSequenceComponent RightScreensaverBackgroundSequencePrefab;
    private PollImageSequenceComponent RightScreensaverBackgroundSequenceInstance;

    public bool Loading {
        get {
            if (ShowScreensaverImages)
            {
                if (Data == null)
                {
                    return true;
                }
                return Data.Loading;
            }
            else
            {
                if (LeftScreensaverBackgroundSequenceInstance == null || RightScreensaverBackgroundSequenceInstance == null)
                {
                    return true;
                }
                return LeftScreensaverBackgroundSequenceInstance.Loading || RightScreensaverBackgroundSequenceInstance.Loading;
            }
        }
    }

    public IEnumerator CreateObjects()
    {
        //BtnStartOver = GetComponentInChildren<PollButtonComponent>();
        if (ShowScreensaverImages)
        {
            Data = new ScreensaverData(Application.dataPath + "/Screensaver/Images/screensaver_images");
            yield return StartCoroutine(Data.GetData());
            ScreensaverImages = new List<PollImageComponent>();
            for (var i = 0; i < Data.ScreensaverImages.Count; i++)
            {
                var screensaverImageComponent = Instantiate(ScreensaverImagePrefab).GetComponent<PollImageComponent>();
                screensaverImageComponent.transform.SetParent(transform);
                screensaverImageComponent.CreateObjects(Data.ScreensaverImages[i]);
                screensaverImageComponent.HideObjects();
                ScreensaverImages.Add(screensaverImageComponent);
            }
        }
        else
        {
            LeftScreensaverBackgroundSequenceInstance = Instantiate(LeftScreensaverBackgroundSequencePrefab).GetComponent<PollImageSequenceComponent>();
            LeftScreensaverBackgroundSequenceInstance.transform.SetParent(transform);
            LeftScreensaverBackgroundSequenceInstance.transform.position += new Vector3(0, 0, 2);
            LeftScreensaverBackgroundSequenceInstance.SetImageSequenceFolder("Screensaver/Images/LeftScreenBars");
            LeftScreensaverBackgroundSequenceInstance.SetLoop(true);
            LeftScreensaverBackgroundSequenceInstance.CreateObjects(false);

            RightScreensaverBackgroundSequenceInstance = Instantiate(RightScreensaverBackgroundSequencePrefab).GetComponent<PollImageSequenceComponent>();
            RightScreensaverBackgroundSequenceInstance.transform.SetParent(transform);
            RightScreensaverBackgroundSequenceInstance.transform.position += new Vector3(0, 0, 2);
            RightScreensaverBackgroundSequenceInstance.SetImageSequenceFolder("Screensaver/Images/RightScreenBars");
            RightScreensaverBackgroundSequenceInstance.SetLoop(true);
            RightScreensaverBackgroundSequenceInstance.CreateObjects(false);
        }
        CountdownComponent.CreateObjects();
    }

    public void ShowCountdown()
    {
        Hidden = false;
        gameObject.SetActive(true);
        CountdownHidden = false;
        CountdownComponent.StartCountdown();
    }

    public void HideCountdown()
    {
        CountdownHidden = true;
        CountdownComponent.CancelCountdown();
    }

    public void ShowObjects()
    {
        Hidden = false;
        CurrentScreensaverImage = -1;
        gameObject.SetActive(true);
        if (ShowScreensaverImages)
        {
            StartCoroutine(ShowNextScreensaverImage());
        }
        else
        {
            ScreensaverTextInstance.SetActive(true);
            LeftScreensaverBackgroundSequenceInstance.ShowObjects();
            LeftScreensaverBackgroundSequenceInstance.Play();
            RightScreensaverBackgroundSequenceInstance.ShowObjects();
            RightScreensaverBackgroundSequenceInstance.Play();
            StartCoroutine(WaitThenShowLeaderboard());
        }
    }

    public IEnumerator WaitThenShowLeaderboard()
    {
        yield return new WaitForSeconds(40);
        if (Hidden)
        {
            yield return null;
        }
        FinishScreensaver();
        ExhibitGameManager.Instance.OnFinishScreensaver();
    }

    public IEnumerator ShowNextScreensaverImage()
    {
        if (Hidden)
        {
            yield return null;
        }
        if (CurrentScreensaverImage > -1 && CurrentScreensaverImage <= ScreensaverImages.Count -1)
        {
            ScreensaverImages[CurrentScreensaverImage].HideObjects();
        }
        CurrentScreensaverImage++;
        if (CurrentScreensaverImage < 0)
        {
            CurrentScreensaverImage = 0;
        }

        if (CurrentScreensaverImage > ScreensaverImages.Count - 1)
        {
            LeaderboardManager.Instance.ShowLeaderboardScreensaver();
            //BtnStartOver.gameObject.SetActive(false);
            CurrentScreensaverImage = -1;
        }
        else
        {
            ScreensaverImages[CurrentScreensaverImage].ShowObjects();
            LeaderboardManager.Instance.HideLeaderboardScreensaver();
            //BtnStartOver.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(10);
        yield return ShowNextScreensaverImage();
    }

    public void HideObjects()
    {
        Hidden = true;
        CountdownHidden = true;
        gameObject.SetActive(false);
        LeftScreensaverBackgroundSequenceInstance.HideObjects();
        RightScreensaverBackgroundSequenceInstance.HideObjects();
        ScreensaverTextInstance.SetActive(false);
    }

    public void FinishScreensaver()
    {
        HideObjects();
    }

    public void Update()
    {
        if (!Hidden && !Loading)
        {
            if (Input.anyKeyDown || Input.touchCount > 0)
            {
                FinishScreensaver();
            }
        }
    }
}
