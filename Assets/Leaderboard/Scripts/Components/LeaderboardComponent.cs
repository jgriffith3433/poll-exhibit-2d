using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardComponent : MonoBehaviour
{
    public LeadboardTopDownComponent LeadboardTopDownPrefab;
    private LeadboardTopDownComponent LeadboardTopDownInstance;
    private LeaderboardData Data;
    private bool Loading = true;

    public bool Hidden;

    public void FillLeaderboard()
    {
        Loading = true;
        ShowObjects();
        Data = new LeaderboardData(Application.dataPath + "/poll-leaderboard.json");
        StartCoroutine(Data.GetData());
        StartCoroutine(CheckIsDoneParsing());
    }

    IEnumerator CheckIsDoneParsing()
    {
        if (Data != null && Data.IsDoneParsing)
        {
            CreateObjects();
            Loading = false;
        }
        else
        {
            yield return new WaitForSeconds(1);
            yield return CheckIsDoneParsing();
        }
    }

    public void CreateObjects()
    {
        LeadboardTopDownInstance = Instantiate(LeadboardTopDownPrefab).GetComponent<LeadboardTopDownComponent>();
        LeadboardTopDownInstance.transform.SetParent(transform);
        LeadboardTopDownInstance.SetPlayerData(Data.PlayerData);
        LeadboardTopDownInstance.CreateObjects();
    }

    public void ShowObjects()
    {
        gameObject.SetActive(true);
        Hidden = false;
    }

    public void HideObjects()
    {
        Hidden = true;
        gameObject.SetActive(false);
    }

    public void Update()
    {
        if (!Hidden && !Loading && ExhibitGameManager.Instance.CurrentGameState != "Screensaver")
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
                LeaderboardManager.Instance.OnPlay();
            }
        }
    }
}
