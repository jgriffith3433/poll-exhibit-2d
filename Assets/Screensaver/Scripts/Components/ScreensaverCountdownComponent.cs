using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreensaverCountdownComponent : MonoBehaviour {

    public bool Hidden;
    public int CountdownFrom = 10;
    public PollTextComponent CountdownTitle;
    public PollTextComponent CountdownNumber;

    private float CountdownStartTime;

    public void CreateObjects()
    {
        HideObjects();
    }

    private void ShowObjects()
    {
        Hidden = false;
        gameObject.SetActive(true);
    }

    private void HideObjects()
    {
        Hidden = true;
        gameObject.SetActive(false);
    }

    public void StartCountdown()
    {
        CountdownStartTime = Time.time;
        ShowObjects();
    }

    public void FinishCountdown()
    {
        HideObjects();
        ScreensaverManager.Instance.FinishCountdown();
    }

    public void CancelCountdown()
    {
        HideObjects();
    }

    public void Update()
    {
        if (!Hidden)
        {
            CountdownNumber.SetTextData(Mathf.FloorToInt(CountdownFrom + CountdownStartTime - Time.time).ToString());
            CountdownNumber.CreateAllObjects();
            if (Time.time > CountdownStartTime + CountdownFrom)
            {
                FinishCountdown();
            }
        }
    }
}
