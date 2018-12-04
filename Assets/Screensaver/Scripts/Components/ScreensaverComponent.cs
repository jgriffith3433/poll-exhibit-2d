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

    public IEnumerator CreateObjects()
    {
        BtnStartOver = GetComponentInChildren<PollButtonComponent>();
        Data = new ScreensaverData(Application.dataPath + "/screensaver_images");
        yield return StartCoroutine(Data.GetData());
        ScreensaverImages = new List<PollImageComponent>();
        for(var i = 0; i < Data.ScreensaverImages.Count; i++)
        {
            var screensaverImageComponent = Instantiate(ScreensaverImagePrefab).GetComponent<PollImageComponent>();
            screensaverImageComponent.transform.SetParent(transform);
            screensaverImageComponent.CreateObjects(Data.ScreensaverImages[i]);
            screensaverImageComponent.HideObjects();
            ScreensaverImages.Add(screensaverImageComponent);
        }
        CountdownComponent.CreateObjects();
        HideObjects();
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
        StartCoroutine(ShowNextScreensaverImage());
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
            BtnStartOver.gameObject.SetActive(false);
            CurrentScreensaverImage = -1;
        }
        else
        {
            ScreensaverImages[CurrentScreensaverImage].ShowObjects();
            LeaderboardManager.Instance.HideLeaderboardScreensaver();
            BtnStartOver.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(3);
        yield return ShowNextScreensaverImage();
    }

    public void HideObjects()
    {
        Hidden = true;
        CountdownHidden = true;
        gameObject.SetActive(false);
    }

    public void FinishScreensaver()
    {
        HideObjects();
    }

    public void Update()
    {
        if (!Hidden)
        {
            if (Input.anyKeyDown || Input.touchCount > 0)
            {
                FinishScreensaver();
            }
        }
    }
}
